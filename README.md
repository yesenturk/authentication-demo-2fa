# Two-Factor Authentication System

İki faktörlü kimlik doğrulama (2FA) sistemi. Google Authenticator ile TOTP doğrulaması yapılır.

## Teknolojiler

- **Backend:** .NET Core 8 Web API
- **Frontend:** Angular 16
- **Database:** Microsoft SQL Server
- **Authentication:** JWT + Google Authenticator (TOTP)

## Özellikler

✅ Kullanıcı kayıt ve giriş  
✅ Google Authenticator ile 2FA  
✅ Güvenilir cihaz hatırlama  
✅ "Beni Unut" özelliği  
✅ JWT token tabanlı authentication  

## Kurulum

### Backend

1. SQL Server'da `AuthenticationDemo` database'ini oluştur
2. `appsettings.Development.json` dosyasını düzenle (connection string)
3. Migration uygula:
```bash
   Update-Database
```
4. Projeyi çalıştır:
```bash
   dotnet run
```

### Frontend

1. Node.js yükle (v16+)
2. Paketleri yükle:
```bash
   npm install
```
3. Uygulamayı başlat:
```bash
   ng serve
```
4. Tarayıcıda aç: `http://localhost:4200`

## Kullanım

1. **Kayıt Ol** sayfasından yeni kullanıcı oluştur
2. **Giriş Yap** sayfasından giriş yap
3. QR kodu Google Authenticator ile tarat
4. 6 haneli kodu gir
5. Giriş başarılı!

## Ekran Görüntüleri

(TODO: Ekran görüntüleri eklenecek)

## Lisans

MIT License
