using System.ComponentModel.DataAnnotations;

namespace FirstAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O Primeiro Nome é obrigatório.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "O Último Nome é obrigatório.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "O E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "A Senha é obrigatória.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "A Confirmação da Senha é obrigatória.")]
    public string? ConfirmPassword { get; set; }

    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF inválido.")]
    public string? CPF { get; set; }

    [StringLength(11, MinimumLength = 11, ErrorMessage = "Número de Telefone inválido.")]
    public string? Phone { get; set; }
}