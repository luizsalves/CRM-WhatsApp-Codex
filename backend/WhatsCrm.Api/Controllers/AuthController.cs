using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WhatsCrm.Api.DTOs.Auth;
using WhatsCrm.Api.Interfaces;

namespace WhatsCrm.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpGet("bootstrap-status")]
    [AllowAnonymous]
    public async Task<ActionResult<BootstrapStatusResponse>> ObterBootstrapStatus(CancellationToken cancellationToken)
        => Ok(await authService.ObterBootstrapStatusAsync(cancellationToken));

    [HttpPost("bootstrap")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponse>> CriarPrimeiroAdministrador(
        BootstrapRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.CriarPrimeiroAdministradorAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
        => Ok(await authService.LoginAsync(request, cancellationToken));

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UsuarioResponse>> Me(CancellationToken cancellationToken)
        => Ok(await authService.ObterUsuarioAtualAsync(cancellationToken));
}
