using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigoAPIMinimal.Model
{
    [Table("tabela_Recurso")]
    public class Recurso
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        public string Tipo { get; set; } // Ex: Alimentação, Higiene, Medicamento

        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        public DateTime DataValidade { get; set; }

        [Required]
        public int AbrigoId { get; set; }
    }
}
