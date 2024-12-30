namespace MSFEP.Models
{
    public class VerifiedProjectAccessCreate
    {
        public required string UserPrincipalName;
        public required string ProjectName;
        public required string GitHubUsername;
        public required string Role;
    }
}
