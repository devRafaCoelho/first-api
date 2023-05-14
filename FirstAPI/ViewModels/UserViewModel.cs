using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels;

public class UserViewModel
{
    [Required(ErrorMessage = "O Nome é obrigatório.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "O E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A Senha é obrigatória.")]
    public string? Password { get; set; }
}

