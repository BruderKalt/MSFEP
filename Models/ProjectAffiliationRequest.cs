namespace MSFEP.Models;

public class ProjectAffiliationRequest
{
    public required int Id;
    public required string ProjectName;
    public required string Name;
    public required string Role;
    public required string GitHubUsername;
    public required string UserPrincipalName;
    public required bool IsGranted;
}
