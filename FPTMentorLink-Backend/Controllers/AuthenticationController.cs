using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services.Models.Request.Authentication;
using Services.Models.Request.Authorization;
using Services.Settings;
using Services.Utils;
using IAuthenticationService = Services.Interfaces.IAuthenticationService;

namespace FPTMentorLink_Backend.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly RedirectUrlSettings _redirectUrlSettings;

    public AuthenticationController(IAuthenticationService authenticationService,
        IOptions<RedirectUrlSettings> redirectUrlSettings)
    {
        _authenticationService = authenticationService;
        _redirectUrlSettings = redirectUrlSettings.Value;
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
        var redirectUrl = _redirectUrlSettings.PostGoogleLoginUrl;
        if (result is { IsSuccess: true, Value: not null })
        {
            redirectUrl = result.Value;
        }

        // Sign out the user from the authentication scheme
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect(redirectUrl);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authenticationService.LoginAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost("admin-login")]
    public async Task<IActionResult> AdminLogin([FromBody] AdminLoginRequest request)
    {
        var result = await _authenticationService.AdminLoginAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var accessToken = HttpContext.GetAccessToken();
        if (accessToken == null)
        {
            return BadRequest(Result.Failure("Invalid access token"));
        }

        request.AccessToken = accessToken;
        var result = await _authenticationService.RefreshTokenAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
}