using MySqlConnector;
using OnlineStoreAPI.Interfaces;
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
    
    public bool CreateUser(string firstName, string lastName, string username, string email, int phoneNumber, string passphrase, int accessLevel)
    {
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        
        const string credentialsString = "insert into online_store.credentials (username, passphrase, token, access_level) values (@username, @passphrase, @token, @access_level)";
        var credentialsCommand = new MySqlCommand(credentialsString, connection);
        
        const string userString = "insert into online_store.users (first_name, last_name, username, email, phone_number) values (@first_name, @last_name, @username, @email, @phone_number)";
        var userCommand = new MySqlCommand(userString, connection);

        credentialsCommand.Parameters.AddWithValue("@username", username);
        credentialsCommand.Parameters.AddWithValue("@passphrase", passphrase);
        credentialsCommand.Parameters.AddWithValue("@token", RandomString(64));
        credentialsCommand.Parameters.AddWithValue("@access_level", accessLevel);

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

    public bool DeleteUser(string username)
    {
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "delete from online_store.users where username = @username; delete from online_store.credentials where username = @username";
        var command = new MySqlCommand(commandString, connection);

        command.Parameters.AddWithValue("@username", username);

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