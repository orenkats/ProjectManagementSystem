using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Logging;

namespace ProjectManagementSystem.API.AuthService.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly IAmazonCognitoClient _cognitoClient;
        private readonly string _clientId;
        private readonly ILogger<AuthManager> _logger;

        public AuthManager(IAmazonCognitoClient cognitoClient, string clientId, ILogger<AuthManager> logger)
        {
            _cognitoClient = cognitoClient;
            _clientId = clientId;
            _logger = logger;
        }

        public async Task<string> SignInAsync(string username, string password)
        {
            _logger.LogInformation($"Attempting to sign in user: {username}");

            var request = new InitiateAuthRequest
            {
                ClientId = _clientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", username },
                    { "PASSWORD", password }
                }
            };

            var response = await _cognitoClient.InitiateAuthAsync(request);

            if (response.AuthenticationResult?.IdToken != null)
            {
                _logger.LogInformation($"User {username} signed in successfully");
                return response.AuthenticationResult.IdToken;
            }

            _logger.LogWarning($"User {username} sign in failed");
            return null;
        }
    }
}