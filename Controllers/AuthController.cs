using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MSFEP.Models;
using MSFEP.Services;
using System.Security.Claims;

namespace MSFEP.Controllers;

[Authorize]
[Route("auth")]
[ApiController]
public class AuthController(IMemoryCache cache, IVerificationService verificationService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IMemoryCache _cache = cache;
    private readonly IVerificationService _verificationService = verificationService;
    private readonly ILogger<AuthController> _logger = logger;

    [AllowAnonymous]
    [HttpGet("login")]
    public async Task<IActionResult> SignIn([FromQuery] string state)
    {
        if (_cache.TryGetValue(state, out PresentationCallbackRequest callback))
        {
            if (callback?.verifiedCredentialsData?.Any() == true)
            {
                var displayName = callback.verifiedCredentialsData[0].claims["displayName"];
                var givenName = callback.verifiedCredentialsData[0].claims["givenName"];
                var jobTitle = callback.verifiedCredentialsData[0].claims["jobTitle"];
                var preferredLanguage = callback.verifiedCredentialsData[0].claims["preferredLanguage"];
                var surname = callback.verifiedCredentialsData[0].claims["surname"];
                var mail = callback.verifiedCredentialsData[0].claims["mail"];
                var userPrincipalName = callback.verifiedCredentialsData[0].claims["revocationId"];

                var userClaims = new List<Claim>
                    {
                        new("displayName", displayName),
                        new("firstName", givenName),
                        new("lastName", surname),
                        new("preferredLanguage", preferredLanguage),
                        new("jobTitle", jobTitle),
                        new("email", mail),
                        new("userPrincipalName", userPrincipalName),
                    };

                var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Redirect("/");
            }

            return BadRequest("The presentationCallbackResponse contains no credentials.");
        }

        return BadRequest("The presentationCallbackResponse wasnt found.");
    }


    [Authorize]
    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect("/");
    }
}
