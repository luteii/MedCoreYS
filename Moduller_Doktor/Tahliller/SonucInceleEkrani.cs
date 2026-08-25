using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Doktor.Tahliller
{
    public partial class SonucInceleEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color MedikalMavi = ColorTranslator.FromHtml("#4318FF");
        Color BasariYesili = ColorTranslator.FromHtml("#05CD99");

        Panel pnlSol, pnlSag, pnlKart;
        DataGridView dgvBekleyen, dgvTamamlanan;
        DataTable dtBekleyen, dtTamamlanan, dtTumSonuclar;
        
        Label lblHastaAdi, lblTarih, lblTahlilAdi, lblSonuc, lblReferans;
        RichTextBox txtAciklama;
        Button btnKaydet;
        int seciliSonucId = -1;

        public SonucInceleEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniInsaEt();
            this.Load += (s, e) => VerileriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlSol == null || pnlSag == null) return;

            int margin = 30;
            int spacing = 20;
            int w = this.ClientSize.Width - (margin * 2);
            int h = this.ClientSize.Height - 80 - margin;

            int solGenislik = (int)(w * 0.45);
            int sagGenislik = w - solGenislik - spacing;

            pnlSol.Bounds = new Rectangle(margin, 80, solGenislik, h);
            pnlSag.Bounds = new Rectangle(margin + solGenislik + spacing, 80, sagGenislik, h);

            int yarimBoy = (h - 60) / 2;
            if (dgvBekleyen != null) dgvBekleyen.Bounds = new Rectangle(0, 40, solGenislik, yarimBoy);
            if (dgvTamamlanan != null) dgvTamamlanan.Bounds = new Rectangle(0, yarimBoy + 80, solGenislik, yarimBoy - 20);

            if (pnlKart != null)
            {
                pnlKart.Bounds = new Rectangle(0, 0, sagGenislik, h);
                if (txtAciklama != null)
                {
                    txtAciklama.Bounds = new Rectangle(20, 240, pnlKart.Width - 40, pnlKart.Height - 330);
                    btnKaydet.Bounds = new Rectangle(20, pnlKart.Height - 70, pnlKart.Width - 40, 50);
                }
            }
        }

        private void EkraniInsaEt()
        {
            Label lblTitle = new Label { Text = "Tahlil Sonuçları Ekranı", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, 30) };
            this.Controls.Add(lblTitle);

            // SOL PANEL (Listeler)
            pnlSol = new Panel();
            
            Label lblBTitle = new Label { Text = "Sonucu Beklenen Tahliller", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextMuted, AutoSize = true, Location = new Point(0, 10) };
            pnlSol.Controls.Add(lblBTitle);
            
            dgvBekleyen = ModernDataGridOlustur();
            dgvBekleyen.CellClick += DgvBekleyen_CellClick;
            pnlSol.Controls.Add(dgvBekleyen);

            Label lblTTitle = new Label { Text = "Sonucu Çıkan Tahliller", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextMuted, AutoSize = true, Location = new Point(0, 10) };
            lblTTitle.Paint += (s, e) => { lblTTitle.Location = new Point(0, dgvBekleyen.Bottom + 20); };
            pnlSol.Controls.Add(lblTTitle);

            dgvTamamlanan = ModernDataGridOlustur();
            dgvTamamlanan.CellClick += DgvTamamlanan_CellClick;
            pnlSol.Controls.Add(dgvTamamlanan);

            this.Controls.Add(pnlSol);

            // SAĞ PANEL (Detay Kartı)
            pnlSag = new Panel();
            pnlKart = new Panel { BackColor = SafBeyaz };
            pnlKart.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(pnlKart.Width, pnlKart.Height, 20))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                { pnlKart.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };

            Label lblKartTitle = new Label { Text = "Detaylı Tahlil Raporu", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = MedikalMavi, AutoSize = true, Location = new Point(20, 20) };
            pnlKart.Controls.Add(lblKartTitle);

            lblHastaAdi = new Label { Text = "Hasta: -", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(20, 70) };
            pnlKart.Controls.Add(lblHastaAdi);

            lblTarih = new Label { Text = "Tarih: -", Font = new Font("Segoe UI", 11), ForeColor = TextMuted, AutoSize = true, Location = new Point(20, 100) };
            pnlKart.Controls.Add(lblTarih);

            lblTahlilAdi = new Label { Text = "İstenen Tahlil: -", Font = new Font("Segoe UI", 11), ForeColor = TextMuted, AutoSize = true, Location = new Point(20, 130) };
            pnlKart.Controls.Add(lblTahlilAdi);

            lblSonuc = new Label { Text = "Sonuç: -", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(20, 170) };
            pnlKart.Controls.Add(lblSonuc);

            lblReferans = new Label { Text = "Referans Aralığı: -", Font = new Font("Segoe UI", 10, FontStyle.Italic), ForeColor = TextMuted, AutoSize = true, Location = new Point(20, 200) };
            pnlKart.Controls.Add(lblReferans);

            txtAciklama = new RichTextBox
            {
                Font = new Font("Segoe UI", 12),
                BackColor = ColorTranslator.FromHtml("#F8FAFC"),
                BorderStyle = BorderStyle.None,
                Text = "Doktor açıklaması eklemek için tıklayın..."
            };
            txtAciklama.Enter += (s, e) => { if (txtAciklama.Text == "Doktor açıklaması eklemek için tıklayın...") txtAciklama.Text = ""; };
            txtAciklama.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtAciklama.Text)) txtAciklama.Text = "Doktor açıklaması eklemek için tıklayın..."; };
            pnlKart.Controls.Add(txtAciklama);

            btnKaydet = new Button
            {
                Text = "Açıklamayı Kaydet",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SafBeyaz,
                BackColor = BasariYesili,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnKaydet.FlatAppearance.BorderSize = 0;
            btnKaydet.Click += BtnKaydet_Click;
            pnlKart.Controls.Add(btnKaydet);

            pnlSag.Controls.Add(pnlKart);
            this.Controls.Add(pnlSag);
            
            pnlKart.Visible = false; // Başlangıçta gizli olsun
        }

        private DataGridView ModernDataGridOlustur()
        {
            DataGridView dgv = new DataGridView
            {
                BackgroundColor = AnaZemin,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#E0F2FE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 10) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = AnaZemin, ForeColor = TextMuted, Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                ColumnHeadersHeight = 35,
                RowHeadersVisible = false,
                RowTemplate = { Height = 40 },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.DataBindingComplete += (s, e) => {
                if (dgv.Columns.Contains("sonuc_id")) dgv.Columns["sonuc_id"].Visible = false;
                if (dgv.Columns.Contains("Referans Aralığı")) dgv.Columns["Referans Aralığı"].Visible = false;
                if (dgv.Columns.Contains("Doktor Açıklaması")) dgv.Columns["Doktor Açıklaması"].Visible = false;
                dgv.ClearSelection();
            };
            return dgv;
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                dtTumSonuclar = db.GetTable("sp_DoktorTahlilSonuclariGetir", new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } });

                dtBekleyen = dtTumSonuclar.Clone();
                dtTamamlanan = dtTumSonuclar.Clone();

                foreach (DataRow row in dtTumSonuclar.Rows)
                {
                    if (row["Sonuç / Durum"].ToString() == "Sonuç Bekleniyor...")
                        dtBekleyen.ImportRow(row);
                    else
                        dtTamamlanan.ImportRow(row);
                }

                dgvBekleyen.DataSource = dtBekleyen;
                dgvTamamlanan.DataSource = dtTamamlanan;
            }
            catch (Exception ex) { MessageBox.Show("Sonuçlar yüklenemedi: " + ex.Message); }
        }

        private void DgvBekleyen_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) { dgvTamamlanan.ClearSelection(); DetayGoster(dgvBekleyen.Rows[e.RowIndex]); }
        }

        private void DgvTamamlanan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) { dgvBekleyen.ClearSelection(); DetayGoster(dgvTamamlanan.Rows[e.RowIndex]); }
        }

        private void DetayGoster(DataGridViewRow row)
        {
            seciliSonucId = Convert.ToInt32(row.Cells["sonuc_id"].Value);
            lblHastaAdi.Text = "Hasta: " + row.Cells["Hasta Adı"].Value.ToString();
            lblTarih.Text = "Tarih: " + row.Cells["Tarih"].Value.ToString();
            lblTahlilAdi.Text = "İstenen Tahlil: " + row.Cells["İstenen Tahlil"].Value.ToString();
            lblSonuc.Text = "Sonuç: " + row.Cells["Sonuç / Durum"].Value.ToString();
            lblReferans.Text = "Referans Aralığı: " + row.Cells["Referans Aralığı"].Value.ToString();
            
            string aciklama = row.Cells["Doktor Açıklaması"].Value.ToString();
            if (string.IsNullOrWhiteSpace(aciklama))
                txtAciklama.Text = "Doktor açıklaması eklemek için tıklayın...";
            else
                txtAciklama.Text = aciklama;

            pnlKart.Visible = true;
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (seciliSonucId == -1) return;
            string aciklama = txtAciklama.Text == "Doktor açıklaması eklemek için tıklayın..." ? "" : txtAciklama.Text;

            try
            {
                SqlHelper db = new SqlHelper();
                db.ExecuteNonQuery("sp_DoktorAciklamasiKaydet", new Dictionary<string, object>
                {
                    { "@sonuc_id", seciliSonucId },
                    { "@doktor_aciklamasi", aciklama }
                });

                MessageBox.Show("Açıklama başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                VerileriYukle();
                pnlKart.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kaydedilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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