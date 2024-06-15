using System.IO;
using Godot;
using Vosk;

public partial class TextChanger : Label
{
    VoskRecognizer recognizer;
    private string pathToAudio;
    private string pathToModel;

    private const string emptyResult = "\"\"";

    public void InitVosk()
    {
        if(!PathsAreSet()) return; 

        Model model = new Model(pathToModel);
        recognizer = new VoskRecognizer(model, 16000.0f);
        recognizer.SetMaxAlternatives(0);
        recognizer.SetWords(true);
        ProcessData();
    }

    private void ProcessData()
    {
        Stream source = File.OpenRead(pathToAudio);
        byte[] buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
        {
            if(!recognizer.AcceptWaveform(buffer, bytesRead) && !recognizer.PartialResult().Contains(emptyResult))
            {
                ChangeLabelText(recognizer.PartialResult());
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

    private void ChangeLabelText(string newText)
    {
        Text = newText;
    }

    private bool PathsAreSet()
    {
        LabelSettings.FontColor = Colors.Yellow;
        if(pathToModel == null )
        { 
            Text = "Select the model path";
            return false;
        }

        if (pathToAudio == null)
        {
            Text = "Select an audio to transcribe";
            return false;
        }

        LabelSettings.FontColor = Colors.White;
        return true;
    }

    public override void _ExitTree()
    {
       recognizer.Dispose();
    }
}
