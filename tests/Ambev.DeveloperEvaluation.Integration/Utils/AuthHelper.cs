using Ambev.DeveloperEvaluation.ORM;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ambev.DeveloperEvaluation.Integration.Utils;

public static class AuthHelper
{
    public static string GenerateJwtToken(string key, string issuer, string audience, string userId)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        if (keyBytes.Length < 32)
            throw new ArgumentException("A chave precisa ter pelo menos 256 bits (32 caracteres).");

        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, "cliente@teste.com"),
            new Claim(ClaimTypes.Role, "Customer")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
