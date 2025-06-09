using Microsoft.ML.Data;
using Microsoft.ML;
using AbrigoAPIMinimal.Models;

namespace AbrigoAPIMinimal.Services
{
    public static class AbrigoCapacidadeService
    {
        public static void RegisterAbrigoCapacidadeEndpointService(this WebApplication app)
        {
            app.MapPost("/treinar-modelo", (List<CapacidadeModel> abrigos) =>
            {
                var mlContext = new MLContext();

                // Converte a lista para IDataView
                var dadosTreinamento = mlContext.Data.LoadFromEnumerable(abrigos);

                var pipeline = mlContext.Transforms.CopyColumns("Label", "PrevisaoOcupacao")
                    .Append(mlContext.Transforms.Conversion.ConvertType(
                        new[]
                        {
                new InputOutputColumnPair("Infrastrutura"),
                new InputOutputColumnPair("EventoEmergencial"),
                new InputOutputColumnPair("CondicaoClimatica")
                        },
                        outputKind: DataKind.Single))
                    .Append(mlContext.Transforms.Concatenate("Features",
                        nameof(CapacidadeModel.Capacidade),
                        nameof(CapacidadeModel.OcupacaoAtual),
                        nameof(CapacidadeModel.HistoricoOcupacao),
                        "Infraestrutura",
                        "EventoEmergencial",
                        "CondicaoClimatica"))
                    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

                var modelo = pipeline.Fit(dadosTreinamento);

                // Salva o modelo em disco
                mlContext.Model.Save(modelo, dadosTreinamento.Schema, "modelo_odonto.zip");

                return Results.Ok("✅ Modelo treinado e salvo com sucesso!");
            });
            app.MapPost("/prever", (List<CapacidadeModel> abrigos) =>

            {
                var mlContext = new MLContext();
                var modelo = mlContext.Model.Load("modelo_odonto.zip", out var schema);

                var predictionEngine = mlContext.Model.CreatePredictionEngine<CapacidadeModel, AbrigoPrediction>(modelo);

                var resultados = abrigos.Select(abrigo => predictionEngine.Predict(abrigo)).ToList();

                return Results.Ok(resultados);
            });
        }
    }
}
