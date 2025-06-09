using AbrigoAPIMinimal.Context.Database;
using AbrigoAPIMinimal.Model;
using Microsoft.EntityFrameworkCore;

namespace AbrigoAPIMinimal.Middelewares.Endpoints
{
    public static class EventoEndpoint
    {
        public static void RegisterEventoEndpoints(this WebApplication app)
        {
            var eventoItemsGroup = app.MapGroup("/eventos");

            eventoItemsGroup.MapGet("/todos", GetAllEventos)
                .RequireAuthorization()
                .WithSummary("Lista todos os eventos registrados")
                .WithDescription("Retorna a lista de eventos relacionados aos abrigos.");

            eventoItemsGroup.MapGet("/buscar/{id}", GetEvento)
                .WithSummary("Busca um evento por ID")
                .WithDescription("Procura no banco de dados um evento correspondente ao ID fornecido.");

            eventoItemsGroup.MapPost("/criar", CreateEvento)
                .WithSummary("Registra um novo evento")
                .WithDescription("Adiciona um novo evento ao banco de dados.");

            eventoItemsGroup.MapPut("/atualizar/{id}", UpdateEvento)
                .WithSummary("Atualiza um evento existente")
                .WithDescription("Atualiza os dados de um evento baseado no ID fornecido.");

            eventoItemsGroup.MapDelete("/deletar/{id}", DeleteEvento)
                .WithSummary("Deleta um evento")
                .WithDescription("Remove um evento específico do banco de dados.");
        }

        #region Métodos de Endpoints
        static async Task<IResult> GetAllEventos(AppDbContext db)
        {
            try
            {
                var eventos = await db.Eventos.ToListAsync();
                return TypedResults.Ok(eventos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter eventos: {ex.Message}");
                return TypedResults.Problem($"Erro interno ao listar eventos: {ex.Message}");
            }
        }


        static async Task<IResult> GetEvento(int id, AppDbContext db)
        {
            var evento = await db.Eventos.FindAsync(id);
            return evento is not null
                ? TypedResults.Ok(evento)
                : TypedResults.NotFound();
        }

        static async Task<IResult> CreateEvento(Evento evento, AppDbContext db)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(evento.Descricao) || evento.AbrigoId < 1 || evento.Data == DateTime.MinValue)
                {
                    return TypedResults.BadRequest("Dados inválidos para Evento.");
                }

                db.Eventos.Add(evento);
                await db.SaveChangesAsync();
                return TypedResults.Created($"/eventos/{evento.Id}", evento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar Evento: {ex.Message}");
                return TypedResults.Problem($"Erro interno ao criar evento: {ex.Message}");
            }
        }


        static async Task<IResult> UpdateEvento(int id, Evento updatedEvento, AppDbContext db)
        {
            var evento = await db.Eventos.FindAsync(id);
            if (evento is null) return TypedResults.NotFound();

            evento.Descricao = updatedEvento.Descricao;
            evento.Data = updatedEvento.Data;
            evento.AbrigoId = updatedEvento.AbrigoId;
            await db.SaveChangesAsync();

            return TypedResults.Ok(evento);
        }

        static async Task<IResult> DeleteEvento(int id, AppDbContext db)
        {
            var evento = await db.Eventos.FindAsync(id);
            if (evento is null) return TypedResults.NotFound();

            db.Eventos.Remove(evento);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        #endregion
    }
}
