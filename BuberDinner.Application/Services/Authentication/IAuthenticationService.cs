
using BuberDinner.Application.Errors;
using OneOf;

namespace BuberDinner.Application.Services.Authentication;

// The Contract should not be referenced here because this is core logic.
// In fact, it's the other way around. The Contract references this logic.
// Something not explained yet is why
public interface IAuthenticationService
{
    // AuthenticationResponse Login(LoginRequest request);
    OneOf<AuthenticationResult, DuplicateEmailError> Register(string firstName, string lastName, string email, string password);
    OneOf<AuthenticationResult, Exception> Login(string email, string password);
}


