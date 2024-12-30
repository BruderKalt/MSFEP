using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MSFEP.Models;
using MSFEP.Services.Interfaces;
using Octokit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace MSFEP.Services;

public class GitHubAppService : IGitHubAppService
{
    private readonly GitHubAppConfig _config;
    private readonly ILogger<GitHubAppService> _logger;
    private readonly GitHubClient _client;
    private RSA _rsa;

    public GitHubAppService(IOptions<GitHubAppConfig> config, ILogger<GitHubAppService> logger)
    {
        _config = config.Value;
        _logger = logger;

        var jwtToken = CreateJwtToken();
        _client = new GitHubClient(new ProductHeaderValue("MSFEP"))
        {
            Credentials = new Credentials(jwtToken, AuthenticationType.Bearer)
        };
    }

    public async Task<IReadOnlyList<Repository>> GetOrganizationRepositoriesAsync(string orgName)
    {
        try
        {
            var installationToken = await GetInstallationAccessToken();
            var installationClient = new GitHubClient(new ProductHeaderValue("MSFEP"))
            {
                Credentials = new Credentials(installationToken)
            };

            var x = await installationClient.Repository.GetAllForOrg(orgName);
            return x;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to fetch organization repositories: {ex.Message}");
            throw;
        }
    }

    public async Task GrantRepositoryAccess(string organization, string repositoryName, string username, string permission)
    {
        try
        {
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Username is null or empty.");
                return;
            }

            var installationToken = await GetInstallationAccessToken();
            var installationClient = new GitHubClient(new ProductHeaderValue("MSFEP"))
            {
                Credentials = new Credentials(installationToken)
            };

            var options = new CollaboratorRequest(permission);
            await installationClient.Repository.Collaborator.Add(organization, repositoryName, username, options);

            Console.WriteLine($"Successfully granted {permission} access to {repositoryName} for {username}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to grant access: {ex.Message}");
        }
    }

    public async Task RemoveRepositoryAccess(string organization, string repositoryName, string username)
    {
        try
        {
            var installationToken = await GetInstallationAccessToken();
            var installationClient = new GitHubClient(new ProductHeaderValue("MSFEP"))
            {
                Credentials = new Credentials(installationToken)
            };

            await installationClient.Repository.Collaborator.Delete(organization, repositoryName, username);
            Console.WriteLine($"Successfully removed {username} from {repositoryName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to remove access: {ex.Message}");
        }
    }

    private async Task<string> GetInstallationAccessToken()
    {
        var installationToken = await _client.GitHubApps.CreateInstallationToken(Convert.ToInt32(_config.InstallationId));
        return installationToken.Token;
    }

    private string CreateJwtToken()
    {
        var now = DateTimeOffset.UtcNow;

        var exp = now.AddMinutes(10).ToUnixTimeSeconds();
        var iat = now.ToUnixTimeSeconds();
        var iss = _config.AppId;

        var claims = new Dictionary<string, object>
        {
            { "exp", exp },
            { "iss", iss },
            { "iat", iat },
        };

        var privateKeyBytes = Convert.FromBase64String(_config.PrivateKey);
        _rsa = RSA.Create();
        _rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(_rsa), SecurityAlgorithms.RsaSha256);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Expires = now.AddMinutes(1).UtcDateTime,
            SigningCredentials = signingCredentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return jwtToken;
    }
}
