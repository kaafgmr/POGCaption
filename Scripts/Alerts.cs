using Godot;

public partial class Alerts : AcceptDialog
{
    public static Alerts instance;
	public override void _Ready()
	{
        instance = this;
        Label myLabel = GetLabel();
        myLabel.HorizontalAlignment = HorizontalAlignment.Center;
	}

    public void ShowWarning(string warningText)
    {
        Show();
        Title = "Warning!!!";
        DialogText = warningText;
        OkButtonText = "OK I'll fix it :D";
    }

    public void ShowError(string errorText)
    {
        Show();
        Title = "Error!!!";
        DialogText = errorText;
        OkButtonText = "Tell the developer that he needs to fix this ASAP!!!";
    }
}
