using System.ComponentModel.DataAnnotations;

namespace FirstAPI.ViewModels
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
