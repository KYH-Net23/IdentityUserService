using System.Text;
using IdentityService.Models.RequestModels;
using Newtonsoft.Json;

namespace IdentityService.Services.HttpClientServices;

public class VerificationHttpClient
{
    private readonly HttpClient _httpClient;

    public VerificationHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> PostAsync(EmailRequestModel emailRequestModel)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/passcode/generate");
        var jsonBody = JsonConvert.SerializeObject(emailRequestModel);

        request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.SendAsync(request);

        return response;
    }
}
