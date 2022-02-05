using MySqlConnector;
using OnlineStoreAPI.Interfaces;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace OnlineStoreAPI.Services;

public class AuthService : IAuthService
{
    public bool VerifyCredentials(string user, string pass)
    {
        using var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
        const string commandString = "select count(*) from online_store.credentials where username = @user and passphrase = @pass";
        var command = new MySqlCommand(commandString, connection);

        command.Parameters.AddWithValue("@user", user);
        command.Parameters.AddWithValue("@pass", pass);

        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read()) if ((Int64) reader[0] == 1) return true;

        return false;
    }
}