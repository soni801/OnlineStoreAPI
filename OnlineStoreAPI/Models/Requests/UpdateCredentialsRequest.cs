namespace OnlineStoreAPI.Models.Requests;

public class UpdateCredentialsRequest
{
    public string Token { get; set; } = null!;
    public string NewPassphrase { get; set; } = null!;
}