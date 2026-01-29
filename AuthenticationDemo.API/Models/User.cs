namespace AuthenticationDemo.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string TotpSecret { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? LastLoginDate { get; set; }

        // Navigation property
        public ICollection<TrustedDevice> TrustedDevices { get; set; } = new List<TrustedDevice>();
    }
}