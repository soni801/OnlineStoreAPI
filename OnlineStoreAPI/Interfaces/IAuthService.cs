namespace OnlineStoreAPI.Interfaces;

public interface IAuthService
{
    public string VerifyCredentials(string user, string pass);
    public bool UpdatePassphrase(string user, string pass, string newPass);
}