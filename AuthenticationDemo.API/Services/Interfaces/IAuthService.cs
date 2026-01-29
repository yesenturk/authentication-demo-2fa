using AuthenticationDemo.API.DTOs;

namespace AuthenticationDemo.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress);
        Task<AuthResponse> VerifyTotpAsync(VerifyTotpRequest request, string? ipAddress);
        Task<bool> ForgetDeviceAsync(string deviceToken, int userId);
        Task<bool> RegisterUserAsync(string username, string password);
    }
}