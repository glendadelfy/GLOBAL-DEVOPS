using AbrigoAPIMinimal.Models;
using Microsoft.ML.Data;
using Microsoft.ML;
using Microsoft.EntityFrameworkCore;

namespace AbrigoAPIMinimal.Services
{
    public static class AbrigoService
    {
        public static void RegisterAbrigoEndpointService(this WebApplication app)
        {
            app.MapPost("/treinar-modelo", (List<AbrigoModel> abrigos) =>
            {
                var mlContext = new MLContext();
                var dadosTreinamento = mlContext.Data.LoadFromEnumerable(abrigos);

                var pipeline = mlContext.Transforms.CopyColumns("Label", "PrevisaoOcupacao")
                    .Append(mlContext.Transforms.Conversion.ConvertType(
                        new[]
                        {
                            new InputOutputColumnPair("EventoEmergencia"),
                            new InputOutputColumnPair("CondicaoClimaticaCritica")
                        }, outputKind: DataKind.Single))
                    .Append(mlContext.Transforms.Concatenate("Features",
                        nameof(AbrigoModel.Capacidade),
                        nameof(AbrigoModel.OcupacaoAtual),
                        nameof(AbrigoModel.HistoricoOcupacao),
                        nameof(AbrigoModel.InfraestruturaNota),
                        "EventoEmergencia",
                        "CondicaoClimaticaCritica"))
                    .Append(mlContext.Regression.Trainers.Sdca());

                var modelo = pipeline.Fit(dadosTreinamento);

                // Salva o modelo corretamente
                mlContext.Model.Save(modelo, dadosTreinamento.Schema, "modelo_abrigo.zip");

                Console.WriteLine("✅ Modelo treinado e salvo com sucesso!");

                return Results.Ok("✅ Modelo treinado e salvo com sucesso!");
            });

            app.MapPost("/prever", (List<AbrigoModel> abrigos) =>
            {
                var mlContext = new MLContext();

                if (!File.Exists("modelo_abrigo.zip"))
                {
                    Console.WriteLine("⚠️ O arquivo modelo_abrigo.zip não foi encontrado.");
                    return Results.Problem("Modelo não encontrado.");
                }

                var modelo = mlContext.Model.Load("modelo_abrigo.zip", out var schema);
                var predictionEngine = mlContext.Model.CreatePredictionEngine<AbrigoModel, AbrigoPrediction>(modelo);
                var resultados = abrigos.Select(abrigo => predictionEngine.Predict(abrigo)).ToList();

                return Results.Ok(resultados);
            });
        }
    }
}
