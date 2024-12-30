using MSFEP.Models;

namespace MSFEP.DataAccess.Interfaces
{
    public interface IProjectAffiliationRequestDataAccess
    {
        Task<int> CreateAsync(ProjectAffiliationRequestCreate givenProjectAffiliationRequestCreate);
        Task<IEnumerable<ProjectAffiliationRequest>> GetByProjectNameAsync(string projectName);
        Task<int> GrantIssuanceAsync(int id);
        Task<bool> CheckGrantAsync(string userPrincipalName, string projectName, string name, string gitHubUsername, string role);
    }
}