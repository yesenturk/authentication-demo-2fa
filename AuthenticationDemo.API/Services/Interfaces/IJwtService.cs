namespace AuthenticationDemo.API.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string username, int userId);
    }
}