using MSFEP.Controllers;

namespace MSFEP.Models
{
    public class VerifiedCredentialData
    {
        public string issuer { get; set; } = string.Empty;
        public List<string> type { get; set; } = [];
        public Dictionary<string, string> claims { get; set; } = [];
        public CredentialState credentialState { get; set; } = new();
        public DateTime expirationDate { get; set; } = DateTime.MinValue;
        public DateTime issuanceDate { get; set; } = DateTime.MinValue;
    }
}
