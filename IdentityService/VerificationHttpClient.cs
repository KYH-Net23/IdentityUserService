using IdentityService.Models.FormModels;

namespace IdentityService;

public class VerificationHttpClient
{
    private readonly HttpClient _httpClient;

    public VerificationHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> PostAsync(EmailRequestModel emailRequestModel)
    {
        var response = await _httpClient.PostAsJsonAsync("passcode/generate", emailRequestModel);

        return response;
    }
}
