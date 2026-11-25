namespace KeyEventLogger
{
    // Kayıt durumunu ve tuş kayıtlarının listesini yöneten sınıf
    public class KeyLogger
    {
        private readonly List<KeyLogEntry> entries = new List<KeyLogEntry>();
        private readonly HashSet<Keys> heldKeys = new HashSet<Keys>();   // şu an basılı tutulan tuşlar

        public bool IsRecording { get; private set; } = false;
        public IReadOnlyList<KeyLogEntry> Entries => entries;
        public int TotalCount => entries.Count;

        public void Start()
        {
            IsRecording = true;
        }

        public void Stop()
        {
            IsRecording = false;
            heldKeys.Clear();
        }

        public void Clear()
        {
            entries.Clear();
        }

        // Tuşa basıldığında çağrılır. Kayıt eklenirse kaydı, eklenmezse null döndürür.
        public KeyLogEntry? RecordKeyDown(Keys keyCode, Keys modifiers)
        {
            if (!IsRecording)
                return null;

            // Tuş basılı tutulunca KeyDown tekrar tekrar gelir; aynı basışı bir kez say
            if (!heldKeys.Add(keyCode))
                return null;

            KeyLogEntry entry = new KeyLogEntry(DateTime.Now, FormatKey(keyCode, modifiers));
            entries.Add(entry);
            return entry;
        }

        // Tuş bırakıldığında çağrılır
        public void RecordKeyUp(Keys keyCode)
        {
            heldKeys.Remove(keyCode);
        }

        // Pencere odağı kaybedince basılı tuş bilgisi sıfırlanır
        public void ReleaseAllKeys()
        {
            heldKeys.Clear();
        }

        // En sık basılan tuşları (tuş adı, sayı) olarak döndürür
        public List<(string Key, int Count)> GetMostFrequentKeys(int count)
        {
            return entries
                .GroupBy(e => e.KeyName)                       // aynı tuşları grupla
                .Select(g => (Key: g.Key, Count: g.Count()))   // her grubun sayısını al
                .OrderByDescending(x => x.Count)               // çoktan aza sırala
                .Take(count)                                   // ilk 'count' tanesini al
                .ToList();
        }

        // Alt bilgi satırında gösterilecek istatistik metni
        public string GetStatisticsText()
        {
            string text = $"Toplam kayıt: {TotalCount}";

            List<(string Key, int Count)> top = GetMostFrequentKeys(3);
            if (top.Count > 0)
                text += "   |   En sık: " + string.Join(", ", top.Select(t => $"{t.Key} ({t.Count})"));

            return text;
        }

        // Tuş kodunu okunabilir bir metne çevirir: "Ctrl + Shift + A" gibi
        private static string FormatKey(Keys keyCode, Keys modifiers)
        {
            string key = keyCode switch
            {
                Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey => "Shift",
                Keys.ControlKey or Keys.LControlKey or Keys.RControlKey => "Ctrl",
                Keys.Menu or Keys.LMenu or Keys.RMenu => "Alt",
                Keys.Enter => "Enter",
                Keys.Back => "Backspace",
                Keys.Space => "Space",
                Keys.Escape => "Esc",
                >= Keys.D0 and <= Keys.D9 => ((int)keyCode - (int)Keys.D0).ToString(),
                >= Keys.NumPad0 and <= Keys.NumPad9 => "Num " + ((int)keyCode - (int)Keys.NumPad0),
                _ => keyCode.ToString()
            };

            // Basılan tuşun kendisi Shift/Ctrl/Alt ise önüne tekrar ekleme
            if (key == "Shift" || key == "Ctrl" || key == "Alt")
                return key;

            List<string> parts = new List<string>();
            if (modifiers.HasFlag(Keys.Control)) parts.Add("Ctrl");
            if (modifiers.HasFlag(Keys.Alt)) parts.Add("Alt");
            if (modifiers.HasFlag(Keys.Shift)) parts.Add("Shift");
            parts.Add(key);

            return string.Join(" + ", parts);
        }
    }
}