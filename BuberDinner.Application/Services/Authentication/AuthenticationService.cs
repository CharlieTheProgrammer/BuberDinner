using System;
using BuberDinner.Application.Common.Errors;
using BuberDinner.Application.Common.Interfaces.Authentication;
using BuberDinner.Application.Common.Interfaces.Persistence;
using BuberDinner.Domain.Entities;

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

    public AuthenticationResult Login(string email, string password)
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

    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
        // Check if a user already exists
        var user = userRepository.GetUserByEmail(email);
        if (user is not null)
        {
            throw new DuplicateEmailException();
            // throw new Exception("User with this email already exists.");
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