using System.Text;
using System.Text.Json;
using NUnit.Framework;

namespace ProjectManagementSystem.Test.AuthService.Integration
{
    public class AuthIntegrationTests
    {
        private const string BaseUrl = "http://localhost:5000"; 
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            // Initialize an HttpClient for sending requests to the API
            _httpClient = new HttpClient { BaseAddress = new System.Uri(BaseUrl) };
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up any resources used in the tests
            _httpClient.Dispose();
        }

        [Test]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var validCredentials = new
            {
                Username = "admin",
                Password = "Orenkats95!"
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(validCredentials), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("auth/login", requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Expected a successful response from the API.");
            Assert.IsNotNull(responseContent, "Expected a response body containing a token.");
        }
    }
}