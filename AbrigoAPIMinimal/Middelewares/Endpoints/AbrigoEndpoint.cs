using AbrigoAPIMinimal.Context.Database;
using AbrigoAPIMinimal.Model;
using Microsoft.EntityFrameworkCore;

namespace AbrigoAPIMinimal.Middelewares.Endpoints
{
    public static class AbrigoEndpoint
    {
        public static void RegisterAbrigoEndpoints(this WebApplication app)
        {
            var abrigoItemsGroup = app.MapGroup("/abrigos");

            abrigoItemsGroup.MapGet("/disponiveis", GetAllAbrigos)
                .WithSummary("Lista todos os abrigos disponíveis")
                .WithDescription("Retorna a lista de abrigos temporários registrados.");

            abrigoItemsGroup.MapGet("/buscar/{id}", GetAbrigo)
                .WithSummary("Busca um abrigo por ID")
                .WithDescription("Procura no banco de dados um abrigo correspondente ao ID fornecido.");

            abrigoItemsGroup.MapPost("/criar", CreateAbrigo)
                .WithSummary("Cria um novo abrigo")
                .WithDescription("Registra um novo abrigo temporário no sistema.");

            abrigoItemsGroup.MapPut("/atualizar/{id}", UpdateAbrigo)
                .RequireAuthorization()
                .WithSummary("Atualiza um abrigo existente")
                .WithDescription("Atualiza os dados de um abrigo baseado no ID fornecido.");

            abrigoItemsGroup.MapDelete("/deletar/{id}", DeleteAbrigo)
                .WithSummary("Deleta um abrigo")
                .WithDescription("Remove um abrigo específico do banco de dados.");
        }

        #region Métodos de Endpoints
        static async Task<IResult> GetAllAbrigos(AppDbContext db)
        {
            var abrigos = await db.Abrigos.ToListAsync();
            return TypedResults.Ok(abrigos);
        }

        static async Task<IResult> GetAbrigo(int id, AppDbContext db)
        {
            var abrigo = await db.Abrigos.FindAsync(id);
            return abrigo is not null
                ? TypedResults.Ok(abrigo)
                : TypedResults.NotFound();
        }

        static async Task<IResult> CreateAbrigo(Abrigo abrigo, AppDbContext db)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(abrigo.Nome) ||
                    string.IsNullOrWhiteSpace(abrigo.Localizacao) ||
                    abrigo.Capacidade < 1)
                {
                    return TypedResults.BadRequest("Dados inválidos para Abrigo.");
                }

                db.Abrigos.Add(abrigo);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/abrigos/{abrigo.Id}", abrigo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar Abrigo: {ex.Message}");
                return TypedResults.Problem($"Erro interno ao criar abrigo: {ex.Message}");
            }
        }


        static async Task<IResult> UpdateAbrigo(int id, Abrigo updatedAbrigo, AppDbContext db)
        {
            var abrigo = await db.Abrigos.FindAsync(id);
            if (abrigo is null) return TypedResults.NotFound();

            abrigo.Nome = updatedAbrigo.Nome;
            abrigo.Localizacao = updatedAbrigo.Localizacao;
            abrigo.Capacidade = updatedAbrigo.Capacidade;
            await db.SaveChangesAsync();

            return TypedResults.Ok(abrigo);
        }

        static async Task<IResult> DeleteAbrigo(int id, AppDbContext db)
        {
            var abrigo = await db.Abrigos.FindAsync(id);
            if (abrigo is null) return TypedResults.NotFound();

            db.Abrigos.Remove(abrigo);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        #endregion
    }
}
