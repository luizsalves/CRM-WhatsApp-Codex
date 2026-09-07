using System.ComponentModel.DataAnnotations;

namespace WhatsCrm.Api.Options;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(5, 1440)]
    public int ExpirationMinutes { get; init; } = 480;

    [Required, MinLength(32)]
    public string Secret { get; init; } = string.Empty;
}
