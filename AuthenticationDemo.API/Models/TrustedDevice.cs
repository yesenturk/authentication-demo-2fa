namespace AuthenticationDemo.API.Models
{
    public class TrustedDevice
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DeviceToken { get; set; } = string.Empty;
        public string? DeviceName { get; set; }
        public string? IpAddress { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastUsedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public User User { get; set; } = null!;
    }
}