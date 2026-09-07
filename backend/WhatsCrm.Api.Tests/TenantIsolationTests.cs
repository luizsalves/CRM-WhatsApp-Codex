using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WhatsCrm.Api.Data;
using WhatsCrm.Api.Entities;
using WhatsCrm.Api.Interfaces;

namespace WhatsCrm.Api.Tests;

public sealed class TenantIsolationTests
{
    [Fact]
    public async Task QueryFilter_DeveRetornarSomenteUsuariosDaEmpresaAutenticada()
    {
        var empresaA = Guid.NewGuid();
        var empresaB = Guid.NewGuid();
        var databaseName = Guid.NewGuid().ToString();
        var databaseRoot = new InMemoryDatabaseRoot();

        await using (var seed = CriarContexto(databaseName, databaseRoot, null))
        {
            seed.Empresas.AddRange(
                NovaEmpresa(empresaA, "Empresa A"),
                NovaEmpresa(empresaB, "Empresa B"));
            seed.Usuarios.AddRange(
                NovoUsuario(empresaA, "a@teste.local"),
                NovoUsuario(empresaB, "b@teste.local"));
            await seed.SaveChangesAsync();
        }

        await using var contextoEmpresaA = CriarContexto(databaseName, databaseRoot, empresaA);
        var usuariosVisiveis = await contextoEmpresaA.Usuarios.AsNoTracking().ToListAsync();

        var usuario = Assert.Single(usuariosVisiveis);
        Assert.Equal(empresaA, usuario.EmpresaId);
    }

    [Fact]
    public async Task SaveChanges_DeveBloquearEntidadeDeOutraEmpresa()
    {
        var empresaAutenticada = Guid.NewGuid();
        var outraEmpresa = Guid.NewGuid();
        await using var context = CriarContexto(
            Guid.NewGuid().ToString(),
            new InMemoryDatabaseRoot(),
            empresaAutenticada);

        context.Usuarios.Add(NovoUsuario(outraEmpresa, "invasor@teste.local"));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => context.SaveChangesAsync());
    }

    private static AppDbContext CriarContexto(
        string databaseName,
        InMemoryDatabaseRoot root,
        Guid? empresaId)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName, root)
            .Options;
        return new AppDbContext(options, new TestTenantContext(empresaId));
    }

    private static Empresa NovaEmpresa(Guid id, string nome) => new()
    {
        Id = id,
        Nome = nome,
        TipoNegocio = TipoNegocio.Vendas
    };

    private static Usuario NovoUsuario(Guid empresaId, string email) => new()
    {
        EmpresaId = empresaId,
        Nome = "Usuário de teste",
        Email = email,
        EmailNormalizado = email.ToUpperInvariant(),
        PasswordHash = "hash-de-teste",
        Perfil = PerfilUsuario.Atendente
    };

    private sealed class TestTenantContext(Guid? empresaId) : ITenantContext
    {
        public Guid? EmpresaId { get; } = empresaId;
        public Guid? UsuarioId => EmpresaId.HasValue ? Guid.NewGuid() : null;
        public bool EstaAutenticado => EmpresaId.HasValue;
    }
}
