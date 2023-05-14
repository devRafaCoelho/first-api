namespace FirstAPI.ViewModels
{
    public class MessageViewModel
    {
        public object Message { get; set; }

        public MessageViewModel(string message)
        {
            Message = new { message };
        }
    }
}
