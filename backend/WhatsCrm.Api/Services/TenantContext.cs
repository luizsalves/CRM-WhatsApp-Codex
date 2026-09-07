using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WhatsCrm.Api.Interfaces;

namespace WhatsCrm.Api.Services;

public sealed class TenantContext(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? EmpresaId => ObterGuidClaim("empresa_id");
    public Guid? UsuarioId => ObterGuidClaim(JwtRegisteredClaimNames.Sub);
    public bool EstaAutenticado => User?.Identity?.IsAuthenticated == true && EmpresaId.HasValue && UsuarioId.HasValue;

    private Guid? ObterGuidClaim(string claimType)
    {
        var value = User?.FindFirstValue(claimType);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
