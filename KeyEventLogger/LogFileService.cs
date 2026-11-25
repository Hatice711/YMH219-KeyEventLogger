using System.Text;

namespace KeyEventLogger
{
    // Kayıtları metin dosyasına yazmaktan sorumlu sınıf
    public class LogFileService
    {
        public void Save(string filePath, IEnumerable<KeyLogEntry> entries)
        {
            // using: iş bitince dosya otomatik kapatılır (hata olsa bile)
            using StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);

            writer.WriteLine("Key Event Logger - Kayıt Dosyası");
            writer.WriteLine($"Oluşturulma: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            writer.WriteLine(new string('-', 45));

            int count = 0;
            foreach (KeyLogEntry entry in entries)
            {
                writer.WriteLine(entry.ToString());
                count++;
            }

            writer.WriteLine(new string('-', 45));
            writer.WriteLine($"Toplam kayıt: {count}");
        }
    }
}