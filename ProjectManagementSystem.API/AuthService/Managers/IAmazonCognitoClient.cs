using Amazon.CognitoIdentityProvider.Model;

namespace ProjectManagementSystem.API.AuthService.Managers;

public interface IAmazonCognitoClient
{
    Task<InitiateAuthResponse> InitiateAuthAsync(InitiateAuthRequest request, CancellationToken cancellationToken = default);
}