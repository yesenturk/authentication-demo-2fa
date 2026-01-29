using AuthenticationDemo.API.Data;
using AuthenticationDemo.API.DTOs;
using AuthenticationDemo.API.Models;
using AuthenticationDemo.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AuthenticationDemo.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ITotpService _totpService;

        public AuthService(
            ApplicationDbContext context,
            IJwtService jwtService,
            ITotpService totpService)
        {
            _context = context;
            _jwtService = jwtService;
            _totpService = totpService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress)
        {
            // Kullanıcıyı bul
            var user = await _context.Users
                .Include(u => u.TrustedDevices)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Kullanıcı adı veya şifre hatalı!"
                };
            }

            // Şifreyi doğrula
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Kullanıcı adı veya şifre hatalı!"
                };
            }

            // Device token kontrolü
            if (!string.IsNullOrEmpty(request.DeviceToken))
            {
                var trustedDevice = user.TrustedDevices
                    .FirstOrDefault(d => d.DeviceToken == request.DeviceToken && d.IsActive);

                if (trustedDevice != null)
                {
                    // Güvenilir cihaz - 2FA atla
                    trustedDevice.LastUsedDate = DateTime.Now;
                    trustedDevice.IpAddress = ipAddress;
                    user.LastLoginDate = DateTime.Now;

                    await _context.SaveChangesAsync();

                    var token = _jwtService.GenerateToken(user.Username, user.Id);

                    return new AuthResponse
                    {
                        Success = true,
                        Message = "Giriş başarılı!",
                        Token = token,
                        RequiresTwoFactor = false
                    };
                }
            }

            // 2FA gerekli - QR kod oluştur
            var qrCodeUri = _totpService.GenerateQrCodeUri(user.Username, user.TotpSecret);
            var qrCodeImage = _totpService.GenerateQrCodeImage(qrCodeUri);
            var qrCodeBase64 = Convert.ToBase64String(qrCodeImage);

            return new AuthResponse
            {
                Success = true,
                Message = "2FA kodu gerekli",
                RequiresTwoFactor = true,
                QrCodeImage = $"data:image/png;base64,{qrCodeBase64}"
            };
        }

        public async Task<AuthResponse> VerifyTotpAsync(VerifyTotpRequest request, string? ipAddress)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !user.IsActive)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Kullanıcı bulunamadı!"
                };
            }

            // TOTP kodunu doğrula
            if (!_totpService.ValidateCode(user.TotpSecret, request.TotpCode))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Geçersiz doğrulama kodu!"
                };
            }

            // Device token varsa güvenilir cihaz olarak kaydet
            if (!string.IsNullOrEmpty(request.DeviceToken))
            {
                var existingDevice = await _context.TrustedDevices
                    .FirstOrDefaultAsync(d => d.DeviceToken == request.DeviceToken && d.UserId == user.Id);

                if (existingDevice == null)
                {
                    var newDevice = new TrustedDevice
                    {
                        UserId = user.Id,
                        DeviceToken = request.DeviceToken,
                        IpAddress = ipAddress,
                        DeviceName = "Browser", // Frontend'den gönderebiliriz
                        IsActive = true
                    };

                    _context.TrustedDevices.Add(newDevice);
                }
                else
                {
                    existingDevice.LastUsedDate = DateTime.Now;
                    existingDevice.IsActive = true;
                    existingDevice.IpAddress = ipAddress;
                }
            }

            user.LastLoginDate = DateTime.Now;
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user.Username, user.Id);

            return new AuthResponse
            {
                Success = true,
                Message = "Giriş başarılı!",
                Token = token,
                RequiresTwoFactor = false
            };
        }

        public async Task<bool> ForgetDeviceAsync(string deviceToken, int userId)
        {
            if (userId == 0)
            {
                // userId yoksa hata ver
                return false;
            }

            // Belirli kullanıcıya ait cihazı bul
            var device = await _context.TrustedDevices
                .FirstOrDefaultAsync(d => d.DeviceToken == deviceToken && d.UserId == userId && d.IsActive);

            if (device == null)
                return false;

            device.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RegisterUserAsync(string username, string password)
        {
            // Kullanıcı zaten var mı?
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                TotpSecret = _totpService.GenerateSecret(),
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}