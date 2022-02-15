using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Models.Requests;

namespace OnlineStoreAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public string VerifyCredentials([FromBody] VerifyRequest payload)
    {
        return _authService.VerifyCredentials(payload.Username, payload.Passphrase);
    }

    [HttpPost]
    public bool UpdatePassphrase([FromBody] UpdateCredentialsRequest payload)
    {
        return _authService.UpdatePassphrase("", "", ""); // TODO: Fill this
    }
}