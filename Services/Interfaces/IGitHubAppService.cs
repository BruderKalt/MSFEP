using Octokit;

namespace MSFEP.Services.Interfaces;

public interface IGitHubAppService
{
    Task<IReadOnlyList<Repository>> GetOrganizationRepositoriesAsync(string orgName);
    Task GrantRepositoryAccess(string organization, string repositoryName, string username, string permission);
    Task RemoveRepositoryAccess(string organization, string repositoryName, string username);
}