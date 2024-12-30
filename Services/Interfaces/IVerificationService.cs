using MSFEP.Models;

namespace MSFEP.Services;

public interface IVerificationService
{
    Task<VerificationResponse?> VerifyCredential(object CredentialRequestTemplate);
    Task<PresentationCallbackRequest?> PollForCallback(string state);
}