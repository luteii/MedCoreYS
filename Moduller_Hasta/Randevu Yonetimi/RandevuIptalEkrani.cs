using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class RandevuIptalEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color HastaMavi = ColorTranslator.FromHtml("#3B82F6");
        Color KirmiziAl = ColorTranslator.FromHtml("#EF4444");
        Color KirmiziAc = ColorTranslator.FromHtml("#FCA5A5");

        Panel       pnlGrid, pnlDetay;
        Label       lblTitle, lblDetayBaslik, lblDetayTarih, lblDetayDoktor;
        DataGridView dgvBekleyen;
        int         seciliRandevuID = 0;

        public RandevuIptalEkrani()
        {
            this.BackColor    = AnaZemin;
            this.Dock         = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
            this.Load += (s, e) => VerileriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlGrid == null || pnlDetay == null) return;
            int margin = 40; int spacing = 20;
            int w = this.ClientSize.Width - margin * 2;
            int h = this.ClientSize.Height - 100;
            int gridW = (int)(w * 0.60); int detayW = w - gridW - spacing;
            pnlGrid.Bounds  = new Rectangle(margin, 80, gridW, h);
            pnlDetay.Bounds = new Rectangle(margin + gridW + spacing, 80, detayW, h);
            if (dgvBekleyen != null) dgvBekleyen.Bounds = new Rectangle(0, 50, gridW, h - 50);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Randevu İptal Et", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(40, 30) };
            this.Controls.Add(lblTitle);

            // Sol — Bekleyen Randevular Listesi
            pnlGrid = new Panel { BackColor = SafBeyaz };
            pnlGrid.Resize += (s, e) => OvalKirp(pnlGrid, 16);
            pnlGrid.Paint  += (s, e) => InceCerceveCiz(pnlGrid, e.Graphics, 16);
            this.Controls.Add(pnlGrid);
            pnlGrid.Controls.Add(new Label { Text = "Aktif Randevularım", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 15), AutoSize = true });

            dgvBekleyen = ModernGridOlustur(HastaMavi);
            pnlGrid.Controls.Add(dgvBekleyen);
            dgvBekleyen.CellClick += DgvBekleyen_CellClick;

            // Sağ — Detay Kartı
            pnlDetay = new Panel { BackColor = SafBeyaz };
            pnlDetay.Resize += (s, e) => OvalKirp(pnlDetay, 16);
            pnlDetay.Paint  += (s, e) => InceCerceveCiz(pnlDetay, e.Graphics, 16);
            this.Controls.Add(pnlDetay);

            lblDetayBaslik = new Label { Text = "Randevu Seçin", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = HastaMavi, Location = new Point(25, 30), AutoSize = true };
            pnlDetay.Controls.Add(lblDetayBaslik);

            Panel ayrac = new Panel { Location = new Point(25, 60), Size = new Size(350, 2), BackColor = ColorTranslator.FromHtml("#E2E8F0") };
            pnlDetay.Controls.Add(ayrac);

            Label lbl1 = new Label { Text = "Doktor:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, 80), AutoSize = true };
            pnlDetay.Controls.Add(lbl1);
            lblDetayDoktor = new Label { Text = "--", Font = new Font("Segoe UI", 12), ForeColor = TextDark, Location = new Point(25, 98), AutoSize = true };
            pnlDetay.Controls.Add(lblDetayDoktor);

            Label lbl2 = new Label { Text = "Tarih:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, 135), AutoSize = true };
            pnlDetay.Controls.Add(lbl2);
            lblDetayTarih = new Label { Text = "--", Font = new Font("Segoe UI", 12), ForeColor = TextDark, Location = new Point(25, 153), AutoSize = true };
            pnlDetay.Controls.Add(lblDetayTarih);

            // Uyarı Kutusu
            Panel pnlUyari = new Panel { Location = new Point(25, 210), Size = new Size(350, 70), BackColor = ColorTranslator.FromHtml("#FEF2F2") };
            pnlUyari.Paint += (s, e) => { using (GraphicsPath p = TamKoseOval(pnlUyari.Width, pnlUyari.Height, 10)) using (Pen pen = new Pen(KirmiziAc, 1)) { pnlUyari.Region = new Region(p); e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; e.Graphics.DrawPath(pen, p); } };
            Label lblUyari = new Label { Text = "⚠  İptal işlemi geri alınamaz.\nRandevudan 24 saat önce iptal edilebilir.", Font = new Font("Segoe UI", 9), ForeColor = KirmiziAl, Location = new Point(15, 12), Size = new Size(320, 50) };
            pnlUyari.Controls.Add(lblUyari);
            pnlDetay.Controls.Add(pnlUyari);

            // İptal Butonu
            Button btnIptal = new Button { Text = "🚫  Randevuyu İptal Et", Location = new Point(25, 300), Size = new Size(350, 50), Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, ForeColor = SafBeyaz, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btnIptal.FlatAppearance.BorderSize = 0;
            btnIptal.Paint += (s, e) =>
            {
                Button b = (Button)s; bool hov = b.ClientRectangle.Contains(b.PointToClient(System.Windows.Forms.Cursor.Position));
                Rectangle r = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                using (GraphicsPath path = TamKoseOval(b.Width, b.Height, 12))
                using (SolidBrush br = new SolidBrush(hov ? KirmiziAc : KirmiziAl))
                { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; b.Region = new Region(path); e.Graphics.FillPath(br, path); }
                TextRenderer.DrawText(e.Graphics, b.Text, b.Font, r, b.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            btnIptal.MouseEnter += (s, e) => btnIptal.Invalidate();
            btnIptal.MouseLeave += (s, e) => btnIptal.Invalidate();
            btnIptal.Click += BtnIptal_Click;
            pnlDetay.Controls.Add(btnIptal);

            OnResize(EventArgs.Empty);
        }

        private void DgvBekleyen_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvBekleyen.Rows[e.RowIndex];
            try
            {
                seciliRandevuID = Convert.ToInt32(row.Cells[0].Value);
                lblDetayDoktor.Text = row.Cells.Count > 2 ? row.Cells[2].Value?.ToString() ?? "--" : "--";
                lblDetayTarih.Text  = row.Cells.Count > 3 ? row.Cells[3].Value?.ToString() ?? "--" : "--";
                lblDetayBaslik.Text = "Randevu Detayı";
            }
            catch { }
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } };
                dgvBekleyen.DataSource = db.GetTable("sp_RandevulariGetir", prm);
                if (dgvBekleyen.Columns.Count > 0)
                    dgvBekleyen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Randevular yüklenemedi: " + ex.Message);
            }
        }

        private void BtnIptal_Click(object sender, EventArgs e)
        {
            if (seciliRandevuID == 0) { MessageBox.Show("Lütfen önce bir randevu seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("Bu randevuyu iptal etmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SqlHelper db = new SqlHelper();
                    Dictionary<string, object> prm = new Dictionary<string, object> { { "@randevu_id", seciliRandevuID } };
                    db.ExecuteNonQuery("sp_RandevuSil", prm);
                    MessageBox.Show("Randevunuz iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    seciliRandevuID = 0; lblDetayBaslik.Text = "Randevu Seçin"; lblDetayDoktor.Text = "--"; lblDetayTarih.Text = "--";
                    VerileriYukle();
                }
                catch (Exception ex) { MessageBox.Show("İptal işlemi başarısız: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private DataGridView ModernGridOlustur(Color headerColor)
        {
            return new DataGridView
            {
                BackgroundColor = SafBeyaz, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#DBEAFE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = headerColor, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, RowHeadersVisible = false, RowTemplate = { Height = 46 },
                AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
        }

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
