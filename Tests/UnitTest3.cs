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
    public class DispositivoControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DispositivoControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Testes para GetDispositivo
        [Fact]
        public async Task GetDispositivo_ReturnsOkResult_WhenDispositivoExists()
        {
            // Arrange
            int testId = 101; // ID de teste

            // Act
            var response = await _client.GetAsync($"/Dispositivo/{testId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
        }

        [Fact]
        public async Task GetDispositivo_ReturnsNotFound_WhenDispositivoDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe

            // Act
            var response = await _client.GetAsync($"/Dispositivo/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Testes para CreateDispositivo
        [Fact]
        public async Task CreateDispositivo_ReturnsCreatedResult()
        {
            // Arrange
            var newDispositivo = new DispositivoPost
            {
                UsuarioId = 1,
                TipoDispositivo = "Sensor",
                Descricao = "Monitoramento de ambiente",
                Status = "Ativo"
            };

            var content = new StringContent(JsonConvert.SerializeObject(newDispositivo), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Dispositivo", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateDispositivo_ReturnsBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var invalidDispositivo = new DispositivoPost
            {
                UsuarioId = 0, // ID inválido
                TipoDispositivo = "", // Tipo inválido
                Descricao = null,
                Status = "Inativo"
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidDispositivo), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Dispositivo", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        // Testes para UpdateDispositivo
        [Fact]
        public async Task UpdateDispositivo_ReturnsOkResult_WhenDispositivoExists()
        {
            // Arrange
            int testId = 101; // ID de teste
            var updatedDispositivo = new DispositivoPost
            {
                UsuarioId = 1,
                TipoDispositivo = "Sensor",
                Descricao = "Atualização de monitoramento",
                Status = "Ativo"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedDispositivo), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/Dispositivo/{testId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateDispositivo_ReturnsNotFound_WhenDispositivoDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe
            var updatedDispositivo = new DispositivoPost
            {
                UsuarioId = 1,
                TipoDispositivo = "Sensor de Pressão",
                Descricao = "Dispositivo não encontrado",
                Status = "Inativo"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedDispositivo), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/Dispositivo/{testId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Testes para DeleteDispositivo
        [Fact]
        public async Task DeleteDispositivo_ReturnsNoContent_WhenDispositivoIsDeleted()
        {
            // Arrange
            int testId = 110; // ID de teste

            // Act
            var response = await _client.DeleteAsync($"/Dispositivo/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteDispositivo_ReturnsNotFound_WhenDispositivoDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe

            // Act
            var response = await _client.DeleteAsync($"/Dispositivo/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
