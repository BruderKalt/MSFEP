using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MSFEP.Models;

namespace MSFEP.Controllers;

[AllowAnonymous]
[ApiController]
[Route("verification")]
public class PresentationController(IMemoryCache cache) : Controller
{
    private readonly IMemoryCache _cache = cache;

    [HttpPost("presentationCallback")]
    public async Task<IActionResult> PresentationCallback([FromBody] PresentationCallbackRequest callbackRequest)
    {
        // callbackRequest.requestStatus == "request_retrieved" for when presentation is in progress
        if (callbackRequest.requestStatus == "presentation_verified" || callbackRequest.requestStatus == "presentation_error")
        {
            _cache.Set(callbackRequest.state, callbackRequest, TimeSpan.FromMinutes(5));
            return Ok();
        }

        return BadRequest();
    }

    [Authorize]
    [HttpGet("request-status")]
    public IActionResult GetRequestStatus([FromQuery] string state)
    {
        if (_cache.TryGetValue(state, out PresentationCallbackRequest callback))
        {
            return Ok(callback);
        }

        return NotFound();
    }
}