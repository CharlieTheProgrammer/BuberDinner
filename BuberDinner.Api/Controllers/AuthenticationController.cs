using System.Diagnostics.Eventing.Reader;
using BuberDinner.Api.Filters;
using BuberDinner.Application.Services.Authentication;
using BuberDinner.Contract.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Route("auth")]
// [ErrorHandlingFilter]
public class AuthenticationController : ControllerBase
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
        // 2 steps
        // Get response from service
        var registerResult = authenticationService.Register(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password
        );

        // This is the longer way to deal with OneOf object
        // if (registerResult.IsT0)
        // {
        //     var authResult = registerResult.AsT0;
        //     // Map services response to http response. (The Laravel equivalent is API resources)
        //     return Ok(MapAuthResponse(authResult));
        // }
        //
        // return Problem(statusCode: StatusCodes.Status409Conflict, title: "Email already exists!" );
        
        // The shortcut
        return registerResult.Match(
            authResult => Ok(MapAuthResponse(authResult)),
            error => Problem(statusCode: StatusCodes.Status409Conflict, title: "Email already exists!"));
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // 2 steps
        // Service call
        var loginResult = authenticationService.Login(request.Email, request.Password);

        // Map to API response
        return loginResult.Match(
            authResult => Ok(MapAuthResponse(authResult)),
            error => Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Login failed."));
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
