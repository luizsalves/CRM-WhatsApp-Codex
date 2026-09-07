using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using WhatsCrm.Api.Data;
using WhatsCrm.Api.DTOs.Auth;
using WhatsCrm.Api.Entities;
using WhatsCrm.Api.Interfaces;
using WhatsCrm.Api.Middlewares;

namespace WhatsCrm.Api.Services;

public sealed class AuthService(
    AppDbContext dbContext,
    IPasswordHasher<Usuario> passwordHasher,
    IJwtTokenService jwtTokenService,
    ITenantContext tenantContext) : IAuthService
{
    public async Task<BootstrapStatusResponse> ObterBootstrapStatusAsync(CancellationToken cancellationToken)
    {
        var possuiUsuario = await dbContext.Usuarios
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(cancellationToken);

        return new BootstrapStatusResponse(!possuiUsuario);
    }

    public async Task<AuthResponse> CriarPrimeiroAdministradorAsync(
        BootstrapRequest request,
        CancellationToken cancellationToken)
    {
        var errosSenha = PasswordPolicy.Validar(request.Senha);
        if (errosSenha.Count > 0)
        {
            throw new RegraNegocioException(string.Join(" ", errosSenha));
        }

        IDbContextTransaction? transaction = null;
        if (dbContext.Database.IsRelational())
        {
            transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        }

        await using (transaction)
        {
            if (await dbContext.Usuarios.IgnoreQueryFilters().AnyAsync(cancellationToken))
            {
                throw new RegraNegocioException(
                    "A configuração inicial já foi concluída.",
                    StatusCodes.Status409Conflict);
            }

            var empresa = new Empresa
            {
                Nome = request.EmpresaNome.Trim(),
                NomeFantasia = request.EmpresaNome.Trim(),
                TipoNegocio = request.TipoNegocio
            };

            var usuario = new Usuario
            {
                EmpresaId = empresa.Id,
                Nome = request.Nome.Trim(),
                Email = request.Email.Trim(),
                EmailNormalizado = NormalizarEmail(request.Email),
                PasswordHash = string.Empty,
                Perfil = PerfilUsuario.Administrador,
                Empresa = empresa
            };
            usuario.PasswordHash = passwordHasher.HashPassword(usuario, request.Senha);

            dbContext.Empresas.Add(empresa);
            dbContext.Usuarios.Add(usuario);

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                if (transaction is not null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }
            }
            catch (Exception exception) when (exception is DbUpdateException or PostgresException)
            {
                throw new RegraNegocioException(
                    "Não foi possível concluir a configuração inicial. Verifique se ela já foi realizada.",
                    StatusCodes.Status409Conflict);
            }

            return CriarResposta(usuario);
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var emailNormalizado = NormalizarEmail(request.Email);
        var usuario = await dbContext.Usuarios
            .IgnoreQueryFilters()
            .Include(item => item.Empresa)
            .SingleOrDefaultAsync(item => item.EmailNormalizado == emailNormalizado, cancellationToken);

        if (usuario is null || !usuario.Ativo || !usuario.Empresa.Ativo)
        {
            throw CredenciaisInvalidas();
        }

        var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Senha);
        if (resultado == PasswordVerificationResult.Failed)
        {
            throw CredenciaisInvalidas();
        }

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.PasswordHash = passwordHasher.HashPassword(usuario, request.Senha);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return CriarResposta(usuario);
    }

    public async Task<UsuarioResponse> ObterUsuarioAtualAsync(CancellationToken cancellationToken)
    {
        if (!tenantContext.EstaAutenticado || !tenantContext.UsuarioId.HasValue)
        {
            throw new UnauthorizedAccessException();
        }

        var usuario = await dbContext.Usuarios
            .AsNoTracking()
            .Include(item => item.Empresa)
            .SingleOrDefaultAsync(item => item.Id == tenantContext.UsuarioId.Value, cancellationToken);

        if (usuario is null || !usuario.Ativo || !usuario.Empresa.Ativo)
        {
            throw new UnauthorizedAccessException();
        }

        return MapearUsuario(usuario);
    }

    private AuthResponse CriarResposta(Usuario usuario)
    {
        var token = jwtTokenService.Gerar(usuario);
        return new AuthResponse(token.Valor, token.ExpiraEm, MapearUsuario(usuario));
    }

    private static UsuarioResponse MapearUsuario(Usuario usuario) => new(
        usuario.Id,
        usuario.EmpresaId,
        usuario.Empresa.Nome,
        usuario.Empresa.TipoNegocio,
        usuario.Nome,
        usuario.Email,
        usuario.Perfil);

    private static string NormalizarEmail(string email) => email.Trim().ToUpperInvariant();

    private static RegraNegocioException CredenciaisInvalidas() => new(
        "E-mail ou senha inválidos.",
        StatusCodes.Status401Unauthorized);
}
