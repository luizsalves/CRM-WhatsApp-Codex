using Microsoft.EntityFrameworkCore;
using WhatsCrm.Api.Entities;
using WhatsCrm.Api.Interfaces;

namespace WhatsCrm.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
    : DbContext(options)
{
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Empresa>()
            .HasQueryFilter(empresa => tenantContext.EmpresaId.HasValue && empresa.Id == tenantContext.EmpresaId.Value);
        modelBuilder.Entity<Usuario>()
            .HasQueryFilter(usuario => tenantContext.EmpresaId.HasValue && usuario.EmpresaId == tenantContext.EmpresaId.Value);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AplicarAuditoriaEIsolamento();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        AplicarAuditoriaEIsolamento();
        return base.SaveChanges();
    }

    private void AplicarAuditoriaEIsolamento()
    {
        var agora = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Empresa empresa)
            {
                if (entry.State == EntityState.Added)
                {
                    empresa.CreatedAt = agora;
                }

                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    empresa.UpdatedAt = agora;
                }
            }

            if (entry.Entity is not IEntidadeEmpresa entidadeEmpresa)
            {
                continue;
            }

            if (entry.State == EntityState.Added && entidadeEmpresa.EmpresaId == Guid.Empty && tenantContext.EmpresaId.HasValue)
            {
                entidadeEmpresa.EmpresaId = tenantContext.EmpresaId.Value;
            }

            if (entidadeEmpresa.EmpresaId == Guid.Empty)
            {
                throw new InvalidOperationException("EmpresaId é obrigatório para entidades multiempresa.");
            }

            if (tenantContext.EmpresaId.HasValue && entidadeEmpresa.EmpresaId != tenantContext.EmpresaId.Value)
            {
                throw new UnauthorizedAccessException("A entidade não pertence à empresa autenticada.");
            }

            if (entry.State == EntityState.Modified && entry.Property(nameof(IEntidadeEmpresa.EmpresaId)).IsModified)
            {
                throw new InvalidOperationException("EmpresaId não pode ser alterado.");
            }

            if (entry.Entity is Usuario usuario)
            {
                if (entry.State == EntityState.Added)
                {
                    usuario.CreatedAt = agora;
                }

                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    usuario.UpdatedAt = agora;
                }
            }
        }
    }
}
