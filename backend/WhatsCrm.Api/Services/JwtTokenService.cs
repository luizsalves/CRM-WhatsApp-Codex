using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WhatsCrm.Api.Entities;
using WhatsCrm.Api.Interfaces;
using WhatsCrm.Api.Options;

namespace WhatsCrm.Api.Services;

public sealed class JwtTokenService(IOptions<JwtSettings> settings, TimeProvider timeProvider) : IJwtTokenService
{
    private readonly JwtSettings _settings = settings.Value;

    public TokenGerado Gerar(Usuario usuario)
    {
        var agora = timeProvider.GetUtcNow();
        var expiraEm = agora.AddMinutes(_settings.ExpirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, usuario.Nome),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim("role", usuario.Perfil.ToString().ToUpperInvariant()),
            new Claim("empresa_id", usuario.EmpresaId.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: agora.UtcDateTime,
            expires: expiraEm.UtcDateTime,
            signingCredentials: credentials);

        return new TokenGerado(new JwtSecurityTokenHandler().WriteToken(jwt), expiraEm);
    }
}
