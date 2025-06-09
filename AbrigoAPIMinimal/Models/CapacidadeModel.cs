using Microsoft.ML.Data;

namespace AbrigoAPIMinimal.Models
{
    public class CapacidadeModel
    {
        public float Capacidade { get; set; }
        public float OcupacaoAtual{ get; set; }
        public float HistoricoOcupacao{ get; set; }
        public bool Infraestrutura{ get; set; }
        public bool EventoEmergencia{ get; set; }
        public float CondicaoClimatica { get; set; }

        [ColumnName("PrevisaoOcupacao")]
        public bool PrevisaoOcupacao { get; set; }
    }
}
