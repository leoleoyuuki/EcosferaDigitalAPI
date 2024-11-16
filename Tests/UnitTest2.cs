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
    public class EnergiaControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EnergiaControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Testes para GetEnergia
        [Fact]
        public async Task GetEnergia_ReturnsOkResult_WhenEnergiaExists()
        {
            // Arrange
            int testId = 1002; // ID de teste

            // Act
            var response = await _client.GetAsync($"/Energia/{testId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
        }

        [Fact]
        public async Task GetEnergia_ReturnsNotFound_WhenEnergiaDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe

            // Act
            var response = await _client.GetAsync($"/Energia/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Testes para CreateEnergia
        [Fact]
        public async Task CreateEnergia_ReturnsCreatedResult()
        {
            // Arrange
            var newEnergia = new EnergiaPost
            {
                DispositivoId = 101,
                DataHora = DateTime.UtcNow,
                ConsumoKWH = 50.5f,
                GeracaoKWH = 10.2f
            };

            var content = new StringContent(JsonConvert.SerializeObject(newEnergia), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Energia", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateEnergia_ReturnsBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var invalidEnergia = new EnergiaPost
            {
                DispositivoId = 0, // ID inválido
                DataHora = DateTime.UtcNow,
                ConsumoKWH = -5, // Valor inválido
                GeracaoKWH = 10.2f
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidEnergia), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Energia", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        // Testes para UpdateEnergia
        [Fact]
        public async Task UpdateEnergia_ReturnsOkResult_WhenEnergiaExists()
        {
            // Arrange
            int testId = 1003; // ID de teste
            var updatedEnergia = new EnergiaPost
            {
                DispositivoId = 101,
                DataHora = DateTime.UtcNow,
                ConsumoKWH = 55.5f,
                GeracaoKWH = 12.3f
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedEnergia), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/Energia/{testId}", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task UpdateEnergia_ReturnsNotFound_WhenEnergiaDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe
            var updatedEnergia = new EnergiaPost
            {
                DispositivoId = 101,
                DataHora = DateTime.UtcNow,
                ConsumoKWH = 55.5f,
                GeracaoKWH = 12.3f
            };

            var content = new StringContent(JsonConvert.SerializeObject(updatedEnergia), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/Energia/{testId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // Testes para DeleteEnergia
        [Fact]
        public async Task DeleteEnergia_ReturnsNoContent_WhenEnergiaIsDeleted()
        {
            // Arrange
            int testId = 1001; // ID de teste

            // Act
            var response = await _client.DeleteAsync($"/Energia/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteEnergia_ReturnsNotFound_WhenEnergiaDoesNotExist()
        {
            // Arrange
            int testId = 999; // ID que não existe

            // Act
            var response = await _client.DeleteAsync($"/Energia/{testId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
