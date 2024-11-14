using System.Net.Http.Headers;
using System.Text;
using Azure.Communication.Email;
using IdentityService.Factory;
using IdentityService.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;

namespace IdentityService.Services;

public class AzureEmailSender(
    HttpClient httpClient,
    IConfiguration config,
    UserManager<IdentityUser> userManager
) : IEmailSender
{
    private readonly string _secret = config["EmailProviderSecretKey"]!;

    public async Task Execute(ResponseResult response)
    {
        var user = response.Content as IdentityUser;
        var subject = "";
        var body = "";
        await SendEmailAsync(user.Email, subject, body);
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var user = userManager.FindByEmailAsync(email).Result;
        var confirmationLink = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailMessage = new EmailMessage(
            "",
            new EmailRecipients(),
            new EmailContent(confirmationLink)
        );

        var accessToken = TokenGeneratorFactory.GenerateAccessToken(_secret);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );

        var emailContent = new StringContent(
            JsonConvert.SerializeObject(emailMessage),
            Encoding.UTF8,
            "application/json"
        );

        var response = await httpClient.PostAsync("api/Email/SendConfirmationEmail", emailContent);

        response.EnsureSuccessStatusCode();
    }
}
