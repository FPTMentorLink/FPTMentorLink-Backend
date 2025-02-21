using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthenticationService = Services.Interfaces.IAuthenticationService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace FPTMentorLink_Backend.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = $"{Request.Scheme}://{Request.Host}/api/authentication/google-callback",
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        try 
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var loginResult = await _authenticationService.LoginAsync(result.Principal);
            return loginResult.IsSuccess ? Ok(loginResult.Value) : BadRequest(loginResult.Error);
        }
        catch (Exception ex)
        {
            // Log the exception details
            return Unauthorized(new { error = "Authentication failed", message = ex.Message });
        }
    }
}