using Newtonsoft.Json;

namespace MSFEP.Components.Miscellaneous;

public class AuthHelper
{
    public static async Task<string> GetAccessToken()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://login.microsoftonline.com/f8655086-9b69-4877-8594-b8859330ee84/oauth2/v2.0/token");

        var content = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("client_id", "9e4497cb-c1dd-4e8a-bbaf-1cd131af939c"),
        new KeyValuePair<string, string>("scope", "3db474b9-6a0c-4840-96ac-1fceb342124f/.default"),
        new KeyValuePair<string, string>("client_secret", "zensiert"),
        new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        request.Content = content;

        var response = await client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            dynamic tokenResponse = JsonConvert.DeserializeObject(json);
            return tokenResponse.access_token;
        }

        Console.WriteLine("Error in token request: " + json);
        return string.Empty;
    }


}
