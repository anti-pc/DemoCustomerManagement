namespace WebApi.TokenAuthentication
{
    public interface ITokenManager
    {
        bool Authenticate(string username, string password);
        Token NewToken();
        bool VerifyToken(string token);
    }
}