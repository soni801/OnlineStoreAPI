namespace OnlineStoreAPI.Models;

public class Credentials
{
    public string Username { get; set; } = null!;
    public string Passphrase { get; set; } = null!;
    public string Token { get; set; } = null!;
    public int AccessLevel { get; set; }
}