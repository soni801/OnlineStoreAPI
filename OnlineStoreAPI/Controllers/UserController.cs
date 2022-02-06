using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Interfaces;

namespace OnlineStoreAPI.Controllers;

[ApiController]
[Route("users")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public bool CreateUser(string firstName, string lastName, string username, string email, int phoneNumber, string passphrase, int accessLevel)
    {
        return _userService.CreateUser(firstName, lastName, username, email, phoneNumber, passphrase, accessLevel);
    }
}