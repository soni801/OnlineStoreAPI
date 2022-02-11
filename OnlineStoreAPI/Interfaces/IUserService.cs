using OnlineStoreAPI.Models;

namespace OnlineStoreAPI.Interfaces;

public interface IUserService
{
    public User GetUser(string token);
    public bool CreateUser(string firstName, string lastName, string username, string email, int phoneNumber, string passphrase, int accessLevel, string profilePictureUrl);
    public bool DeleteUser(string username);
}