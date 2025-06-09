using AbrigoAPIMinimal.Context.Database;
using AbrigoAPIMinimal.Middelewares.Endpoints;
using AbrigoAPIMinimal.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// desenvolvido por Glenda Delfy 06/06/2025

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Abrigo API",
        Version = "v1",
        Description = "API para gerenciamento de abrigos temporários.",
        Contact = new OpenApiContact
        {
            Name = "Glenda Delfy",
            Email = "glendadelfy20@gmail.com",
            Url = new Uri("https://github.com/glendadelfy/GLOBAL-DOTNET")
        }
    });
    options.MapType<Abrigo>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            { "Id", new OpenApiSchema { Type = "integer", Description = "Identificador único do abrigo" } },
            { "Nome", new OpenApiSchema { Type = "string", Description = "Nome do abrigo" } },
            { "Localizacao", new OpenApiSchema { Type = "string", Description = "Endereço ou coordenadas do abrigo" } },
            { "Capacidade", new OpenApiSchema { Type = "integer", Description = "Capacidade máxima de ocupação" } },
            { "Moradores", new OpenApiSchema { Type = "array", Description = "Lista de pessoas abrigadas", Items = new OpenApiSchema { Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "Pessoa" } } } }
        }
    });
    options.MapType<Pessoa>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            { "Id", new OpenApiSchema { Type = "integer", Description = "Identificador único da pessoa" } },
            { "Nome", new OpenApiSchema { Type = "string", Description = "Nome completo da pessoa" } },
            { "Idade", new OpenApiSchema { Type = "integer", Description = "Idade da pessoa" } },
            { "Genero", new OpenApiSchema { Type = "string", Description = "Gênero da pessoa (masculino, feminino, não-binário, etc.)" } },
            { "NecessidadesEspeciais", new OpenApiSchema { Type = "boolean", Description = "Indica se a pessoa tem necessidades especiais" } },
            { "AbrigoId", new OpenApiSchema { Type = "integer", Description = "Identificador do abrigo onde a pessoa está registrada" } }
        }
    });
    options.MapType<Evento>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            { "Id", new OpenApiSchema { Type = "integer", Description = "Identificador único do evento" } },
            { "Descricao", new OpenApiSchema { Type = "string", Description = "Descrição do evento ocorrido no abrigo" } },
            { "Data", new OpenApiSchema { Type = "string", Format = "date-time", Description = "Data e hora do evento" } },
            { "AbrigoId", new OpenApiSchema { Type = "integer", Description = "Identificador do abrigo onde o evento ocorreu" } },
            { "Tipo", new OpenApiSchema { Type = "string", Description = "Tipo do evento (Entrada, Doação, Emergência, etc.)" } },
            { "UsuarioId", new OpenApiSchema { Type = "integer", Description = "Identificador do usuário que registrou o evento" } },
            { "IsUrgente", new OpenApiSchema { Type = "boolean", Description = "Indica se o evento é uma emergência" } }
        }
    });
    options.MapType<Recurso>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            { "Id", new OpenApiSchema { Type = "integer", Description = "Identificador único do recurso" } },
            { "Nome", new OpenApiSchema { Type = "string", Description = "Nome do recurso" } },
            { "Tipo", new OpenApiSchema { Type = "string", Description = "Tipo de recurso (Alimentação, Higiene, Medicamento, etc.)" } },
            { "Quantidade", new OpenApiSchema { Type = "integer", Description = "Número de unidades disponíveis" } },
            { "DataValidade", new OpenApiSchema { Type = "string", Format = "date-time", Description = "Data de validade do recurso, se aplicável" } },
            { "AbrigoId", new OpenApiSchema { Type = "integer", Description = "Identificador do abrigo onde o recurso está armazenado" } }
        }
    });
    // Configura o Swagger para usar o JWT
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no formato Bearer {seu token}"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Adiciona a autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/gerar-token", (IConfiguration config) =>
{
    // Normalmente, você validaria as credenciais do usuário aqui
    // Para simplificar, vamos assumir que o usuário é válido

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, "usuarioTeste"),
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.RegisterRecursoEndpoints();
app.RegisterPessoaEndpoints();
app.RegisterAbrigoEndpoints();
app.RegisterEventoEndpoints();
app.RegisterAbrigoCapacidadeEndpoint();

// **Mensagem na raiz!**
app.MapGet("/", () => "API rodando no Azure 🚀");

app.Run();

//Code for unit test
public partial class Program { }
