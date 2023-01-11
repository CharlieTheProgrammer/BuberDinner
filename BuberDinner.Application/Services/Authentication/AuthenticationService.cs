using System;
namespace BuberDinner.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
  public AuthenticationResult Login(string email, string password)
  {
        
        return new AuthenticationResult()
        {
            Id = Guid.NewGuid(),
            FirstName = "First",
            LastName = "Last",
            Email = "test@test.com",
            Token = "token"
        };

        //throw new NotImplementedException();
    }

  public AuthenticationResult Register(string firstName, string lastName, string email, string password)
  {
        
    return new AuthenticationResult(){
      Id = Guid.NewGuid(),
      FirstName = "First",
      LastName = "Last",
      Email = "test@test.com",
      Token = "token"
    };

    //throw new NotImplementedException();
  }
}

