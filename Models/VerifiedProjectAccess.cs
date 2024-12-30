namespace MSFEP.Models
{
    public class VerifiedProjectAccess
    {
        public required int Id;
        public required string UserPrincipalName;
        public required string ProjectName;
        public required string GitHubUsername;
        public required string Role;
    }
}
