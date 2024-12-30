using MSFEP.Components.Miscellaneous;
using MSFEP.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MSFEP.Services;

public class VerificationService(HttpClient httpClient, ILogger<VerificationService> logger) : IVerificationService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<VerificationService> _logger = logger;

    public async Task<VerificationResponse?> VerifyCredential(object CredentialRequestTemplate)
    {
        var token = await AuthHelper.GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogInformation("Failed to acquire application access token.");
            return null;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jsonContent = new StringContent(JsonConvert.SerializeObject(CredentialRequestTemplate), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://verifiedid.did.msidentity.com/v1.0/verifiableCredentials/createPresentationRequest", jsonContent);

        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Error in verification request: {responseContent}");
            return null;
        }
        else
        {
            var verificationResponse = JsonConvert.DeserializeObject<VerificationResponse>(responseContent);
            return verificationResponse;
        }
    }

    public async Task<PresentationCallbackRequest?> PollForCallback(string state)
    {
        string verificationStatus = "waiting";

        while (verificationStatus == "waiting" || verificationStatus == "request_retrieved")
        {
            await Task.Delay(1000);

            var response = await _httpClient.GetAsync($"https://7400-2003-c5-720-ffa7-b1c4-a29a-a053-5e36.ngrok-free.app/verification/request-status?state={state}");

            if (response.IsSuccessStatusCode)
            {
                var callback = await response.Content.ReadFromJsonAsync<PresentationCallbackRequest>();
                return callback;
            }
            else
            {
                verificationStatus = "waiting";
            }
        }

        return null;
    }
}