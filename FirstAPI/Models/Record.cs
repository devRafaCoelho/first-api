using System.ComponentModel.DataAnnotations;

namespace FirstAPI.Models;

public class Record
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O Id do cliente é obrigatório.")]
    public int Id_Client { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "A data é obrigatória.")]
    public DateTime Due_Date { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório.")]
    public int Value { get; set; }

    [Required(ErrorMessage = "O Paid_Out é obrigatório.")]
    public bool Paid_Out { get; set; }
}

