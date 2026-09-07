namespace WhatsCrm.Api.Middlewares;

public sealed class RegraNegocioException(string message, int statusCode = StatusCodes.Status400BadRequest)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
