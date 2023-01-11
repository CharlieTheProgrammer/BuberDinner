
namespace BuberDinner.Application.Services.Authentication;

// The Contract should not be referenced here because this is core logic.
// In fact, it's the other way around. The Contract references this logic.
// Something not explained yet is why
public interface IAuthenticationService
{
    // AuthenticationResponse Login(LoginRequest request);
    AuthenticationResult Register(string firstName, string lastName, string email, string password);
    AuthenticationResult Login(string email, string password);
}


