namespace AuthenticationDemo.API.DTOs
{
    public class VerifyTotpRequest
    {
        public string Username { get; set; } = string.Empty;
        public string TotpCode { get; set; } = string.Empty;
        public string? DeviceToken { get; set; }
    }
}