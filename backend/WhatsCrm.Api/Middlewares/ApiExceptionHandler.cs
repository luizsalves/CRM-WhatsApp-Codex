using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WhatsCrm.Api.Middlewares;

public sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            RegraNegocioException regra => (regra.StatusCode, "Regra de negócio não atendida", regra.Message),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Acesso negado", "Você não tem permissão para executar esta operação."),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno", "Não foi possível concluir a operação.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Erro não tratado na requisição {TraceIdentifier}", httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path,
                Extensions = { ["traceId"] = httpContext.TraceIdentifier }
            }
        });
    }
}
