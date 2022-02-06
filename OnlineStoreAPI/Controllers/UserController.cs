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
    public bool CreateUser(string firstName, string lastName, string username, string email, int phoneNumber, string passphrase, int accessLevel)
    {
        return _userService.CreateUser(firstName, lastName, username, email, phoneNumber, passphrase, accessLevel);
    }

    [HttpDelete]
    public bool DeleteUser(string username)
    {
        return _userService.DeleteUser(username);
    }
}