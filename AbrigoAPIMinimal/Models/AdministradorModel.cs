using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AbrigoAPIMinimal.Models
{
    [Table("ADMINISTRADOR")]
    public class AdministradorModel
    {
        [Column("role_user")]
        [MaxLength(50)] // Define o comprimento máximo
        public string Role { get; set; } = "User";
        [Required]
        [MinLength(6)]
        [MaxLength(128)] // Define o comprimento máximo
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
