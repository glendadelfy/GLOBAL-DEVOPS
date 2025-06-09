using Microsoft.ML.Data;

namespace AbrigoAPIMinimal.Models
{
    public class AbrigoPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }

    }
}
