using BuberDinner.Domain.Entities;

namespace BuberDinner.Application.Services.Authentication;

// disabling the.
public record AuthenticationResult
{
    // The init keyword defines an accessor method. It assigns a value only during the initial object construction.
    // This basically creates an immutable property.
    public User User { get; init; } = null!;
    public string Token { get; init; } = null!;
};

