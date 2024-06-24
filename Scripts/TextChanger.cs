using System.IO;
using Godot;
using Vosk;

public partial class TextChanger : Node 
{
    VoskRecognizer recognizer;
    private string pathToAudio;
    private string pathToModel;
    private AudioStreamWav audioInfo;

    [Export] private Label partialLabel;
    [Export] private Label resultLabel;

    public void InitVosk()
    {
        if(!PathsAreSet()) return; 

        audioInfo = ResourceLoader.Load<AudioStreamWav>(pathToAudio);

        if(audioInfo == null)
        {
            Alerts.instance.ShowError("Failed to load audio");
            return;
        }

        Model model = new Model(pathToModel);
        recognizer = new VoskRecognizer(model, audioInfo.MixRate);
        recognizer.SetMaxAlternatives(0);
        recognizer.SetWords(true);
        ProcessData();
    }

    private void ProcessData()
    {
        Stream source = File.OpenRead(pathToAudio);
        byte[] deltaSamples = new byte[GetDeltaSamples()];
        int bytesRead;
        while ((bytesRead = source.Read(deltaSamples, 0, deltaSamples.Length)) > 0)
        {
            Json json = new Json();
            Error err;
            if(recognizer.AcceptWaveform(deltaSamples, bytesRead))
            {
               err = json.Parse(recognizer.FinalResult());
            
               if (err != Error.Ok)
               {
                   Alerts.instance.ShowError("Parsing final result failed");
                   continue;
               }

               resultLabel.Text = json.Data.AsGodotDictionary()["text"].ToString();
            }
            else
            {
               err = json.Parse(recognizer.PartialResult());

               if (err != Error.Ok)
               {
                   Alerts.instance.ShowError("Parsing partial result failed");
                   continue;
               }
               partialLabel.Text = json.Data.AsGodotDictionary()["partial"].ToString(); 
            }
        }
    }

    public void SetAudioPath(string audioPath)
    {
        pathToAudio = audioPath;
    }

    public void SetModelPath(string modelPath)
    {
        pathToModel = modelPath;
    }

    private bool PathsAreSet()
    {
        if(pathToModel == null )
        { 
            Alerts.instance.ShowWarning("Path to model not set");
            return false;
        }

        if (pathToAudio == null)
        {
            Alerts.instance.ShowWarning("Path to audio not set");
            return false;
        }
        return true;
    }

    private int GetDeltaSamples()
    {
        return (GetDeltaTimeInMillis() * audioInfo.MixRate) / 1000;
    }

    private int GetDeltaTimeInMillis()
    {
        return (int)(GetPhysicsProcessDeltaTime() * 1000);
    }

    public override void _ExitTree()
    {
       recognizer.Dispose();
    }
}
