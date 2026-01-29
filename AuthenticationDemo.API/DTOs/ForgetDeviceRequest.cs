namespace AuthenticationDemo.API.DTOs
{
    public class ForgetDeviceWithUserRequest
    {
        public string DeviceToken { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}