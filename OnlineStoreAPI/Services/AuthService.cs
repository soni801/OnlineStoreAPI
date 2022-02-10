﻿using System.Security.Cryptography;
using System.Text;
using MySqlConnector;
using OnlineStoreAPI.Interfaces;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace OnlineStoreAPI.Services;

public class AuthService : IAuthService
{
    private static string ByteArrayToString(byte[] arrInput)
    {
        int i;
        var sOutput = new StringBuilder(arrInput.Length);
        for (i = 0; i < arrInput.Length; i++) sOutput.Append(arrInput[i].ToString("X2"));
        return sOutput.ToString();
    }
    
    public string VerifyCredentials(string user, string pass)
    {
        var token = "";
        
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "select token from online_store.credentials where username = @user and passphrase = @pass";
        var command = new MySqlCommand(commandString, connection);

        var passBytes = Encoding.UTF8.GetBytes(pass);
        var passHash = SHA256.Create().ComputeHash(passBytes);
        
        command.Parameters.AddWithValue("@user", user);
        command.Parameters.AddWithValue("@pass", ByteArrayToString(passHash));

        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read()) token = (string) reader[0];

        return token;
    }

    public bool UpdatePassphrase(string user, string pass, string newPass)
    {
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "update online_store.credentials set passphrase = @newPass where username = @user and passphrase = @pass";
        var command = new MySqlCommand(commandString, connection);

        var passBytes = Encoding.UTF8.GetBytes(pass);
        var passHash = SHA256.Create().ComputeHash(passBytes);
        
        var newPassBytes = Encoding.UTF8.GetBytes(newPass);
        var newPassHash = SHA256.Create().ComputeHash(newPassBytes);

        command.Parameters.AddWithValue("@user", user);
        command.Parameters.AddWithValue("@pass", ByteArrayToString(passHash));
        command.Parameters.AddWithValue("@newPass", ByteArrayToString(newPassHash));

        try
        {
            connection.Open();
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