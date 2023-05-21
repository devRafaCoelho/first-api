using System.ComponentModel.DataAnnotations;

namespace FirstAPI.Models;

public class UpdateUser
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O Nome é obrigatório.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "O E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A Senha é obrigatória.")]
    public string? Password { get; set; }

    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF inválido.")]
    public string? CPF { get; set; }

    [StringLength(11, MinimumLength = 11, ErrorMessage = "Número de Telefone inválido.")]
    public string? Phone { get; set; }
}