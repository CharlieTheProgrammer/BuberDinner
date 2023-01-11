using System;
using BuberDinner.Application.Common.Interfaces.Authentication;

namespace BuberDinner.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator jwtTokenGenerator;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator)
    {
        this.jwtTokenGenerator = jwtTokenGenerator;
    }

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
        // Check if a user already exists

        // Create a user (generate a unique id)

        // Create a JWT Token
        Guid id = Guid.NewGuid();
        var token = jwtTokenGenerator.GenerateToken(id, firstName, lastName);
        
    return new AuthenticationResult(){
      Id = Guid.NewGuid(),
      FirstName = firstName,
      LastName = lastName,
      Email = "test@test.com",
      Token = token
    };

    //throw new NotImplementedException();
  }
}

