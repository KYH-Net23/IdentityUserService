using Newtonsoft.Json;

public partial class EmailProviderHttpClient
{
    private readonly HttpClient _httpClient;

    public EmailProviderHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> PostAsync(string url, ResetPasswordModel model)
    {
        var response = await _httpClient.PostAsJsonAsync(url, model);
        return response;
    }
}