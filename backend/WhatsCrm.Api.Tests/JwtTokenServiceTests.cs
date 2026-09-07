using System.IdentityModel.Tokens.Jwt;
using WhatsCrm.Api.Entities;
using WhatsCrm.Api.Options;
using WhatsCrm.Api.Services;

namespace WhatsCrm.Api.Tests;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void Gerar_DeveIncluirUsuarioEmpresaERoleEmClaimsPadrao()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            EmpresaId = Guid.NewGuid(),
            Nome = "Administrador",
            Email = "admin@teste.local",
            EmailNormalizado = "ADMIN@TESTE.LOCAL",
            PasswordHash = "hash",
            Perfil = PerfilUsuario.Administrador,
            Empresa = new Empresa { Nome = "Teste", TipoNegocio = TipoNegocio.Vendas }
        };
        var settings = Microsoft.Extensions.Options.Options.Create(new JwtSettings
        {
            Issuer = "WhatsCRM.Tests",
            Audience = "WhatsCRM.Tests.Web",
            ExpirationMinutes = 60,
            Secret = "uma-chave-de-testes-com-mais-de-32-caracteres"
        });
        var service = new JwtTokenService(settings, TimeProvider.System);

        var token = service.Gerar(usuario);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.Valor);

        Assert.Equal(usuario.Id.ToString(), jwt.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal(usuario.EmpresaId.ToString(), jwt.Claims.Single(claim => claim.Type == "empresa_id").Value);
        Assert.Equal("ADMINISTRADOR", jwt.Claims.Single(claim => claim.Type == "role").Value);
    }
}
