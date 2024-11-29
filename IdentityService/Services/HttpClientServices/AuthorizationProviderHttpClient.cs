using System.Text.Json;
using IdentityService.Models.RequestModels;

namespace IdentityService.Services.HttpClientServices;

public class AuthorizationProviderHttpClient
{
    private readonly HttpClient _httpClient;

    public AuthorizationProviderHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> PostAsync(
        string url,
        AuthorizationForEmailProviderRequestModel model
    )
    {
        var body = JsonSerializer.Serialize(model);
        return await _httpClient.PostAsJsonAsync(url, body); // TODO
    }
}
