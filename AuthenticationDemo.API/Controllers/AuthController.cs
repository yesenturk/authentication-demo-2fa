using AuthenticationDemo.API.DTOs;
using AuthenticationDemo.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthenticationDemo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Kullanıcı adı ve şifre ile giriş
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOs.LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Kullanıcı adı ve şifre gereklidir!"
                });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.LoginAsync(request, ipAddress);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// 2FA TOTP kodunu doğrula
        /// </summary>
        [HttpPost("verify-totp")]
        public async Task<IActionResult> VerifyTotp([FromBody] VerifyTotpRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.TotpCode))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Kullanıcı adı ve doğrulama kodu gereklidir!"
                });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.VerifyTotpAsync(request, ipAddress);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// Cihazı güvenilir listeden çıkar (Beni Unut)
        /// </summary>
        [HttpPost("forget-device")]
        public async Task<IActionResult> ForgetDevice([FromBody] ForgetDeviceWithUserRequest request)
        {
            if (string.IsNullOrEmpty(request.DeviceToken) || request.UserId == 0)
            {
                return BadRequest(new { success = false, message = "Device token ve user ID gereklidir!" });
            }

            var result = await _authService.ForgetDeviceAsync(request.DeviceToken, request.UserId);

            if (!result)
                return NotFound(new { success = false, message = "Cihaz bulunamadı!" });

            return Ok(new { success = true, message = "Cihaz başarıyla unutuldu!" });
        }

        /// <summary>
        /// Yeni kullanıcı kaydı (test için)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOs.RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { success = false, message = "Kullanıcı adı ve şifre gereklidir!" });
            }

            if (request.Password.Length < 6)
            {
                return BadRequest(new { success = false, message = "Şifre en az 6 karakter olmalıdır!" });
            }

            var result = await _authService.RegisterUserAsync(request.Username, request.Password);

            if (!result)
                return BadRequest(new { success = false, message = "Bu kullanıcı adı zaten kullanılıyor!" });

            return Ok(new { success = true, message = "Kullanıcı başarıyla oluşturuldu!" });
        }

        /// <summary>
        /// Test endpoint - API çalışıyor mu kontrol
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.Now,
                message = "Authentication API is running!"
            });
        }
    }
}