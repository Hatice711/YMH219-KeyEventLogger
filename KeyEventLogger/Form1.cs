namespace KeyEventLogger
{
    public partial class Form1 : Form
    {
        private readonly KeyLogger logger = new KeyLogger();
        private readonly LogFileService fileService = new LogFileService();

        // Arayüz bileşenleri
        private readonly Button btnStart = new Button();
        private readonly Button btnStop = new Button();
        private readonly Button btnClear = new Button();
        private readonly Button btnSave = new Button();
        private readonly Label lblStatus = new Label();
        private readonly Label lblCount = new Label();
        private readonly Label lblInfo = new Label();
        private readonly ListBox lstLog = new ListBox();

        public Form1()
        {
            InitializeComponent();
            BuildUi();
            UpdateUi();
        }

        // Tüm kontrolleri oluşturup forma yerleştirir
        private void BuildUi()
        {
            Text = "Key Event Logger - YMH219";
            ClientSize = new Size(680, 480);
            MinimumSize = new Size(640, 400);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);
            Padding = new Padding(8);
            KeyPreview = true;   // tuş olayları önce forma gelsin

            // Üst çubuk: düğmeler ve durum göstergesi soldan sağa yan yana dizilir.
            // FlowLayoutPanel konumları kendisi hesapladığı için ekran ölçeğinden etkilenmez.
            FlowLayoutPanel topBar = new FlowLayoutPanel();
            topBar.Dock = DockStyle.Top;
            topBar.AutoSize = true;
            topBar.WrapContents = false;
            topBar.Padding = new Padding(0, 0, 0, 6);

            SetupButton(btnStart, "Başlat", BtnStart_Click);
            SetupButton(btnStop, "Durdur", BtnStop_Click);
            SetupButton(btnClear, "Temizle", BtnClear_Click);
            SetupButton(btnSave, "Kaydet (.txt)", BtnSave_Click);

            // Kayıt durumu göstergesi (düğmelerin sağında)
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblStatus.Margin = new Padding(16, 8, 0, 0);

            topBar.Controls.AddRange(new Control[] { btnStart, btnStop, btnClear, btnSave, lblStatus });

            // Alt çubuk: istatistik ve bilgi satırı alt alta
            FlowLayoutPanel bottomBar = new FlowLayoutPanel();
            bottomBar.Dock = DockStyle.Bottom;
            bottomBar.AutoSize = true;
            bottomBar.FlowDirection = FlowDirection.TopDown;
            bottomBar.WrapContents = false;
            bottomBar.Padding = new Padding(0, 6, 0, 0);

            lblCount.AutoSize = true;

            lblInfo.AutoSize = true;
            lblInfo.ForeColor = Color.Gray;
            lblInfo.Text = "Not: Tuşlar yalnızca bu pencere aktifken kaydedilir.";

            bottomBar.Controls.AddRange(new Control[] { lblCount, lblInfo });

            // Kayıt listesi: üst ve alt çubuk arasında kalan alanı doldurur
            lstLog.Dock = DockStyle.Fill;
            lstLog.Font = new Font("Consolas", 10);
            lstLog.IntegralHeight = false;

            Controls.Add(lstLog);
            Controls.Add(topBar);
            Controls.Add(bottomBar);
            lstLog.BringToFront();   // Fill olan kontrol en son yerleşsin, çubukların altında kalmasın

            // Klavye ve pencere olayları
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            Deactivate += Form1_Deactivate;
        }

        private void SetupButton(Button button, string text, EventHandler onClick)
        {
            button.Text = text;
            button.AutoSize = true;                    // yazı sığmazsa düğme genişlesin
            button.MinimumSize = new Size(100, 36);
            button.Click += onClick;
        }

        // Düğmelerin aktifliğini, göstergeyi ve istatistikleri günceller
        private void UpdateUi()
        {
            bool recording = logger.IsRecording;
            bool hasEntries = logger.TotalCount > 0;

            btnStart.Enabled = !recording;
            btnStop.Enabled = recording;
            btnClear.Enabled = hasEntries;
            btnSave.Enabled = hasEntries;

            lblStatus.Text = recording ? "● KAYIT AÇIK" : "● KAYIT KAPALI";
            lblStatus.ForeColor = recording ? Color.ForestGreen : Color.Crimson;

            lblCount.Text = logger.GetStatisticsText();
        }

        // Odağı listeye ver: böylece Space/Enter yanlışlıkla bir düğmeye basmaz
        private void FocusList()
        {
            ActiveControl = lstLog;
        }

        private void BtnStart_Click(object? sender, EventArgs e)
        {
            logger.Start();
            UpdateUi();
            FocusList();
        }

        private void BtnStop_Click(object? sender, EventArgs e)
        {
            logger.Stop();
            UpdateUi();
            FocusList();
        }

        private void BtnClear_Click(object? sender, EventArgs e)
        {
            DialogResult answer = MessageBox.Show(
                "Ekrandaki tüm kayıtlar silinsin mi?", "Temizle",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer == DialogResult.Yes)
            {
                logger.Clear();
                lstLog.Items.Clear();
            }

            UpdateUi();
            FocusList();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            using SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Kayıtları Kaydet";
            dialog.Filter = "Metin dosyası (*.txt)|*.txt";
            dialog.FileName = $"keylog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    fileService.Save(dialog.FileName, logger.Entries);
                    MessageBox.Show($"{logger.TotalCount} kayıt dosyaya kaydedildi:\n{dialog.FileName}",
                        "Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    MessageBox.Show("Dosya kaydedilemedi:\n" + ex.Message,
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            FocusList();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            KeyLogEntry? entry = logger.RecordKeyDown(e.KeyCode, e.Modifiers);

            if (entry != null)
            {
                lstLog.Items.Add(entry);                  // ListBox, ToString() sonucunu gösterir
                lstLog.TopIndex = lstLog.Items.Count - 1;   // en son kayda kaydır
                UpdateUi();
            }
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            logger.RecordKeyUp(e.KeyCode);
        }

        // Pencere arka plana geçince (başka uygulamaya geçilince)
        private void Form1_Deactivate(object? sender, EventArgs e)
        {
            logger.ReleaseAllKeys();
        }
    }
}