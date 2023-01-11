using System;
namespace BuberDinner.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SECTION_NAME = "JwtSettings";
    public string Secret { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int ExpiryMinutes { get; init; }
}

