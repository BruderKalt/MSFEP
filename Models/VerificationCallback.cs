namespace MSFEP.Models;

public class VerificationCallback
{
    public string RequestStatus { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public CredentialInfo CredentialInfo { get; set; }
}
