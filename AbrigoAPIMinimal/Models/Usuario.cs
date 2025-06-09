using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigoAPIMinimal.Model
{
    [Table("tabela_Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Senha{ get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } // Ex: Admin, Gestor
    }
}
