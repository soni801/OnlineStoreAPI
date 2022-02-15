namespace OnlineStoreAPI.Models.Requests;

public class VerifyRequest
{
    public string Username { get; set; } = null!;
    public string Passphrase { get; set; } = null!;
}