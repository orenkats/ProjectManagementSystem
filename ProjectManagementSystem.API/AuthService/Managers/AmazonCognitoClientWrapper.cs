using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

namespace ProjectManagementSystem.API.AuthService.Managers;

public class AmazonCognitoClientWrapper : IAmazonCognitoClient
{
    private readonly AmazonCognitoIdentityProviderClient _client;

    public AmazonCognitoClientWrapper(string awsAccessKeyId, string awsSecretAccessKey, string region)
    {
        _client = new AmazonCognitoIdentityProviderClient(
            awsAccessKeyId, 
            awsSecretAccessKey, 
            Amazon.RegionEndpoint.GetBySystemName(region));
    }

    public Task<InitiateAuthResponse> InitiateAuthAsync(InitiateAuthRequest request, CancellationToken cancellationToken = default)
    {
        return _client.InitiateAuthAsync(request, cancellationToken);
    }
}