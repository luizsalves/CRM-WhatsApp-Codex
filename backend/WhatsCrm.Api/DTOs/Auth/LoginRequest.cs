using System.ComponentModel.DataAnnotations;

namespace WhatsCrm.Api.DTOs.Auth;

public sealed record LoginRequest(
    [property: Required, EmailAddress, StringLength(254)] string Email,
    [property: Required, MaxLength(128)] string Senha);
