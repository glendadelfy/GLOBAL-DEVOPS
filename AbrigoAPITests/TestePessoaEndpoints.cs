using AbrigoAPIMinimal.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AbrigoAPITests
{
    public class TestePessoaEndpoints: IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TestePessoaEndpoints(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllPessoas_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/pessoas/cadastradas");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetPessoaById_ReturnsOkOrNotFound()
        {
            // Act
            var response = await _client.GetAsync("/pessoas/buscar/1"); // ID fictício

            // Assert
            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
        }

        //[Fact]
        //public async Task CreatePessoa_ReturnsCreated()
        //{
        //    var novaPessoa = new Pessoa
        //    {
        //        Nome = "Maria da Silva",
        //        Idade = 30,
        //        Genero = "Feminino",
        //        NecessidadesEspeciais = false,
        //        AbrigoId = 1
        //    };

        //    var response = await _client.PostAsJsonAsync("/pessoas/criar", novaPessoa);
        //    var responseBody = await response.Content.ReadAsStringAsync();

        //    Console.WriteLine($"Resposta da API: {responseBody}");
        //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        //}

        [Fact]
        public async Task CreatePessoa_ReturnsBadRequest_WhenDataIsInvalid()
        {
            var invalidPessoa = new Pessoa
            {
                Nome = "", // Nome inválido
                Idade = -5, // Idade impossível
                Genero = "", // Gênero não informado
                AbrigoId = 0 // Abrigo inválido
            };

            var response = await _client.PostAsJsonAsync("/pessoas/criar", invalidPessoa);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePessoa_ReturnsOkOrNotFound()
        {
            var pessoaAtualizada = new Pessoa
            {
                Id = 1, // ID fictício
                Nome = "Carlos Atualizado",
                Idade = 45,
                Genero = "Masculino",
                NecessidadesEspeciais = true,
                AbrigoId = 1
            };

            var response = await _client.PutAsJsonAsync("/pessoas/atualizar/1", pessoaAtualizada);

            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task DeletePessoa_ReturnsNoContentOrNotFound()
        {
            var response = await _client.DeleteAsync("/pessoas/deletar/1"); // ID fictício

            Assert.Contains(response.StatusCode, new[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound });
        }

        [Fact]
        public async Task GetPessoaById_ReturnsNotFound_WhenPessoaDoesNotExist()
        {
            var response = await _client.GetAsync("/pessoas/buscar/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeletePessoa_ReturnsNotFound_WhenPessoaDoesNotExist()
        {
            var response = await _client.DeleteAsync("/pessoas/deletar/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    

    }
}
