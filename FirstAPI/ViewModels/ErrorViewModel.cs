namespace FirstAPI.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorViewModel(string email)
        {
            Error = new { email };
        }

        public object Error { get; set; }
    }
}
