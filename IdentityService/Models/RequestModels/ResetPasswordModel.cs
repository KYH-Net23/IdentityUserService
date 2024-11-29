namespace IdentityService.Models.RequestModels;

public class ResetPasswordModel
{
    public string Receiver { get; set; } = null!;
    public string ResetGuid { get; set; } = null!;
}
