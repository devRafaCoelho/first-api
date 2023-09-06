﻿using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels;

public class UserViewModel
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? CPF { get; set; }

    public string? Phone { get; set; }
}

