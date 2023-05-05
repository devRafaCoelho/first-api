using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels
{
    public class RegisterProductViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
