using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbrigoAPIMinimal.Model
{
    [Table("tabela_Evento")]
    public class Evento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Descricao { get; set; }

        public DateTime Data { get; set; }

        [Required]
        public int AbrigoId { get; set; }
    }
}
