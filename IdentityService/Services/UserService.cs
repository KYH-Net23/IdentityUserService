using IdentityService.Models.FormModels;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services
{
	public class UserService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
	{
		public async Task<ResponseResult> Login(LoginModel loginModel)
		{
			try
			{
				var loggedInUser = await userManager.FindByEmailAsync(loginModel.Email);

				if (loggedInUser == null)
				{
					return new ResponseResult
					{
						Succeeded = false,
						Message = "User not found",
						Content = "User not found"
					};
				}

				var tryToSignIn =
					await signInManager.PasswordSignInAsync(loggedInUser.UserName!, loginModel.Password, false, false);

				if (!tryToSignIn.Succeeded)
				{
					var counter = await userManager.GetAccessFailedCountAsync(loggedInUser);
					await userManager.AccessFailedAsync(loggedInUser);
					if (!await userManager.IsLockedOutAsync(loggedInUser))
						return new ResponseResult
						{
							Succeeded = false,
							Message = "Login failed",
							Content = "Login failed"
						};
					var lockoutEndDate = await userManager.GetLockoutEndDateAsync(loggedInUser);
					return new ResponseResult
					{
						Succeeded = false,
						Message = $"Too many attempts. Account temporarily locked until {lockoutEndDate}",
						Content = $"Too many attempts. Account temporarily locked until {lockoutEndDate}"
					};

				}

				var userRole = await userManager.GetRolesAsync(loggedInUser!);

				return new ResponseResult
				{
					Succeeded = true,
					Message = "Login successful",
					Content = new
					{
						loggedInUser.Id,
						loggedInUser.Email,
						Roles = userRole
					}
				};
			}
			catch (Exception e)
			{
				return new ResponseResult
				{
					Succeeded = false,
					Message = e.Message,
					Content = e.Message
				};
			}
		}
	}
}
