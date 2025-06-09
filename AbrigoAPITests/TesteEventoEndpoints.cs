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
        public class TesteEventoEndpoints: IClassFixture<WebApplicationFactory<Program>>
    {
    
        private readonly HttpClient _client;

        public TesteEventoEndpoints(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        //[Fact]
        //public async Task GetAllEventos_ReturnsOk()
        //{
        //    // Act
        //    var response = await _client.GetAsync("/eventos/todos");

        //    // Assert
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}

        [Fact]
        public async Task GetEventoById_ReturnsOkOrNotFound()
        {
            // Act
            var response = await _client.GetAsync("/eventos/buscar/1"); // ID fictício

            // Assert
            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task CreateEvento_ReturnsCreated()
        {
            var novoEvento = new Evento
            {
                Descricao = "Entrega de alimentos",
                Data = DateTime.UtcNow,
                AbrigoId = 1
            };

            var response = await _client.PostAsJsonAsync("/eventos/criar", novoEvento);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Resposta da API: {responseBody}");
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateEvento_ReturnsBadRequest_WhenDataIsInvalid()
        {
            var invalidEvento = new Evento
            {
                Descricao = "",
                Data = DateTime.MinValue,
                AbrigoId = 0
            };

            var response = await _client.PostAsJsonAsync("/eventos/criar", invalidEvento);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateEvento_ReturnsOkOrNotFound()
        {
            var eventoAtualizado = new Evento
            {
                Id = 1, // ID fictício
                Descricao = "Doação de roupas",
                Data = DateTime.UtcNow,
                AbrigoId = 1
            };

            var response = await _client.PutAsJsonAsync("/eventos/atualizar/1", eventoAtualizado);

            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task DeleteEvento_ReturnsNoContentOrNotFound()
        {
            var response = await _client.DeleteAsync("/eventos/deletar/1"); // ID fictício

            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task GetEventoById_ReturnsNotFound_WhenEventoDoesNotExist()
        {
            var response = await _client.GetAsync("/eventos/buscar/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteEvento_ReturnsNotFound_WhenEventoDoesNotExist()
        {
            var response = await _client.DeleteAsync("/eventos/deletar/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
   
    }
}
