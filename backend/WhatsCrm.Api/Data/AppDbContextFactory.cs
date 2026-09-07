using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WhatsCrm.Api.Interfaces;

namespace WhatsCrm.Api.Data;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? "Host=localhost;Port=5432;Database=whatscrm;Username=whatscrm";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options, new DesignTimeTenantContext());
    }

    private sealed class DesignTimeTenantContext : ITenantContext
    {
        public Guid? EmpresaId => null;
        public Guid? UsuarioId => null;
        public bool EstaAutenticado => false;
    }
}
