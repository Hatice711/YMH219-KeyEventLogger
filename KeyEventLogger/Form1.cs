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
            ClientSize = new Size(640, 480);
            MinimumSize = new Size(600, 400);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);
            KeyPreview = true;   // tuş olayları önce forma gelsin

            SetupButton(btnStart, "Başlat", 12, BtnStart_Click);
            SetupButton(btnStop, "Durdur", 122, BtnStop_Click);
            SetupButton(btnClear, "Temizle", 232, BtnClear_Click);
            SetupButton(btnSave, "Kaydet (.txt)", 342, BtnSave_Click);

            // Kayıt durumu göstergesi (sağ üst)
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblStatus.Location = new Point(ClientSize.Width - 170, 20);
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Kayıt listesi
            lstLog.Location = new Point(12, 60);
            lstLog.Size = new Size(ClientSize.Width - 24, ClientSize.Height - 120);
            lstLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstLog.Font = new Font("Consolas", 10);
            lstLog.IntegralHeight = false;

            // Alt bilgi satırları
            lblCount.AutoSize = true;
            lblCount.Location = new Point(12, ClientSize.Height - 52);
            lblCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            lblInfo.AutoSize = true;
            lblInfo.ForeColor = Color.Gray;
            lblInfo.Text = "Not: Tuşlar yalnızca bu pencere aktifken kaydedilir.";
            lblInfo.Location = new Point(12, ClientSize.Height - 28);
            lblInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            Controls.AddRange(new Control[] { btnStart, btnStop, btnClear, btnSave, lblStatus, lstLog, lblCount, lblInfo });

            // Klavye ve pencere olayları
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            Deactivate += Form1_Deactivate;
        }

        private void SetupButton(Button button, string text, int x, EventHandler onClick)
        {
            button.Text = text;
            button.Location = new Point(x, 12);
            button.Size = new Size(100, 36);
            button.Click += onClick;
        }

        // Düğmelerin aktifliğini, göstergeyi ve sayacı günceller
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

            lblCount.Text = $"Toplam kayıt: {logger.TotalCount}";
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