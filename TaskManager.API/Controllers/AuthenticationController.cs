using Microsoft.AspNetCore.Mvc;
using System.Net;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;

namespace TaskManager.API.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenService _tokenService;

    public AuthenticationController(IAuthenticationService authenticationService, ITokenService tokenService)
    {
        this._authenticationService = authenticationService;
        this._tokenService = tokenService;
    }

    /// <summary>
    /// Authenticate
    /// </summary>
    /// <param name="authentication"></param>
    /// <returns>Token</returns>
    /// <response code="200">Token is generated</response>
    /// <response code="400">Params is not correct</response>  
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] Authentication authentication)
    {
        if (!ModelState.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ModelState.ValidationState);
        }

        User user = await this._authenticationService.Authenticate(authentication);

        if (user == null) return StatusCode((int)HttpStatusCode.Forbidden, "User not found");

        string token = this._tokenService.Create(user);

        return Ok(new { token = token });
    }
}

