using fastwriter.Models;
using Microsoft.EntityFrameworkCore;

namespace fastwriter.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.TypingTexts.AnyAsync())
        {
            return;
        }

        db.TypingTexts.AddRange(
            new TypingText
            {
                Title = "Zabıt Katipliği ve Adalet Sistemi",
                Category = "Hukuk",
                Difficulty = 1,
                Content = "Zabıt katipliği adalet sisteminin en temel yapı taşlarından biridir. Mahkemelerde, savcılıklarda ve icra dairelerinde görev yapan zabıt katipleri, yargılama sürecinin düzenli ve eksiksiz bir şekilde ilerlemesini sağlarlar. Duruşma esnasında hakim veya savcının beyanlarını, tanık ifadelerini ve sanık savunmalarını tutanağa eksiksiz bir biçimde geçirmek en önemli asli görevlerindendir. Hızlı ve hatasız klavye kullanımı bu meslekte hayati bir önem taşır. Adalet Bakanlığı tarafından düzenlenen sınavlarda adayların üç dakika içerisinde en az doksan kelimeyi doğru bir şekilde yazmaları beklenir. Klavye sınavında başarılı olan adaylar mülakat aşamasına geçmeye hak kazanırlar. Görevleri sadece duruşma tutanaklarını yazmakla sınırlı değildir. Aynı zamanda dava dosyalarının düzenlenmesi, tebligatların hazırlanması, ara kararların yazılması ve arşivleme işlemleri de zabıt katiplerinin sorumluluğundadır. Mesleğin getirdiği yoğun stres ve dikkat gerektiren çalışma temposu, sabır ve yüksek konsantrasyon gerektirir. Hukuk diline hakim olmak, imla kurallarını kusursuz uygulamak ve resmi yazışma kurallarını bilmek mesleki başarının anahtarları arasındadır. Adliyenin işleyişi büyük ölçüde onların titiz çalışmalarına bağlıdır. Hukukun üstünlüğü ve adaletin tecellisi için arka planda gece gündüz demeden büyük bir özveriyle çalışan zabıt katipleri, yargı mekanizmasının isimsiz kahramanlarıdır."
            },
            new TypingText
            {
                Title = "Duruşma Salonunda Bir Gün",
                Category = "Hukuk",
                Difficulty = 2,
                Content = "Sabahın erken saatlerinde adliye koridorları hareketlenmeye başlar. Duruşma salonunun kapısı açıldığında zabıt katibi yerini almış, bilgisayarını açmış ve dosyaları düzenlemiştir. Hakim salona girdiğinde herkes ayağa kalkar, ardından oturularak duruşma başlar. Taraf vekilleri iddialarını sırayla açıklar, tanıklar tek tek dinlenir. Katip, konuşulan her cümleyi hızlı ve doğru bir şekilde tutanağa geçirmek zorundadır. Cümleler kısa tutulur, anlaşılmayan yerler nazikçe tekrar edilir. Duruşma sırasında teknik terimler, tarihler, dosya numaraları ve isimler sık sık geçer. Bu bilgilerin yanlış yazılması ileride ciddi hukuki sorunlara yol açabileceği için katibin dikkati hiçbir an gevşememelidir. Tutanak, tarafların beyanlarını eksiksiz yansıtmalı ve duruşmanın akışını kronolojik olarak takip etmelidir. Hakim ara karar verdiğinde katip bunu derhal kaydeder ve gerekli evrakları hazırlar. Duruşma sona erdiğinde tutanak çıktısı alınır, ilgililer tarafından imzalanır ve dosyaya eklenir. Gün içinde birden fazla duruşmaya girilmesi, katibin farklı dosyalar arasında hızla geçiş yapmasını gerektirir. Bu tempoya alışmak zaman alır; ancak düzenli çalışma ve klavye pratiği ile kısa sürede ustalaşılır. Adliyenin sakin görünen koridorlarında aslında büyük bir emek ve dikkat süreci işlemektedir."
            },
            new TypingText
            {
                Title = "Hukuk Dilinde Doğru Yazım",
                Category = "Genel",
                Difficulty = 3,
                Content = "Hukuk dili, günlük konuşma dilinden farklı olarak kendine özgü terimler, kalıplaşmış ifadeler ve uzun cümleler içerir. Resmi yazışmalarda açık, anlaşılır ve kurallara uygun bir üslup kullanmak büyük önem taşır. Noktalama işaretlerinin doğru yerde kullanılması, cümlenin anlamını doğrudan etkilediği için ihmal edilmemelidir. Büyük harflerin kullanımı, kısaltmaların yazımı ve sayıların gösterimi konusunda Türk Dil Kurumunun kurallarına uyulmalıdır. Özellikle tarih, saat ve para birimlerinin yazımında yapılan küçük hatalar, belgenin güvenilirliğini zedeler. Yazım sırasında sözcüklerin doğru yazılışına dikkat etmek, imla denetiminden geçirmek ve gerektiğinde sözlük kullanmak iyi bir alışkanlıktır. Klavye kullanımında on parmak tekniği, hem hızı hem de doğruluğu artırır. Ellerin doğru tuşlarda konumlanması, gözlerin ekranda kalması ve düzenli mola verilmesi uzun vadede daha az hata yapılmasını sağlar. Hız kadar doğruluk da önemlidir; yanlış yazılmış bir kelimeyi düzeltmek için harcanan zaman, yavaş ama dikkatli yazmaktan daha fazla olabilir. Bu nedenle sınavlara hazırlanan adayların hem hız hem de doğruluk üzerine ayrı ayrı çalışması önerilir. Günlük kısa pratikler, düzenli tekrar ve gerçek sınav koşullarında deneme yapmak başarının en güvenilir yoludur."
            });

        await db.SaveChangesAsync();
    }
}
