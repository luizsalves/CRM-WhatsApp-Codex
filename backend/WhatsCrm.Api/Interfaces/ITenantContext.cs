namespace WhatsCrm.Api.Interfaces;

public interface ITenantContext
{
    Guid? EmpresaId { get; }
    Guid? UsuarioId { get; }
    bool EstaAutenticado { get; }
}
