using BuberDinner.Application.Services.Authentication;
using BuberDinner.Contract.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[Route("auth")]
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
        // 2 steps
        // Get response from service, it now has either an error object or the AuthenticationResult from the Auth Service
        var result = authenticationService.Register(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password
        );

        // Long way. See login for shortcut
        if (result.IsError)
        {
            // Return the error
            return Problem(result.Errors);
        }
        else
        {
            var authResult = result.Value;
            // Map services response to http response. (The Laravel equivalent is API resources)
            var authResponse = MapAuthResponse(authResult);
            
            return Ok(authResponse);
        }
    }



    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // 2 steps
        // Service call
        var result = authenticationService.Login(request.Email, request.Password);

        // This works, but only for a single error.
        return result.Match(
            authResult => Ok(MapAuthResponse(authResult)),
            errors => Problem(errors));
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
