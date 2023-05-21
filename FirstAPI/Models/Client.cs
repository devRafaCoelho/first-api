using System.ComponentModel.DataAnnotations;

namespace FirstAPI.Models;

public class Client
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O Nome é obrigatório.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "O E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string? Email { get; set; }


    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF inválido.")]
    public string? CPF { get; set; }


    [Required(ErrorMessage = "O Número de Telefone é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "Número de Telefone inválido.")]
    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Complement { get; set; }

    [StringLength(8, MinimumLength = 8, ErrorMessage = "CEP inválido.")]
    public string? Zip_Code { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    [StringLength(2, MinimumLength = 2, ErrorMessage = "Sigla de Estado inválida.")]
    public string? UF { get; set; }
}

