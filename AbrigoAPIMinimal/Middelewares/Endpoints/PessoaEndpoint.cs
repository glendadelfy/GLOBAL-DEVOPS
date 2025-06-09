using AbrigoAPIMinimal.Context.Database;
using AbrigoAPIMinimal.Model;
using AbrigoAPIMinimal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AbrigoAPIMinimal.Middelewares.Endpoints
{
    public static class PessoaEndpoint
    {
        public static void RegisterPessoaEndpoints(this WebApplication app)
        {
            var pessoaItemsGroup = app.MapGroup("/pessoas");
            //app.MapGet("/dados-seguros", [Authorize] () =>
            //{
            //    return "Este é um dado protegido por JWT!";
            //});
            app.MapPost("/admin/login", (IConfiguration config, AdministradorModel usuario) =>
            {
                // Simulação de validação de usuário (substitua por validação real)
                if (usuario.Role != "admin" || usuario.Password != "123456")
                    return Results.Unauthorized();

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
        new Claim(JwtRegisteredClaimNames.Sub, usuario.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

                var token = new JwtSecurityToken(
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new { token = tokenString });
            });

            pessoaItemsGroup.MapGet("/cadastradas", GetAllPessoas)
                .WithSummary("Lista todas as pessoas cadastradas")
                .WithDescription("Acessa o banco de dados e retorna todas as pessoas cadastradas.");

            pessoaItemsGroup.MapGet("/buscar/{id}", GetPessoa)
                .WithSummary("Busca uma pessoa por ID")
                .WithDescription("Procura no banco de dados uma pessoa correspondente ao ID fornecido.");

            pessoaItemsGroup.MapPost("/criar", CreatePessoa)
                .WithSummary("Cria uma nova pessoa")
                .WithDescription("Adiciona uma nova pessoa ao banco de dados.");

            pessoaItemsGroup.MapPut("/atualizar/{id}", UpdatePessoa)
                .WithSummary("Atualiza dados de uma pessoa")
                .WithDescription("Atualiza os dados de uma pessoa específica baseado no ID fornecido.");

            pessoaItemsGroup.MapDelete("/deletar/{id}", DeletePessoa)
                .WithSummary("Deleta uma pessoa")
                .WithDescription("Remove uma pessoa específica do banco de dados.");
        }

        #region Métodos de Endpoints
        static async Task<IResult> GetAllPessoas(AppDbContext db)
        {
            var pessoas = await db.Pessoas.ToListAsync();
            return TypedResults.Ok(pessoas);
        }

        static async Task<IResult> GetPessoa(int id, AppDbContext db)
        {
            var pessoa = await db.Pessoas.FindAsync(id);
            return pessoa is not null
                ? TypedResults.Ok(pessoa)
                : TypedResults.NotFound();
        }

        static async Task<IResult> CreatePessoa(Pessoa pessoa, AppDbContext db)
        {
            db.Pessoas.Add(pessoa);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/pessoas/{pessoa.Id}", pessoa);
        }

        static async Task<IResult> UpdatePessoa(int id, Pessoa updatedPessoa, AppDbContext db)
        {
            var pessoa = await db.Pessoas.FindAsync(id);
            if (pessoa is null) return TypedResults.NotFound();

            pessoa.Nome = updatedPessoa.Nome;
            pessoa.Idade = updatedPessoa.Idade;
            pessoa.Genero = updatedPessoa.Genero;
            pessoa.NecessidadesEspeciais = updatedPessoa.NecessidadesEspeciais;
            pessoa.AbrigoId = updatedPessoa.AbrigoId;
            await db.SaveChangesAsync();

            return TypedResults.Ok(pessoa);
        }

        static async Task<IResult> DeletePessoa(int id, AppDbContext db)
        {
            var pessoa = await db.Pessoas.FindAsync(id);
            if (pessoa is null) return TypedResults.NotFound();

            db.Pessoas.Remove(pessoa);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }
        #endregion
    }
}
