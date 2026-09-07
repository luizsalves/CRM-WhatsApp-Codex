using WhatsCrm.Api.Services;

namespace WhatsCrm.Api.Tests;

public sealed class PasswordPolicyTests
{
    [Fact]
    public void Validar_DeveAceitarSenhaForte()
    {
        var erros = PasswordPolicy.Validar("UmaSenhaForte123");

        Assert.Empty(erros);
    }

    [Theory]
    [InlineData("curta1A")]
    [InlineData("semsimbolomaiusculo123")]
    [InlineData("SEMMINUSCULA123")]
    [InlineData("SemNumeroAlgum")]
    public void Validar_DeveRejeitarSenhaForaDaPolitica(string senha)
    {
        var erros = PasswordPolicy.Validar(senha);

        Assert.NotEmpty(erros);
    }
}
