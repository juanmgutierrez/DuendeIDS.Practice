using IdentityModel.Client;
using Microsoft.Extensions.Options;
using WeatherMVC.Configurations;

namespace WeatherMVC.Services;

public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly IdentityServerSettings _identityServerSettings;
    private readonly DiscoveryDocumentResponse _discoveryDocumentResponse;

    public TokenService(
        ILogger<TokenService> logger,
        IOptions<IdentityServerSettings> identityServerSettings)
    {
        _logger = logger;
        _identityServerSettings = identityServerSettings.Value;

        using var client = new HttpClient();
        _discoveryDocumentResponse = client.GetDiscoveryDocumentAsync(identityServerSettings.Value.DiscoveryUrl).Result;

        if (_discoveryDocumentResponse.IsError)
        {
            _logger.LogCritical($"Couldn't get the discovery document. Message: {_discoveryDocumentResponse.Error}");
            throw new Exception("Couldn't get the discovery document", _discoveryDocumentResponse.Exception);
        }
    }

    public async Task<TokenResponse> GetToken(string scope, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        var tokenResponse = await client.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocumentResponse.TokenEndpoint,
                ClientId = _identityServerSettings.ClientId,
                ClientSecret = _identityServerSettings.ClientSecret,
                Scope = scope
            },
            cancellationToken);

        if (tokenResponse.IsError)
        {
            _logger.LogError($"Unable to get token with scope: {scope}. Message: {tokenResponse.Error}");
            throw new Exception("Unable to get token with scope: {scope}", tokenResponse.Exception);
        }

        return tokenResponse;
    }
}
