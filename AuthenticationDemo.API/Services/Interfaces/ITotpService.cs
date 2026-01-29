namespace AuthenticationDemo.API.Services.Interfaces
{
    public interface ITotpService
    {
        string GenerateSecret();
        string GenerateQrCodeUri(string username, string secret);
        byte[] GenerateQrCodeImage(string qrCodeUri);
        bool ValidateCode(string secret, string code);
    }
}