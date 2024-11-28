// using System.Net.Http.Headers;
// using System.Text;
// using IdentityService.Factories;
// using IdentityService.Models.FormModels;
// using Microsoft.AspNetCore.Identity;
// using Newtonsoft.Json;
//
// namespace IdentityService.Services;
//
// public class AzureEmailSender(
//     HttpClient httpClient,
//     IConfiguration config,
//     UserManager<IdentityUser> userManager
// )
// {
//     private readonly string _secret = config["EmailProviderSecretKey"]!;
//
//     public async Task SendConfirmationLinkAsync(IdentityUser user)
//     {
//         var confirmationLink = await userManager.GenerateEmailConfirmationTokenAsync(user);
//         var accessToken = TokenGeneratorFactory.GenerateAccessToken(_secret);
//
//         httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
//             "Bearer",
//             accessToken
//         );
//
//         var emailContent = new StringContent(
//             JsonConvert.SerializeObject(new { email = user.Email, confirmationLink }),
//             Encoding.UTF8,
//             "application/json"
//         );
//
//         var response = await httpClient.PostAsync("/IdentityEmail", emailContent);
//
//         response.EnsureSuccessStatusCode();
//     }
//
//     public async Task TestMethod(EmailRequestModel email)
//     {
//         var accessToken = TokenGeneratorFactory.GenerateAccessToken(_secret);
//
//         httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
//             "Bearer",
//             accessToken
//         );
//
//         var emailContent = new EmailRequestModel { Receiver = email.Receiver, Uri = email.Uri, };
//
//         var content = new StringContent(
//             JsonConvert.SerializeObject(emailContent),
//             Encoding.UTF8,
//             "application/json"
//         );
//
//         using var response = await httpClient.PostAsync("/IdentityEmail", content);
//         response.EnsureSuccessStatusCode();
//     }
//
//     public async Task ResetEmail(EmailRequestModel email)
//     {
//         var accessToken = TokenGeneratorFactory.GenerateAccessToken(_secret);
//
//         httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
//             "Bearer",
//             accessToken
//         );
//
//         var emailContent = new EmailRequestModel { Receiver = email.Receiver, Uri = email.Uri, };
//
//         var content = new StringContent(
//             JsonConvert.SerializeObject(emailContent),
//             Encoding.UTF8,
//             "application/json"
//         );
//
//         using var response = await httpClient.PostAsync("/IdentityEmail", content);
//         response.EnsureSuccessStatusCode();
//     }
// }
