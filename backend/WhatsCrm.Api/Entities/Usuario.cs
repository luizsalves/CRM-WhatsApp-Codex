namespace WhatsCrm.Api.Entities;

public sealed class Usuario : IEntidadeEmpresa
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmpresaId { get; set; }
    public required string Nome { get; set; }
    public required string Email { get; set; }
    public required string EmailNormalizado { get; set; }
    public required string PasswordHash { get; set; }
    public PerfilUsuario Perfil { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Empresa Empresa { get; set; } = null!;
}
