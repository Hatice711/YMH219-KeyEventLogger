# YMH219 - Flappy Bird

Nesne Tabanlı Programlama (YMH 219) dersi kapsamında C# ve Windows Forms ile geliştirilmiş Flappy Bird benzeri bir masaüstü oyunudur. Oyuncu, kuşu klavye ile zıplatarak borular arasındaki boşluklardan geçirmeye çalışır.

## Geliştirici
- **Ad Soyad:** Hatice Kırmızıgül
- **Öğrenci No:** [Öğrenci Numaran]

## Kullanılan Teknolojiler
- C#
- .NET 10 (Windows Forms)
- GDI+ (`System.Drawing`) ile çizim
- Visual Studio
- Harici NuGet paketi kullanılmamıştır.

## Kurulum ve Çalıştırma
1. Depoyu klonlayın:
   `git clone https://github.com/Hatice711/YMH219-FlappyBird.git`
2. `FlappyBird.slnx` dosyasını Visual Studio ile açın.
3. **F5** tuşuna basarak projeyi derleyip çalıştırın.

> Gereksinim: Windows işletim sistemi ve .NET 10 SDK.

## Kontroller
| Tuş | İşlev |
|---|---|
| `Space` / `Yukarı Ok` | Zıpla / Oyunu başlat |
| `R` | Game Over ekranında yeniden başlat |
| **Yeni Oyun** düğmesi | Oyunu yeniden başlat |

## Özellikler
- Yer çekimi etkisi ve klavye ile zıplama
- Sağdan sola hareket eden, ekran dışına çıkınca yeniden konumlanan 3 boru çifti
- Rastgele yükseklikte boru boşlukları
- Zemin, tavan ve borularla çarpışma kontrolü, Game Over ekranı
- Geçilen her boru için skor artışı
- Yeni Oyun düğmesi ve R tuşu ile yeniden başlatma
- En yüksek skorun dosyada (`highscore.txt`) saklanması
- Her 5 puanda bir hızın artması (seviye sistemi)

## Sınıf Yapısı
| Sınıf | Görevi |
|---|---|
| `Bird` | Kuşun konumu, hızı, yer çekimi, zıplama ve çizimi |
| `Pipe` | Boru çiftinin konumu, hareketi, yeniden konumlanması ve çarpışma kontrolü |
| `GameState` | Oyun durumu (Waiting / Playing / GameOver), skor, hız, seviye ve rekor bilgisi |
| `HighScoreManager` | En yüksek skorun dosyadan okunması ve dosyaya yazılması |
| `Form1` | Oyun döngüsü (Timer), klavye olayları ve ekrana çizim |

## Ekran Görüntüleri
### Başlangıç Ekranı
![Başlangıç ekranı](FlappyBird/screenshots/baslangic.png)

### Oyun Sırasında
![Oyun ekranı](FlappyBird/screenshots/oyun.png)

### Game Over
![Game Over ekranı](FlappyBird/screenshots/gameover.png)

## Bilinen Sınırlamalar / Geliştirme Notları
- Oyun grafikleri hazır görseller yerine GDI+ ile basit şekiller kullanılarak çizilmiştir.
- Ses efekti bulunmamaktadır.
- Pencere boyutu sabittir (400x600).
- Çarpışma kontrolü dikdörtgen (AABB) tabanlıdır; kuşun yuvarlak şeklinin köşelerinde küçük sapmalar olabilir.
