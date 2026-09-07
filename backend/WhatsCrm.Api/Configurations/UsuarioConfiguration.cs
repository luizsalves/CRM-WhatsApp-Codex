using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatsCrm.Api.Entities;

namespace WhatsCrm.Api.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(usuario => usuario.Id);

        builder.Property(usuario => usuario.Id).HasColumnName("id");
        builder.Property(usuario => usuario.EmpresaId).HasColumnName("empresa_id").IsRequired();
        builder.Property(usuario => usuario.Nome).HasColumnName("nome").HasMaxLength(160).IsRequired();
        builder.Property(usuario => usuario.Email).HasColumnName("email").HasMaxLength(254).IsRequired();
        builder.Property(usuario => usuario.EmailNormalizado).HasColumnName("email_normalizado").HasMaxLength(254).IsRequired();
        builder.Property(usuario => usuario.PasswordHash).HasColumnName("password_hash").HasMaxLength(512).IsRequired();
        builder.Property(usuario => usuario.Perfil)
            .HasColumnName("perfil")
            .HasConversion<string>()
            .HasMaxLength(24)
            .IsRequired();
        builder.Property(usuario => usuario.Ativo).HasColumnName("ativo").IsRequired();
        builder.Property(usuario => usuario.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(usuario => usuario.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(usuario => usuario.EmailNormalizado).IsUnique();
        builder.HasIndex(usuario => new { usuario.EmpresaId, usuario.Perfil });
        builder.HasOne(usuario => usuario.Empresa)
            .WithMany(empresa => empresa.Usuarios)
            .HasForeignKey(usuario => usuario.EmpresaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
