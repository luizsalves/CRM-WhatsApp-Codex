namespace WhatsCrm.Api.Services;

public static class PasswordPolicy
{
    public static IReadOnlyList<string> Validar(string senha)
    {
        var erros = new List<string>();

        if (senha.Length < 12)
        {
            erros.Add("A senha deve ter pelo menos 12 caracteres.");
        }

        if (!senha.Any(char.IsUpper))
        {
            erros.Add("A senha deve conter uma letra maiúscula.");
        }

        if (!senha.Any(char.IsLower))
        {
            erros.Add("A senha deve conter uma letra minúscula.");
        }

        if (!senha.Any(char.IsDigit))
        {
            erros.Add("A senha deve conter um número.");
        }

        return erros;
    }
}
