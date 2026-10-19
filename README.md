# fastwriter — Zabıt Katipliği Klavye Sınavı

Zabıt katipliği sınavına hazırlanmak için 3 dakikalık klavye hız ve doğruluk testi.

## Özellikler

- 3 dakikalık sınav simülasyonu ve 90 doğru kelime hedefi
- Karakter bazlı doğruluk takibi: doğru, yanlış ve bekleyen karakterler
- Net WPM, ham WPM, doğruluk ve doğru kelime sayısı
- Geri tuşuyla düzeltme; yapıştırma engeli
- Üyelik ile skor kaydı ve kişisel geçmiş
- Tüm kullanıcılar arasında liderlik tablosu
- Google ile giriş desteği (isteğe bağlı)

## Kullanım

1. Testi başlat.
2. Ekrandaki metni 3 dakika boyunca yaz.
3. Süre bitince veya **Bitir** butonuna basınca sonuçlar gösterilir.
4. Giriş yaptıysan skorun otomatik kaydedilir.

Giriş yapmadan da test çözülebilir; ancak bu durumda skor kaydedilmez.

## Liderlik tablosu

- Her kullanıcı en iyi net WPM değeriyle listelenir.
- Eşit WPM'de doğruluk oranı yüksek olan öne geçer.
- Yalnızca 180 saniyelik tamamlanmış denemeler dikkate alınır.
- Tüm zamanlar, son 7 gün ve son 24 saat filtreleri vardır.

## Çalıştırma

Gereksinim: .NET 10 SDK

```powershell
cd fastwriter
dotnet run
```

Uygulama: http://localhost:5050

İlk çalıştırmada veritabanı otomatik oluşturulur ve örnek sınav metinleri eklenir.

## Google ile giriş (isteğe bağlı)

Google Cloud Console'da bir OAuth client oluşturup şu yönlendirme adreslerini ekle:

- `https://localhost:7219/signin-google`
- `http://localhost:5050/signin-google`

Anahtarları ortam değişkeni olarak tanımla:

```powershell
$env:Authentication__Google__ClientId = "..."
$env:Authentication__Google__ClientSecret = "..."
```

Anahtarlar tanımlı değilse Google butonu devre dışı kalır; e-posta ve şifreyle giriş çalışmaya devam eder.

## Notlar

- Veritabanı yerel bir SQLite dosyasıdır (`fastwriter.db`) ve repoya gönderilmez.
- Şifre en az 6 karakter olmalıdır.
