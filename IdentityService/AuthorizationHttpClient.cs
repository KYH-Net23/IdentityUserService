namespace IdentityService;

public class AuthorizationHttpClient
{
    private readonly HttpClient _httpClient;

    public AuthorizationHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> PostAsync(string url)
    {
        return await _httpClient.PostAsync(url, null);
    }
}
