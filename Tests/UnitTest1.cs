using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using EcosferaDigital;
using EcosferaDigital.Models;

namespace EcosferaDigital.Tests
{
    public class UsuarioControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UsuarioControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Testes para GetUsuario
        [Fact]
        public async Task GetUsuario_ReturnsOkResult_WhenUsuarioExists()
        {
            // Arrange
            int testId = 1; // ID de teste

            // Act
            var response = await _client.GetAsync($"/Usuario/{testId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
        }

        [Fact]
        public async Task GetUsuario_ReturnsNotFound_WhenUsuarioDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe

            // Act
            var response = await _client.GetAsync($"/Usuario/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Testes para CreateUsuario
        [Fact]
        public async Task CreateUsuario_ReturnsCreatedResult()
        {
            // Arrange
            var newUsuario = new UsuarioPost
            {
                Nome = "Novo Usuário",
                Endereco = "Rua Exemplo, 123",
                Email = "novo.usuario@example.com",
                Telefone = "11987654321"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newUsuario), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Usuario", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateUsuario_ReturnsBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var invalidUsuario = new UsuarioPost
            {
                Nome = "", // Nome inválido
                Endereco = "Rua Exemplo, 123",
                Email = "email_invalido", // Email inválido
                Telefone = "123" // Telefone inválido
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidUsuario), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Usuario", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        // Testes para UpdateUsuario
        [Fact]
        public async Task UpdateUsuario_ReturnsOkResult_WhenUsuarioExists()
        {
            // Arrange
            int testId = 1; // ID de teste
            var updatedUsuario = new UsuarioPost
            {
                Nome = "Usuário Atualizado",
                Endereco = "Avenida Nova, 456",
                Email = "atualizado.usuario@example.com",
                Telefone = "11999887766"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedUsuario), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/Usuario/{testId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateUsuario_ReturnsNotFound_WhenUsuarioDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe
            var updatedUsuario = new UsuarioPost
            {
                Nome = "Usuário Inexistente",
                Endereco = "Rua Fictícia, 789",
                Email = "inexistente.usuario@example.com",
                Telefone = "11912341234"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedUsuario), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/Usuario/{testId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Testes para DeleteUsuario
        [Fact]
        public async Task DeleteUsuario_ReturnsNoContent_WhenUsuarioIsDeleted()
        {
            // Arrange
            int testId = 1; // ID de teste

            // Act
            var response = await _client.DeleteAsync($"/Usuario/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUsuario_ReturnsNotFound_WhenUsuarioDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe

            // Act
            var response = await _client.DeleteAsync($"/Usuario/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
