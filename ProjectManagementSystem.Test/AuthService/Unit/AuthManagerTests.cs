using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ProjectManagementSystem.API.AuthService.Managers;

namespace ProjectManagementSystem.Test.AuthService.Unit
{
    public class AuthManagerTests
    {
        private Mock<IAmazonCognitoClient> _mockClient;
        private Mock<ILogger<AuthManager>> _mockLogger;
        private AuthManager _authManager;

        [SetUp]
        public void Setup()
        {
            // Mock the IAmazonCognitoClient and ILogger<AuthManager>
            _mockClient = new Mock<IAmazonCognitoClient>();
            _mockLogger = new Mock<ILogger<AuthManager>>();

            // Inject the mocked dependencies into the AuthManager
            _authManager = new AuthManager(
                _mockClient.Object,
                "fakeClientId",
                _mockLogger.Object);
        }

        [Test]
        public async Task SignInAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var mockResponse = new InitiateAuthResponse
            {
                AuthenticationResult = new AuthenticationResultType
                {
                    IdToken = "mockToken"
                }
            };
            _mockClient
                .Setup(client => client.InitiateAuthAsync(It.IsAny<InitiateAuthRequest>(), CancellationToken.None))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _authManager.SignInAsync("testUser", "testPassword");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("mockToken", result);
        }

        [Test]
        public void SignInAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _mockClient
                .Setup(client => client.InitiateAuthAsync(It.IsAny<InitiateAuthRequest>(), CancellationToken.None))
                .ThrowsAsync(new NotAuthorizedException("Invalid username or password"));

            // Act & Assert
            Assert.ThrowsAsync<NotAuthorizedException>(async () => 
                await _authManager.SignInAsync("invalidUser", "wrongPassword"));
        }
    }
}
