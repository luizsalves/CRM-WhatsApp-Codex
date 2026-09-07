using WhatsCrm.Api.DTOs.Auth;

namespace WhatsCrm.Api.Interfaces;

public interface IAuthService
{
    Task<BootstrapStatusResponse> ObterBootstrapStatusAsync(CancellationToken cancellationToken);
    Task<AuthResponse> CriarPrimeiroAdministradorAsync(BootstrapRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<UsuarioResponse> ObterUsuarioAtualAsync(CancellationToken cancellationToken);
}
