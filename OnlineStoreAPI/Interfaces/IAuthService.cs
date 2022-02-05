namespace OnlineStoreAPI.Interfaces;

public interface IAuthService
{
    public bool VerifyCredentials(string user, string pass);
}