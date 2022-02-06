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
    public string VerifyCredentials(string user, string pass)
    {
        return _authService.VerifyCredentials(user, pass);
    }
}