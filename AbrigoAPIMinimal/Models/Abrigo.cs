using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigoAPIMinimal.Model
{
    [Table("tabela_Abrigo")]
    public class Abrigo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        public string Localizacao { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacidade { get; set; }

        public List<Pessoa> Moradores { get; set; } 
    }
}
