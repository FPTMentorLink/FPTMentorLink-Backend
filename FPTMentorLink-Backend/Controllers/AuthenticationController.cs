using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Models.Request.Authentication;
using IAuthenticationService = Services.Interfaces.IAuthenticationService;

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

    [HttpGet("signin-google")]
    [Authorize(AuthenticationSchemes = GoogleDefaults.AuthenticationScheme)]
    public async Task<IActionResult> LoginByGoogle()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return Unauthorized();
        }

        var result = await _authenticationService.LoginByGoogleAsync(authenticateResult.Principal);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authenticationService.LoginAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
}