using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Factory;

public static class TokenGeneratorFactory
{
    private const int DurationInMinutes = 5;

    public static string GenerateAccessToken(string secretKey)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, "IdentityProvider") };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(DurationInMinutes),
            Issuer = "https://www.rika.se",
            Audience = "https://www.rika.se",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        return new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityTokenHandler().CreateToken(tokenDescriptor)
        );
    }
}
