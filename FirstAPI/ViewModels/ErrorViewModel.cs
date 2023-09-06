namespace FirstAPI.ViewModels;
public class ErrorViewModel
{
    public ErrorDetail Error { get; set; }

    public ErrorViewModel(string type, string message)
    {
        Error = new ErrorDetail { Type = type, Message = message };
    }
}

public class ErrorDetail
{
    public string Type { get; set; }
    public string Message { get; set; }
}
