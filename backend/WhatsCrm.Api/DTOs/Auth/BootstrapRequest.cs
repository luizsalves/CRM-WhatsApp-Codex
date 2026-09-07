using System.ComponentModel.DataAnnotations;
using WhatsCrm.Api.Entities;

namespace WhatsCrm.Api.DTOs.Auth;

public sealed record BootstrapRequest(
    [property: Required, StringLength(160, MinimumLength = 2)] string EmpresaNome,
    [property: Required, StringLength(160, MinimumLength = 2)] string Nome,
    [property: Required, EmailAddress, StringLength(254)] string Email,
    [property: Required, MinLength(12), MaxLength(128)] string Senha,
    TipoNegocio TipoNegocio);
