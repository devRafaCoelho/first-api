﻿using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels
{
    public class UpdateUserViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail é inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; }
    }
}