using AbrigoAPIMinimal.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AbrigoAPITests
{
    public class TesteRecursoEndpoints : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TesteRecursoEndpoints(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllRecursos_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/recursos/cadastrados");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetRecursoById_ReturnsOkOrNotFound()
        {
            // Act
            var response = await _client.GetAsync("/recursos/buscar/1"); // ID fictício

            // Assert
            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task CreateRecurso_ReturnsCreated()
        {
            var novoRecurso = new Recurso
            {
                Nome = "Água Potável",
                Tipo = "Alimentação",
                Quantidade = 50,
                DataValidade = DateTime.UtcNow.AddMonths(6),
                AbrigoId = 1
            };

            var response = await _client.PostAsJsonAsync("/recursos/criar", novoRecurso);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Resposta da API: {responseBody}");
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateRecurso_ReturnsBadRequest_WhenDataIsInvalid()
        {
            var invalidRecurso = new Recurso
            {
                Nome = "", // Nome inválido
                Tipo = "",
                Quantidade = -10, // Quantidade negativa
                AbrigoId = 0 // Abrigo inválido
            };

            var response = await _client.PostAsJsonAsync("/recursos/criar", invalidRecurso);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateRecurso_ReturnsOkOrNotFound()
        {
            var recursoAtualizado = new Recurso
            {
                Id = 1, // ID fictício
                Nome = "Medicamentos Essenciais",
                Tipo = "Saúde",
                Quantidade = 20,
                DataValidade = DateTime.UtcNow.AddYears(1),
                AbrigoId = 1
            };

            var response = await _client.PutAsJsonAsync("/recursos/atualizar/1", recursoAtualizado);

            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task DeleteRecurso_ReturnsNoContentOrNotFound()
        {
            var response = await _client.DeleteAsync("/recursos/deletar/1"); // ID fictício

            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task GetRecursoById_ReturnsNotFound_WhenRecursoDoesNotExist()
        {
            var response = await _client.GetAsync("/recursos/buscar/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRecurso_ReturnsNotFound_WhenRecursoDoesNotExist()
        {
            var response = await _client.DeleteAsync("/recursos/deletar/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
