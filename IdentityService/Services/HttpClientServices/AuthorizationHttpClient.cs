using System.Text.Json;
using IdentityService.Models.RequestModels;

namespace IdentityService.Services.HttpClientServices;

public class AuthorizationHttpClient
{
    private readonly HttpClient _httpClient;

    public AuthorizationHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> PostAsync(string url, UpdateEmailRequest model)
    {
        var body = JsonSerializer.Serialize(model);
        return await _httpClient.PostAsJsonAsync(url, body); // TODO
    }
}
