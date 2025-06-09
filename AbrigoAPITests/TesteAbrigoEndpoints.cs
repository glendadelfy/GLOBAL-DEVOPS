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
    public class TesteAbrigoEndpoints: IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TesteAbrigoEndpoints(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllAbrigos_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/abrigos/disponiveis");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAbrigoById_ReturnsOkOrNotFound()
        {
            // Act
            var response = await _client.GetAsync("/abrigos/buscar/1"); // ID fictício

            // Assert
            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task CreateAbrigo_ReturnsCreated()
        {
            var novoAbrigo = new Abrigo
            {
                Nome = "Abrigo São Paulo",
                Localizacao = "Rua das Flores, SP",
                Capacidade = 100
            };

            var response = await _client.PostAsJsonAsync("/abrigos/criar", novoAbrigo);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Resposta da API: {responseBody}");
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateAbrigo_ReturnsBadRequest_WhenDataIsInvalid()
        {
            var invalidAbrigo = new Abrigo
            {
                Nome = "", // Nome inválido
                Localizacao = "",
                Capacidade = 0 // Capacidade inválida
            };

            var response = await _client.PostAsJsonAsync("/abrigos/criar", invalidAbrigo);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAbrigo_ReturnsOkOrNotFound()
        {
            var abrigoAtualizado = new Abrigo
            {
                Id = 1, // ID fictício
                Nome = "Abrigo Atualizado",
                Localizacao = "Avenida Central, RJ",
                Capacidade = 150
            };

            var response = await _client.PutAsJsonAsync("/abrigos/atualizar/1", abrigoAtualizado);

            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task DeleteAbrigo_ReturnsNoContentOrNotFound()
        {
            var response = await _client.DeleteAsync("/abrigos/deletar/1"); // ID fictício

            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task GetAbrigoById_ReturnsNotFound_WhenAbrigoDoesNotExist()
        {
            var response = await _client.GetAsync("/abrigos/buscar/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAbrigo_ReturnsNotFound_WhenAbrigoDoesNotExist()
        {
            var response = await _client.DeleteAsync("/abrigos/deletar/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
