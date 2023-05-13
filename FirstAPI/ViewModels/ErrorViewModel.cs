namespace FirstAPI.ViewModels;

public class ErrorViewModel
{
    public object Error { get; set; }

    public ErrorViewModel(string error)
    {
        Error = new { error };
    }
}