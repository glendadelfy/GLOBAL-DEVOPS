using Microsoft.ML.Data;
using Microsoft.ML;
using AbrigoAPIMinimal.Models;
using AbrigoAPIMinimal.Services;

namespace AbrigoAPIMinimal.Middelewares.Endpoints
{
    public static class AbrigoEndpointML
    {
        public static void RegisterAbrigoEndpoint(this WebApplication app)
        {
            var abrigoGroup = app.MapGroup("/abrigo");

            // Endpoint para treinar e salvar o modelo
            abrigoGroup.MapPost("/treinar-modelo", (List<AbrigoModel> abrigos) =>
            {
                var mlContext = new MLContext();
                var dadosTreinamento = mlContext.Data.LoadFromEnumerable(abrigos);

                var pipeline = mlContext.Transforms.CopyColumns("Label", "PrevisaoOcupacao")
                    .Append(mlContext.Transforms.Conversion.ConvertType(
                        new[]
                        {
                            new InputOutputColumnPair("EventoEmergencial"),
                            new InputOutputColumnPair("CondicaoClimaticaCritica")
                        }, outputKind: DataKind.Single))
                    .Append(mlContext.Transforms.Concatenate("Features",
                        nameof(AbrigoModel.Capacidade),
                        nameof(AbrigoModel.OcupacaoAtual),
                        nameof(AbrigoModel.HistoricoOcupacao),
                        nameof(AbrigoModel.InfraestruturaNota),
                        "EventoEmergencial",
                        "CondicaoClimaticaCritica"))
                    .Append(mlContext.Regression.Trainers.Sdca());

                var modelo = pipeline.Fit(dadosTreinamento);

                string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modelo_abrigo.zip");
                mlContext.Model.Save(modelo, dadosTreinamento.Schema, modelPath);

                return Results.Ok("✅ Modelo treinado e salvo com sucesso!");
            })
            .WithSummary("Treina e salva o modelo de previsão para abrigos")
            .WithDescription("Recebe dados de abrigos e treina um modelo para prever a ocupação.");

            // Endpoint para prever ocupação com base no modelo treinado
            abrigoGroup.MapPost("/prever", (List<AbrigoModel> abrigos) =>
            {
                var mlContext = new MLContext();

                string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modelo_abrigo.zip");

                if (!File.Exists(modelPath))
                {
                    Console.WriteLine("⚠️ O arquivo modelo_abrigo.zip não foi encontrado.");
                    return Results.Problem("Modelo não encontrado.");
                }

                var modelo = mlContext.Model.Load(modelPath, out var schema);
                var predictionEngine = mlContext.Model.CreatePredictionEngine<AbrigoModel, AbrigoPrediction>(modelo);
                var resultados = abrigos.Select(abrigo => predictionEngine.Predict(abrigo)).ToList();

                return Results.Ok(resultados);
            })
            .WithSummary("Realiza previsões de ocupação para abrigos")
            .WithDescription("Usa o modelo treinado para prever a ocupação de abrigos.");

        }
    }
}
