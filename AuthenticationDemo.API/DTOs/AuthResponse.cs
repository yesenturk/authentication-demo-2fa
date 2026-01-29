namespace AuthenticationDemo.API.DTOs
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public string? QrCodeImage { get; set; } // Base64 QR code
    }
}