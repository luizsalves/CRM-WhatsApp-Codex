using WhatsCrm.Api.Entities;

namespace WhatsCrm.Api.DTOs.Auth;

public sealed record UsuarioResponse(
    Guid Id,
    Guid EmpresaId,
    string EmpresaNome,
    TipoNegocio TipoNegocio,
    string Nome,
    string Email,
    PerfilUsuario Perfil);

public sealed record AuthResponse(
    string Token,
    DateTimeOffset ExpiraEm,
    UsuarioResponse Usuario);

public sealed record BootstrapStatusResponse(bool Disponivel);
