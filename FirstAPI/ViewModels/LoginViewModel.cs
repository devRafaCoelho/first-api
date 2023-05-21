using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels;

public class LoginViewModel
{
    public UserViewModel User { get; set; }

    public string? Token { get; set; }
}

