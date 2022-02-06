﻿using OnlineStoreAPI.Models;

namespace OnlineStoreAPI.Interfaces;

public interface IUserService
{
    public User GetUser(int id);
    public bool CreateUser(string firstName, string lastName, string username, string email, int phoneNumber, string passphrase, int accessLevel);
    public bool DeleteUser(string username);
}