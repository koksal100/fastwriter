# fastwriter — Zabıt Katipliği Klavye Sınavı

ASP.NET Core MVC (.NET 10) tabanlı klavye hız ve doğruluk testi.

## Bu sürümde eklenenler

### 1. Karakter bazlı gerçek sınav metrikleri
- Metin artık kelime kelime değil, **karakter karakter** karşılaştırılıyor.
- Her karakter doğru (yeşil), yanlış (kırmızı) veya bekliyor (gri) olarak gösteriliyor.
- Hesaplanan metrikler:
  - **Net WPM**: doğru karakter / 5 / dakika
  - **Ham WPM**: toplam yazılan karakter / 5 / dakika
  - **Doğruluk**: doğru karakter / toplam yazılan karakter
  - Doğru / yanlış / toplam karakter ve doğru kelime sayısı
- 3 dakikada 90 doğru kelime hedefi sonuç ekranında gösteriliyor.
- Geri tuşuyla düzeltme, çoklu boşluk ve metin sonu durumları düzgün işleniyor.
- Skorlar **sunucuda yeniden hesaplanıyor**; istemciden gelen WPM değerine güvenilmiyor.

### 2. Liderlik tablosu
- `/Leaderboard` adresinde tüm yarışmacıların sıralaması.
- Her kullanıcı yalnızca **en iyi net WPM** değeriyle bir kez listelenir.
- Eşit WPM'de doğruluk oranı yüksek olan öne geçer.
- Dönem filtreleri: **Tüm Zamanlar**, **Bu Hafta** (son 7 gün), **Son 24 Saat**.
- Tabloya yalnızca **180 saniyelik tamamlanmış denemeler** girer; böylece kısa sürede bitirilip şişirilen skorlar sıralamayı bozamaz.
- İlk 3 için madalyalı kartlar, giriş yapan kullanıcı için kendi sırası ve satır vurgusu.
- Liderlik tablosu herkese açıktır; e-posta adresi gösterilmez, yalnızca görünen ad kullanılır.

### 3. Kullanıcı hesapları, Google girişi ve skor kaydı
- ASP.NET Core Identity (e-posta + şifre) ile kayıt / giriş / çıkış.
- Google ile giriş desteği (anahtarlar tanımlanınca otomatik etkinleşir).
- EF Core + SQLite ile kullanıcı ve skor saklama.
- Sonuç ekranında otomatik skor kaydı ve `/Scores` sayfasında geçmiş.
- Skorlarım sayfasında en iyi / ortalama WPM, ortalama doğruluk ve toplam deneme özeti.
- Giriş yapmamış kullanıcılar testi çözebilir; skorları kaydedilmez.

## Çalıştırma

```powershell
cd fastwriter
dotnet run
```

Varsayılan adres: `http://localhost:5050`

İlk çalıştırmada veritabanı otomatik oluşturulur ve 3 örnek sınav metni eklenir.

## Google ile giriş kurulumu

1. [Google Cloud Console](https://console.cloud.google.com/) → **APIs & Services** → **Credentials**.
2. **Create Credentials** → **OAuth client ID** → **Web application**.
3. Yetkili yönlendirme adresleri (Authorized redirect URIs):
   - `https://localhost:7219/signin-google`
   - `http://localhost:5050/signin-google`
4. Anahtarları **repoya commit etmeden** ortam değişkeni veya user-secrets ile tanımla.

   Ortam değişkeni (PowerShell):

   ```powershell
   $env:Authentication__Google__ClientId = "BURAYA_CLIENT_ID"
   $env:Authentication__Google__ClientSecret = "BURAYA_CLIENT_SECRET"
   ```

   veya user-secrets:

   ```powershell
   cd fastwriter
   dotnet user-secrets init
   dotnet user-secrets set "Authentication:Google:ClientId" "BURAYA_CLIENT_ID"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "BURAYA_CLIENT_SECRET"
   ```

5. Uygulamayı yeniden başlat. Giriş sayfasındaki Google butonu artık aktif olur.

> `ClientSecret` değerini `appsettings.json` içine yazıp public repoya **commit etmeyin**. `.gitignore` yerel veritabanını ve `appsettings.*.local.json` dosyalarını hariç tutar.

> Anahtarlar boşken Google butonu devre dışıdır; e-posta/şifre ile giriş çalışmaya devam eder.

## Veritabanı

- Varsayılan dosya: `fastwriter/fastwriter.db` (SQLite)
- `fastwriter.db` `.gitignore` içindedir; kullanıcı hesapları ve skorlar repoya gönderilmez.
- Migration'lar: `fastwriter/Data/Migrations`
- Uygulama başlarken `MigrateAsync()` ve örnek metin seed işlemi çalışır.

Yeni migration:

```powershell
cd fastwriter
dotnet ef migrations add MigrationAdi --output-dir Data/Migrations
dotnet ef database update
```

## Değişen / eklenen dosyalar

| Dosya | Amaç |
|---|---|
| `Models/ApplicationUser.cs` | Identity kullanıcısı + görünen ad |
| `Models/Score.cs` | Kaydedilen skor entity'si |
| `Models/TypingText.cs` | Sınav metni entity'si |
| `Data/ApplicationDbContext.cs` | Identity + uygulama tabloları |
| `Data/DbSeeder.cs` | 3 örnek sınav metni |
| `Data/ApplicationDbContextFactory.cs` | `dotnet ef` design-time desteği |
| `Services/TypingMetrics.cs` | Sunucu tarafı metrik hesabı |
| `Services/ExamContent.cs` | Süre / hedef sabitleri ve metin uzunluğu |
| `Controllers/HomeController.cs` | Rastgele metin API'si (`/Home/GetTestText`) |
| `Controllers/AccountController.cs` | Kayıt, giriş, çıkış, Google callback |
| `Controllers/ScoresController.cs` | Skorlarım sayfası (kişisel geçmiş) |
| `Controllers/ScoresApiController.cs` | `POST /api/scores` (yetki + antiforgery + doğrulama) |
| `Controllers/LeaderboardController.cs` | Tüm yarışmacıların en iyi skorlarını sıralama |
| `Views/Leaderboard/Index.cshtml` | Liderlik tablosu, dönem sekmeleri, madalyalar |
| `ViewModels/LeaderboardViewModels.cs` | Liderlik satırı ve sayfa modeli |
| `ViewModels/AccountViewModels.cs` | Giriş / kayıt form modelleri |
| `ViewModels/ScoreViewModels.cs` | Skor DTO ve geçmiş modeli |
| `Views/Home/Index.cshtml` | Yeni karakter bazlı test arayüzü ve sonuç paneli |
| `Views/Account/Login.cshtml` | Giriş sayfası + Google butonu |
| `Views/Account/Register.cshtml` | Kayıt sayfası |
| `Views/Scores/Index.cshtml` | Skor geçmişi ve özet kartları |
| `Views/Shared/_Layout.cshtml` | Türkçe menü, kullanıcı durumu, çıkış |
| `wwwroot/css/site.css` | Koyu tema + karakter renkleri |
| `Program.cs` | DbContext, Identity, Google, cookie/401 ayarları, migrate + seed |
| `appsettings.json` | SQLite bağlantısı ve Google anahtar alanları |
| `fastwriter.csproj` | EF Core, Identity ve Google paketleri |

## Notlar

- Şifre kuralı: en az 6 karakter (sınav uygulaması için basit tutuldu).
- API istekleri `[Authorize]` ve antiforgery token ile korunur.
- `/api` altındaki yetkisiz istekler login sayfasına yönlendirilmez, `401` döner.
- Yapıştırma (paste) engellenmiştir.
- Skor kaydı sırasında sunucu, gönderilen metni veritabanındaki referans metinle karşılaştırıp tüm metrikleri yeniden hesaplar.
