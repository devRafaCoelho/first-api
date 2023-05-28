using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels;

public class ClientViewModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? CPF { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Complement { get; set; }

    public string? Zip_Code { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public string? UF { get; set; }

    public string? Status { get; set; }
}

