using MSFEP.Controllers;

namespace MSFEP.Models
{
    public class PresentationCallbackRequest
    {
        public string requestId { get; set; } = string.Empty;
        public string requestStatus { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public Receipt receipt { get; set; } = new();
        public List<VerifiedCredentialData> verifiedCredentialsData { get; set; } = [];
        public string subject { get; set; } = string.Empty;
    }
}
