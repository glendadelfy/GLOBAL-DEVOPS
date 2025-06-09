using Microsoft.ML.Data;
using Microsoft.ML;
using AbrigoAPIMinimal.Services;
using AbrigoAPIMinimal.Models;
using System.Reflection.PortableExecutable;

namespace AbrigoAPIMinimal.Middelewares.Endpoints
{
    public static class CapacidadeAbrigoEndpoint
    {
        public static void RegisterAbrigoCapacidadeEndpoint(this WebApplication app)
        {
            var riscoOdontoGroup = app.MapGroup("/capacidade-abrigo");

            riscoOdontoGroup.MapPost("/treinar-modelo", (List<CapacidadeModel> abrigos) =>
            {
                var mlContext = new MLContext();
                var dadosTreinamento = mlContext.Data.LoadFromEnumerable(abrigos);

                var pipeline = mlContext.Transforms.CopyColumns("Label", "PrevisaoOcupacao")
                    .Append(mlContext.Transforms.Conversion.ConvertType(
                        new[]
                        {
                        new InputOutputColumnPair("Infraestrutura"),
                        new InputOutputColumnPair("EventoEmergencia"),
                        new InputOutputColumnPair("CondicaoClimatica")
                        },
                        outputKind: DataKind.Single))
                    .Append(mlContext.Transforms.Concatenate("Features",
                        nameof(CapacidadeModel.Capacidade),
                        nameof(CapacidadeModel.OcupacaoAtual),
                        nameof(CapacidadeModel.HistoricoOcupacao),
                        "Infraestrutura",
                        "EventoEmergencia",
                        "CondicaoClimatica"))
                    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

                var modelo = pipeline.Fit(dadosTreinamento);
                mlContext.Model.Save(modelo, dadosTreinamento.Schema, "modelo_abrigo.zip");

                return Results.Ok("✅ Modelo treinado e salvo com sucesso!");
            })
            .WithSummary("Treina e salva o modelo de ocupação de abrigos")
            .WithDescription("Recebe uma lista de dados e treina um modelo de previsão de ocupação.");

            riscoOdontoGroup.MapPost("/prever", (List<CapacidadeModel> abrigos) =>
            {
                var mlContext = new MLContext();
                var modelo = mlContext.Model.Load("modelo_abrigo.zip", out var schema);

                var predictionEngine = mlContext.Model.CreatePredictionEngine<CapacidadeModel, AbrigoPrediction>(modelo);
                var resultados = abrigos.Select(abrigo => predictionEngine.Predict(abrigo)).ToList();

                return Results.Ok(resultados);
            })
            .WithSummary("Realiza previsões de ocupação de abrigos")
            .WithDescription("Usa um modelo de Machine Learning para prever a ocupação futura com base em dados históricos.");

        }
    }
}
