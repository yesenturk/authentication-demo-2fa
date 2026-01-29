using AuthenticationDemo.API.Services.Interfaces;
using OtpNet;
using QRCoder;

namespace AuthenticationDemo.API.Services.Implementations
{
    public class TotpService : ITotpService
    {
        public string GenerateSecret()
        {
            // 20 byte (160 bit) secret key oluştur
            var key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        public string GenerateQrCodeUri(string username, string secret)
        {
            // Google Authenticator formatı
            // otpauth://totp/{issuer}:{username}?secret={secret}&issuer={issuer}
            var issuer = "AuthenticationDemo";
            return $"otpauth://totp/{issuer}:{username}?secret={secret}&issuer={issuer}";
        }

        public byte[] GenerateQrCodeImage(string qrCodeUri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20); // 20 pixel per module
        }

        public bool ValidateCode(string secret, string code)
        {
            try
            {
                var secretBytes = Base32Encoding.ToBytes(secret);
                var totp = new Totp(secretBytes);

                // 30 saniyelik window ile doğrula (önceki, şu anki, sonraki)
                return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
            }
            catch
            {
                return false;
            }
        }
    }
}