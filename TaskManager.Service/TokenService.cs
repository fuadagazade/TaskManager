using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Core.Configs;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.Service;

public class TokenService : ITokenService
{
    private readonly TokenConfig _config;

    public TokenService(IOptions<TokenConfig> config)
    {
        this._config = config.Value;
    }
    public string Create(User user)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key));
        SigningCredentials signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            claims: GenerateClaims(ref user),
            expires: DateTime.Now.AddDays(_config.Expires),
            signingCredentials: signInCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private List<Claim> GenerateClaims(ref User user)
    {
        List<Claim> claims = new List<Claim>();

        claims.Add(new Claim("Id", (user.Id ?? 0).ToString()));
        claims.Add(new Claim("FirstName", user.FirstName));
        claims.Add(new Claim("LastName", user.LastName));
        claims.Add(new Claim("Email", user.Email));
        claims.Add(new Claim("Organization", user.OrganizationId.ToString()));
        claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));

        return claims;
    }
}
