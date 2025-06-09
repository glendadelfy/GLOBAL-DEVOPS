using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigoAPIMinimal.Model
{
    [Table("tabela_Pessoas")]
    public class Pessoa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Range(0, 120)]
        public int Idade { get; set; }

        [Required]
        public string Genero { get; set; }

        public bool NecessidadesEspeciais { get; set; }

        [Required]
        public int AbrigoId { get; set; }
    }
}
