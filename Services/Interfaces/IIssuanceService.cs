using MSFEP.Models;

namespace MSFEP.Services;

public interface IIssuanceService
{
    Task<IssuanceResponse?> IssueProofOfEmployment();
    Task<IssuanceResponse?> IssueProjectAffiliation(ProjectAffiliationRequestCreate projectAffiliationRequest);
}