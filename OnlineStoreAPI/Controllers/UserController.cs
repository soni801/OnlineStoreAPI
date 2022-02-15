using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Models;
using OnlineStoreAPI.Models.Requests;

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
    public User GetUser([FromHeader] string token)
    {
        return _userService.GetUser(token);
    }
    
    [HttpPost]
    public bool CreateUser([FromBody] CreateUserRequest payload)
    {
        return _userService.CreateUser(payload.FirstName, payload.LastName, payload.Username, payload.Email, payload.PhoneNumber, payload.Passphrase);
    }

    [HttpDelete]
    public bool DeleteUser([FromHeader] string token)
    {
        return _userService.DeleteUser(token);
    }
}