using WhatsCrm.Api.Entities;

namespace WhatsCrm.Api.Interfaces;

public interface IJwtTokenService
{
    TokenGerado Gerar(Usuario usuario);
}

public sealed record TokenGerado(string Valor, DateTimeOffset ExpiraEm);
