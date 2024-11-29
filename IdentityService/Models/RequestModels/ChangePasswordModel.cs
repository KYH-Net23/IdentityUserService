namespace IdentityService.Models.RequestModels
{
    public class ChangePasswordModel
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
