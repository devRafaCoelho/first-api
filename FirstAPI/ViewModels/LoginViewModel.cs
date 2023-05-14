using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        public string Password { get; set; }
    }
}
