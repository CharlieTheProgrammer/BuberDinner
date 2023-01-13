using System;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Application.Errors;
using BuberDinner.Domain.Entities;
using OneOf;

namespace BuberDinner.Application.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator jwtTokenGenerator;
    private readonly IUserRepository userRepository;

    public AuthenticationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        this.jwtTokenGenerator = jwtTokenGenerator;
        this.userRepository = userRepository;
    }

    public OneOf<AuthenticationResult, Exception> Login(string email, string password)
    {
        // Check that the user exists
        var user = userRepository.GetUserByEmail(email);

        if (user is null)
        {
            throw new Exception("Invalid email.");
        }
        
        // Validate the password
        if (user.Password != password)
        {
            throw new Exception("Incorrect password.");
        }
        
        // Create the JWT Token
        var token = jwtTokenGenerator.GenerateToken(user);
        
        return new AuthenticationResult()
        {
            User = user,
            Token = token
        };
    }

    public OneOf<AuthenticationResult, DuplicateEmailError> Register(string firstName, string lastName, string email, string password)
    {
        // Check if a user already exists
        var user = userRepository.GetUserByEmail(email);
        if (user is not null)
        {
            return new DuplicateEmailError();
        }

        // Create a user (generate a unique id)
        Guid id = Guid.NewGuid();
        user = new User()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

        userRepository.Add(user);

        // Create a JWT Token
        var token = jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult()
        {
            User = user,
            Token = token
        };
    }
}