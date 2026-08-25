using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Laborant
{
    public partial class BekleyenTahlillerEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F8FAFC");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color MedikalTeal = ColorTranslator.FromHtml("#0D9488");
        Color BasariYesili = ColorTranslator.FromHtml("#10B981");

        Panel pnlListe, pnlDetay, pnlInfoCard;
        DataGridView dgvBekleyenler;
        TextBox txtSonuc;
        Button btnKaydet;
        
        Label lblDetayHastaAdi, lblDetayDoktorAdi, lblDetayTarih, lblDetayTahlilAdi, lblDetayReferans;

        int seciliSonucId = -1;

        public BekleyenTahlillerEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;

            EkraniInsaEt();
            this.Load += (s, e) => TahlilleriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlListe == null || pnlDetay == null) return;

            int margin = 30;
            int spacing = 20;
            int w = this.ClientSize.Width - (margin * 2);
            int h = this.ClientSize.Height - 80 - margin;

            int solGenislik = w - 450 - spacing; // Grew right panel for info card
            int sagGenislik = 450;

            if (solGenislik < 300) solGenislik = 300;

            pnlListe.Bounds = new Rectangle(margin, 80, solGenislik, h);
            pnlDetay.Bounds = new Rectangle(margin + solGenislik + spacing, 80, sagGenislik, h);

            if (dgvBekleyenler != null) dgvBekleyenler.Bounds = new Rectangle(20, 60, solGenislik - 40, h - 80);
            if (pnlInfoCard != null) pnlInfoCard.Bounds = new Rectangle(20, 60, sagGenislik - 40, 180);
            if (txtSonuc != null) txtSonuc.Bounds = new Rectangle(20, 290, sagGenislik - 40, h - 380);
            if (btnKaydet != null) btnKaydet.Bounds = new Rectangle(20, h - 70, sagGenislik - 40, 50);
        }

        private void EkraniInsaEt()
        {
            Label lblTitle = new Label { Text = "Bekleyen Tahliller", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, 30) };
            this.Controls.Add(lblTitle);

            // SOL PANEL (Liste)
            pnlListe = OlusturPanel();
            pnlListe.Controls.Add(new Label { Text = "İncelenmesi Beklenen Tahlil İstekleri", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true });

            dgvBekleyenler = new DataGridView
            {
                BackgroundColor = SafBeyaz,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#CCFBF1"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 10) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = SafBeyaz, ForeColor = TextMuted, Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                ColumnHeadersHeight = 35,
                RowHeadersVisible = false,
                RowTemplate = { Height = 40 },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                MultiSelect = false
            };
            dgvBekleyenler.SelectionChanged += DgvBekleyenler_SelectionChanged;
            pnlListe.Controls.Add(dgvBekleyenler);
            this.Controls.Add(pnlListe);

            // SAĞ PANEL (Detay ve Sonuç Girme)
            pnlDetay = OlusturPanel();
            pnlDetay.Controls.Add(new Label { Text = "Tahlil Detayları & Sonuç Girişi", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true });

            // BİLGİ KARTI (Info Card)
            pnlInfoCard = new Panel { BackColor = ColorTranslator.FromHtml("#F1F5F9") };
            pnlInfoCard.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(pnlInfoCard.Width, pnlInfoCard.Height, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1))
                { pnlInfoCard.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };

            lblDetayHastaAdi = InfoLabelOlustur("👤 Hasta:", "Seçiniz...", 15);
            lblDetayDoktorAdi = InfoLabelOlustur("👨‍⚕️ Doktor:", "Seçiniz...", 45);
            lblDetayTarih = InfoLabelOlustur("📅 Tarih:", "Seçiniz...", 75);
            lblDetayTahlilAdi = InfoLabelOlustur("🧪 Tahlil:", "Seçiniz...", 105);
            lblDetayReferans = InfoLabelOlustur("📈 Referans:", "Seçiniz...", 135);

            pnlInfoCard.Controls.Add(lblDetayHastaAdi);
            pnlInfoCard.Controls.Add(lblDetayDoktorAdi);
            pnlInfoCard.Controls.Add(lblDetayTarih);
            pnlInfoCard.Controls.Add(lblDetayTahlilAdi);
            pnlInfoCard.Controls.Add(lblDetayReferans);
            
            pnlDetay.Controls.Add(pnlInfoCard);

            pnlDetay.Controls.Add(new Label { Text = "Tahlil Sonucu:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(20, 260), AutoSize = true });
            
            txtSonuc = new TextBox
            {
                Multiline = true,
                Font = new Font("Segoe UI", 12),
                BackColor = SafBeyaz,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlDetay.Controls.Add(txtSonuc);

            btnKaydet = ButonOlustur("Sonucu Kaydet", BasariYesili, new Point(20, 100), 200);
            btnKaydet.Click += BtnKaydet_Click;
            pnlDetay.Controls.Add(btnKaydet);
            
            this.Controls.Add(pnlDetay);
        }

        private Label InfoLabelOlustur(string title, string value, int y)
        {
            Label lbl = new Label();
            lbl.Text = $"{title} {value}";
            lbl.Font = new Font("Segoe UI", 10);
            lbl.ForeColor = TextDark;
            lbl.Location = new Point(15, y);
            lbl.AutoSize = true;
            return lbl;
        }

        private void SetInfoLabel(Label lbl, string title, string value)
        {
            lbl.Text = $"{title} {value}";
            
            // Eğer referans yazısıysa ve uzunsa sarma yapsın diye AutoSize iptal edilebilir 
            // ama şimdilik AutoSize = true kalıyor, basit string interpolasyonu ile kalın başlık görünümü de verebiliriz.
        }

        private void TahlilleriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                DataTable dt = db.GetTable("sp_BekleyenTahlilleriGetir");
                dgvBekleyenler.DataSource = dt;
                
                if(dt != null && dt.Columns.Contains("sonuc_id"))
                    dgvBekleyenler.Columns["sonuc_id"].Visible = false;

                seciliSonucId = -1;
                txtSonuc.Clear();
                SifirlaDetaylar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tahliller yüklenemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SifirlaDetaylar()
        {
            SetInfoLabel(lblDetayHastaAdi, "👤 Hasta:", "Seçiniz...");
            SetInfoLabel(lblDetayDoktorAdi, "👨‍⚕️ Doktor:", "Seçiniz...");
            SetInfoLabel(lblDetayTarih, "📅 Tarih:", "Seçiniz...");
            SetInfoLabel(lblDetayTahlilAdi, "🧪 Tahlil:", "Seçiniz...");
            SetInfoLabel(lblDetayReferans, "📈 Referans:", "Seçiniz...");
        }

        private void DgvBekleyenler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBekleyenler.SelectedRows.Count > 0)
            {
                var row = dgvBekleyenler.SelectedRows[0];
                seciliSonucId = Convert.ToInt32(row.Cells["sonuc_id"].Value);
                
                SetInfoLabel(lblDetayHastaAdi, "👤 Hasta:", row.Cells["HastaAdi"].Value.ToString());
                SetInfoLabel(lblDetayDoktorAdi, "👨‍⚕️ Doktor:", row.Cells["DoktorAdi"].Value.ToString());
                SetInfoLabel(lblDetayTarih, "📅 Tarih:", Convert.ToDateTime(row.Cells["IstenmeTarihi"].Value).ToString("dd.MM.yyyy HH:mm"));
                SetInfoLabel(lblDetayTahlilAdi, "🧪 Tahlil:", row.Cells["TahlilAdi"].Value.ToString());
                SetInfoLabel(lblDetayReferans, "📈 Referans:", row.Cells["ReferansAraligi"].Value?.ToString() ?? "Belirtilmemiş");

                txtSonuc.Clear();
            }
            else
            {
                seciliSonucId = -1;
                SifirlaDetaylar();
            }
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (seciliSonucId == -1)
            {
                MessageBox.Show("Lütfen sonuç girmek istediğiniz tahlili listeden seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSonuc.Text))
            {
                MessageBox.Show("Tahlil sonucu boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object>
                {
                    { "@sonuc_id", seciliSonucId },
                    { "@sonuc_degeri", txtSonuc.Text }
                };

                db.ExecuteNonQuery("sp_TahlilSonucuGuncelle", prm);
                MessageBox.Show("Tahlil sonucu başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                TahlilleriYukle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydetme işlemi sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel OlusturPanel()
        {
            Panel p = new Panel { BackColor = SafBeyaz };
            p.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(p.Width, p.Height, 20))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                { p.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };
            return p;
        }

        private Button ButonOlustur(string text, Color bg, Point loc, int w)
        {
            Button b = new Button { Text = text, BackColor = bg, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = loc, Size = new Size(w, 45), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            b.Paint += (s, e) => {
                using (GraphicsPath path = OvalPath(b.Width, b.Height, 10)) { b.Region = new Region(path); }
            };
            return b;
        }

        private GraphicsPath OvalPath(int w, int h, int r)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, r, r, 180, 90); path.AddArc(w - r, 0, r, r, 270, 90);
            path.AddArc(w - r, h - r, r, r, 0, 90); path.AddArc(0, h - r, r, r, 90, 90);
            path.CloseFigure(); return path;
        }
    }
}
