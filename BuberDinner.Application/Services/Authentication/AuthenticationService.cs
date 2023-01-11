using System;
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
        var token = jwtTokenGenerator.GenerateToken(user.Id, user.FirstName, user.LastName);
        
        return new AuthenticationResult()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Token = token
        };
    }

    public AuthenticationResult Register(string firstName, string lastName, string email, string password)
    {
        // Check if a user already exists
        var user = userRepository.GetUserByEmail(email);
        if (user is not null)
        {
            throw new Exception("User with this email already exists.");
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
        var token = jwtTokenGenerator.GenerateToken(id, firstName, lastName);

        return new AuthenticationResult()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = "test@test.com",
            Token = token
        };
    }
}