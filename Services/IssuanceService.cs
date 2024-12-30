using MSFEP.Components.Miscellaneous;
using MSFEP.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MSFEP.Services;

public class IssuanceService : IIssuanceService
{
    private readonly HttpClient _httpClient;

    public IssuanceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IssuanceResponse?> IssueProofOfEmployment()
    {
        var generatedPin = Random.Shared.Next(100000, 999999);
        var token = await AuthHelper.GetAccessToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var issueRequestProofOfEmployment = new
        {
            includeQRCode = true,
            callback = new
            {
                url = "https://7400-2003-c5-720-ffa7-b1c4-a29a-a053-5e36.ngrok-free.app/api/issuer/issuanceCallback",
                state = "de19cb6b-36c1-45fe-9409-909a51292a9c"
            },
            authority = "did:web:mustersoftwarefabrik.com",
            registration = new
            {
                clientName = "MSFEP"
            },
            type = "VerifiedEmployee",
            manifest = "https://verifiedid.did.msidentity.com/v1.0/tenants/f8655086-9b69-4877-8594-b8859330ee84/verifiableCredentials/contracts/ae222694-8977-c7f0-c8c3-06f972896e1a/manifest"

        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(issueRequestProofOfEmployment), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://verifiedid.did.msidentity.com/v1.0/verifiableCredentials/createIssuanceRequest", jsonContent);

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            string error = $"Error in issuance request: {responseContent}";
            Console.WriteLine(error);
        }
        else
        {
            var issuanceResponse = JsonConvert.DeserializeObject<IssuanceResponse>(responseContent);
            issuanceResponse.pin = generatedPin;
            return issuanceResponse;
        }

        return null;
    }

    public async Task<IssuanceResponse?> IssueProjectAffiliation(ProjectAffiliationRequestCreate projectAffiliationRequest)
    {
        var generatedPin = Random.Shared.Next(100000, 999999);
        var token = await AuthHelper.GetAccessToken();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var issueProjectAffiliationRequest = new
        {
            includeQRCode = true,
            callback = new
            {
                url = "https://7400-2003-c5-720-ffa7-b1c4-a29a-a053-5e36.ngrok-free.app/api/issuer/issuanceCallback",
                state = "de19cb6b-36c1-45fe-9409-909a51292a9c"
            },
            authority = "did:web:mustersoftwarefabrik.com",
            registration = new
            {
                clientName = "MSFEP"
            },
            type = "MSFProjectAffiliation, MSFCostCenter",
            manifest = "https://verifiedid.did.msidentity.com/v1.0/tenants/f8655086-9b69-4877-8594-b8859330ee84/verifiableCredentials/contracts/0e3a008b-90cc-2a72-6eef-1e4692ab825b/manifest",
            pin = new
            {
                value = generatedPin.ToString(),
                length = 6
            },
            claims = new
            {
                projectName = projectAffiliationRequest.ProjectName,
                name = projectAffiliationRequest.Name,
                role = projectAffiliationRequest.Role,
                githubusername = projectAffiliationRequest.GitHubUsername,
                userprincipalname = projectAffiliationRequest.UserPrincipalName,
            }
        };


        var jsonContent = new StringContent(JsonConvert.SerializeObject(issueProjectAffiliationRequest), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://verifiedid.did.msidentity.com/v1.0/verifiableCredentials/createIssuanceRequest", jsonContent);

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            string error = $"Error in issuance request: {responseContent}";
            Console.WriteLine(error);
        }
        else
        {
            var issuanceResponse = JsonConvert.DeserializeObject<IssuanceResponse>(responseContent);
            issuanceResponse.pin = generatedPin;
            return issuanceResponse;
        }

        return null;
    }
}
