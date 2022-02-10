using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Models;

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

    [HttpGet]
    public User GetUser(int id)
    {
        return _userService.GetUser(id);
    }
    
    [HttpPost]
    public bool CreateUser([FromHeader] string firstName, [FromHeader] string lastName, [FromHeader] string username, [FromHeader] string email, [FromHeader] int phoneNumber, [FromHeader] string passphrase, int accessLevel, [FromHeader] string profilePictureUrl)
    {
        return _userService.CreateUser(firstName, lastName, username, email, phoneNumber, passphrase, accessLevel, profilePictureUrl);
    }

    [HttpDelete]
    public bool DeleteUser(string username)
    {
        return _userService.DeleteUser(username);
    }
}