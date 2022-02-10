using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Interfaces;

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
    public string VerifyCredentials([FromHeader] string user, [FromHeader] string pass)
    {
        return _authService.VerifyCredentials(user, pass);
    }

    [HttpPost]
    public bool UpdatePassphrase([FromHeader] string user, [FromHeader] string pass, [FromHeader] string newPass)
    {
        return _authService.UpdatePassphrase(user, pass, newPass);
    }
}