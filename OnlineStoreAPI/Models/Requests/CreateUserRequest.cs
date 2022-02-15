namespace OnlineStoreAPI.Models.Requests;

public class CreateUserRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int PhoneNumber { get; set; }
    public string Passphrase { get; set; } = null!;
}