using AbrigoAPIMinimal.Context.Database;
using AbrigoAPIMinimal.Model;
using Microsoft.EntityFrameworkCore;

namespace AbrigoAPIMinimal.Middelewares.Endpoints
{
    public static class RecursoEndpoint
    {
        public static void RegisterRecursoEndpoints(this WebApplication app)
        {
            var recursoItemsGroup = app.MapGroup("/recursos");

            recursoItemsGroup.MapGet("/cadastrados", GetAllRecursos)
                .WithSummary("Lista todos os recursos disponíveis")
                .WithDescription("Acessa o banco de dados e retorna todos os recursos cadastrados.");

            recursoItemsGroup.MapGet("/buscar/{id}", GetRecurso)
                .WithSummary("Busca um recurso por ID")
                .WithDescription("Procura no banco de dados um recurso correspondente ao ID fornecido.");

            recursoItemsGroup.MapPost("/criar", CreateRecurso)
                .WithSummary("Cria um novo recurso") 
                .WithDescription("Adiciona um novo recurso ao banco de dados.");

            recursoItemsGroup.MapPut("/atualizar/{id}", UpdateRecurso)
                .WithSummary("Atualiza um recurso existente")
                .WithDescription("Atualiza os dados de um recurso específico baseado no ID fornecido.");

            recursoItemsGroup.MapDelete("/deletar/{id}", DeleteRecurso)
                .WithSummary("Deleta um recurso")
                .WithDescription("Remove um recurso específico do banco de dados.");
        }
        #region Métodos de Endpoints
        static async Task<IResult> GetAllRecursos(AppDbContext db)
        {
            var recursos = await db.Recursos.ToListAsync();
            return TypedResults.Ok(recursos);
        }

        static async Task<IResult> GetRecurso(int id, AppDbContext db)
        {
            var recurso = await db.Recursos.FindAsync(id);
            return recurso is not null
                ? TypedResults.Ok(recurso)
                : TypedResults.NotFound();
        }

        static async Task<IResult> CreateRecurso(Recurso recurso, AppDbContext db)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(recurso.Nome) ||
                    string.IsNullOrWhiteSpace(recurso.Tipo) ||
                    recurso.Quantidade < 1 ||
                    recurso.AbrigoId < 1)
                {
                    return TypedResults.BadRequest("Dados inválidos para Recurso.");
                }

                db.Recursos.Add(recurso);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/recursos/{recurso.Id}", recurso);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar Recurso: {ex.Message}");
                return TypedResults.Problem($"Erro interno ao criar recurso: {ex.Message}");
            }
        }


        static async Task<IResult> UpdateRecurso(int id, Recurso updatedRecurso, AppDbContext db)
        {
            var recurso = await db.Recursos.FindAsync(id);
            if (recurso is null) return TypedResults.NotFound();

            recurso.Nome = updatedRecurso.Nome;
            recurso.Tipo = updatedRecurso.Tipo;
            recurso.Quantidade = updatedRecurso.Quantidade;
            recurso.DataValidade = updatedRecurso.DataValidade;
            await db.SaveChangesAsync();

            return TypedResults.Ok(recurso);
        }

        static async Task<IResult> DeleteRecurso(int id, AppDbContext db)
        {
            var recurso = await db.Recursos.FindAsync(id);
            if (recurso is null) return TypedResults.NotFound();

            db.Recursos.Remove(recurso);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        #endregion
    }
}
