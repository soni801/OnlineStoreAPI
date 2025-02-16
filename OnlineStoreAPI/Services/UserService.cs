﻿using System.Security.Cryptography;
using System.Text;
using MySqlConnector;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Models;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace OnlineStoreAPI.Services;

public class UserService : IUserService
{
    private static readonly Random Random = new Random();

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    private static string ByteArrayToString(byte[] arrInput)
    {
        int i;
        var sOutput = new StringBuilder(arrInput.Length);
        for (i = 0; i < arrInput.Length; i++) sOutput.Append(arrInput[i].ToString("X2"));
        return sOutput.ToString();
    }
    
    public User GetUser(string token)
    {
        var user = new User();
        
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "select * from online_store.users, online_store.credentials where users.username = credentials.username and token = @token";
        var command = new MySqlCommand(commandString, connection);

        command.Parameters.AddWithValue("@token", token);
        
        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            user.Id = (int) reader["id"];
            user.FirstName = (string) reader["first_name"];
            user.LastName = (string) reader["last_name"];
            user.Credentials = new Credentials
            {
                Username = (string) reader["username"],
                Passphrase = (string) reader["passphrase"],
                Token = (string) reader["token"],
                AccessLevel = (int) reader["access_level"]
            };
            user.Email = (string) reader["email"];
            user.PhoneNumber = (int) reader["phone_number"];
            user.ProfilePictureUrl = (string) reader["profile_picture_url"];
        }

        return user;
    }

    public bool CreateUser(string firstName, string lastName, string username, string email, int phoneNumber, string passphrase)
    {
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        
        const string credentialsString = "insert into online_store.credentials (username, passphrase, token) values (@username, @passphrase, @token)";
        var credentialsCommand = new MySqlCommand(credentialsString, connection);
        
        const string userString = "insert into online_store.users (first_name, last_name, username, email, phone_number) values (@first_name, @last_name, @username, @email, @phone_number)";
        var userCommand = new MySqlCommand(userString, connection);

        var passBytes = Encoding.UTF8.GetBytes(passphrase);
        var passHash = SHA256.Create().ComputeHash(passBytes);
        
        credentialsCommand.Parameters.AddWithValue("@username", username);
        credentialsCommand.Parameters.AddWithValue("@passphrase", ByteArrayToString(passHash));
        credentialsCommand.Parameters.AddWithValue("@token", RandomString(64));

        userCommand.Parameters.AddWithValue("@first_name", firstName);
        userCommand.Parameters.AddWithValue("@last_name", lastName);
        userCommand.Parameters.AddWithValue("@username", username);
        userCommand.Parameters.AddWithValue("@email", email);
        userCommand.Parameters.AddWithValue("@phone_number", phoneNumber);

        try
        {
            connection.Open();
            credentialsCommand.ExecuteNonQuery();
            userCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
        return true;
    }

    public bool DeleteUser(string token)
    {
        var username = "";
        
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);

        const string getUsernameString = "select username from online_store.credentials where token = @token";
        var getUsernameCommand = new MySqlCommand(getUsernameString, connection);
        getUsernameCommand.Parameters.AddWithValue("@token", token);

        connection.Open();
        using var reader = getUsernameCommand.ExecuteReader();
        while (reader.Read()) username = (string) reader[0]; 
        reader.Close();
        
        const string commandString = "delete from online_store.users where username = @username; delete from online_store.credentials where username = @username";
        var command = new MySqlCommand(commandString, connection);
        command.Parameters.AddWithValue("@username", username);

        try
        {
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        
        return true;
    }
}