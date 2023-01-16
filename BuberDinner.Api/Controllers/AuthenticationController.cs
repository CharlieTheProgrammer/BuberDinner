using BuberDinner.Api.Common.Validators;
using BuberDinner.Application.Services.Authentication;
using BuberDinner.Contract.Authentication;
using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("auth")]
[AllowAnonymous]
public class AuthenticationController : ApiController
{
    private readonly IAuthenticationService authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }


    // GET: /<controller>/
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        // Run validation. Instantiate the validator and pass in the register request
        RegisterRequestValidator validator = new RegisterRequestValidator();
        ValidationResult validationResult = validator.Validate(request);

        if (validationResult.IsValid == false)
        {
            return ValidationProblem(validationResult);
        }
        
        // Get response from service, it now has either an error object or the AuthenticationResult from the Auth Service
        ErrorOr<AuthenticationResult> result = authenticationService.Register(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password
        );
        
        return result.Match(authResult => Ok(MapAuthResponse(authResult)), Problem);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Service call
        ErrorOr<AuthenticationResult> result = authenticationService.Login(request.Email, request.Password);
     
        // Match maps to correct function
        return result.Match(authResult => Ok(MapAuthResponse(authResult)), Problem);
    }
    
    private static AuthenticationResponse MapAuthResponse(AuthenticationResult authResult)
    {
        var authResponse = new AuthenticationResponse(
            authResult.User.Id,
            authResult.User.FirstName,
            authResult.User.LastName,
            authResult.User.Email,
            authResult.Token
        );
        return authResponse;
    }
}
