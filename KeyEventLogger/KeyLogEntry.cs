namespace KeyEventLogger
{
    // Tek bir tuş kaydını temsil eder: hangi tuş, ne zaman
    public class KeyLogEntry
    {
        public DateTime Time { get; }
        public string KeyName { get; }

        public KeyLogEntry(DateTime time, string keyName)
        {
            Time = time;
            KeyName = keyName;
        }

        // Ekranda ve dosyada görünecek format: [25.11.2026 15:04:12.345]  A
        public override string ToString()
        {
            return $"[{Time:dd.MM.yyyy HH:mm:ss.fff}]  {KeyName}";
        }
    }
}