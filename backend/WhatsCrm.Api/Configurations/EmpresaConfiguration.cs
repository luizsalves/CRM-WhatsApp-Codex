using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsCrm.Api.Entities;

namespace WhatsCrm.Api.Configurations;

public sealed class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.ToTable("empresas");
        builder.HasKey(empresa => empresa.Id);

        builder.Property(empresa => empresa.Id).HasColumnName("id");
        builder.Property(empresa => empresa.Nome).HasColumnName("nome").HasMaxLength(160).IsRequired();
        builder.Property(empresa => empresa.NomeFantasia).HasColumnName("nome_fantasia").HasMaxLength(160);
        builder.Property(empresa => empresa.Cnpj).HasColumnName("cnpj").HasMaxLength(14);
        builder.Property(empresa => empresa.TipoNegocio)
            .HasColumnName("tipo_negocio")
            .HasConversion<string>()
            .HasMaxLength(24)
            .IsRequired();
        builder.Property(empresa => empresa.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(empresa => empresa.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(empresa => empresa.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(empresa => empresa.Cnpj).IsUnique().HasFilter("cnpj IS NOT NULL");
    }
}
