using BuberDinner.Application.Services.Authentication;
using BuberDinner.Contract.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BuberDinner.Api.Controllers;

[ApiController]
[Route("auth")]
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
        var authResult = authenticationService.Register(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password
        );

        // Map services response to http response. (The Laravel equivalent is API resources)
        var authResponse = new AuthenticationResponse(
            authResult.User.Id,
            authResult.User.FirstName,
            authResult.User.LastName,
            authResult.User.Email,
            authResult.Token
        );


        return Ok(authResponse);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // 2 steps
        // Service call
        var authResult = authenticationService.Login(request.Email, request.Password);

        // Map to API response
        var authResponse = new AuthenticationResponse(
            authResult.User.Id,
            authResult.User.FirstName,
            authResult.User.LastName,
            authResult.User.Email,
            authResult.Token
        );

        return Ok(authResponse);
    }
}
