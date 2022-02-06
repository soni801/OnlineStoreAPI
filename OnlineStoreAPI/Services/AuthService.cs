using MySqlConnector;
using OnlineStoreAPI.Interfaces;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace OnlineStoreAPI.Services;

public class AuthService : IAuthService
{
    public string VerifyCredentials(string user, string pass)
    {
        var token = "";
        
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "select token from online_store.credentials where username = @user and passphrase = @pass";
        var command = new MySqlCommand(commandString, connection);

        command.Parameters.AddWithValue("@user", user);
        command.Parameters.AddWithValue("@pass", pass);

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

        command.Parameters.AddWithValue("@user", user);
        command.Parameters.AddWithValue("@pass", pass);
        command.Parameters.AddWithValue("@newPass", newPass);

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