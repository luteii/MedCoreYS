using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class RecetelerEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color HastaMavi = ColorTranslator.FromHtml("#3B82F6");
        Color MorTon    = ColorTranslator.FromHtml("#8B5CF6");

        Panel       pnlGrid, pnlDetay;
        Label       lblTitle, lblDetayBaslik, lblIlaclar, lblDoktor, lblTarih;
        DataGridView dgvReceteler;
        Panel       pnlIlacKutu;

        public RecetelerEkrani()
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
            if (pnlGrid == null) return;
            int margin = 40, spacing = 20;
            int w = this.ClientSize.Width - margin * 2;
            int h = this.ClientSize.Height - 100;
            int gridW = (int)(w * 0.55), detayW = w - gridW - spacing;
            pnlGrid.Bounds  = new Rectangle(margin, 80, gridW, h);
            pnlDetay.Bounds = new Rectangle(margin + gridW + spacing, 80, detayW, h);
            if (dgvReceteler != null) dgvReceteler.Bounds = new Rectangle(0, 50, gridW, h - 50);
            if (pnlIlacKutu != null) pnlIlacKutu.Size = new Size(detayW - 50, 300);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Reçetelerim", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(40, 30) };
            this.Controls.Add(lblTitle);

            // Sol Grid
            pnlGrid = new Panel { BackColor = SafBeyaz };
            pnlGrid.Resize += (s, e) => OvalKirp(pnlGrid, 16);
            pnlGrid.Paint  += (s, e) => InceCerceveCiz(pnlGrid, e.Graphics, 16);
            this.Controls.Add(pnlGrid);
            pnlGrid.Controls.Add(new Label { Text = "Tüm Reçetelerim", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 15), AutoSize = true });

            dgvReceteler = ModernGridOlustur(HastaMavi);
            dgvReceteler.CellClick += DgvReceteler_CellClick;
            pnlGrid.Controls.Add(dgvReceteler);

            // Sağ Detay
            pnlDetay = new Panel { BackColor = SafBeyaz };
            pnlDetay.Resize += (s, e) => OvalKirp(pnlDetay, 16);
            pnlDetay.Paint  += (s, e) => InceCerceveCiz(pnlDetay, e.Graphics, 16);
            this.Controls.Add(pnlDetay);

            // Reçete ikonu
            Panel pnlIkon = new Panel { Location = new Point(25, 25), Size = new Size(50, 50), BackColor = Color.Transparent };
            pnlIkon.Paint += (s, e) => { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using (SolidBrush b = new SolidBrush(Color.FromArgb(30, 139, 92, 246))) e.Graphics.FillEllipse(b, 0, 0, 49, 49); TextRenderer.DrawText(e.Graphics, "💊", new Font("Segoe UI Emoji", 18), new Rectangle(0, 0, 50, 50), MorTon, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter); };
            pnlDetay.Controls.Add(pnlIkon);

            lblDetayBaslik = new Label { Text = "Reçete Detayı", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = MorTon, Location = new Point(85, 35), AutoSize = true };
            pnlDetay.Controls.Add(lblDetayBaslik);

            Panel ayrac = new Panel { Location = new Point(25, 85), Size = new Size(350, 2), BackColor = ColorTranslator.FromHtml("#E2E8F0") };
            pnlDetay.Controls.Add(ayrac);

            Label lbl1 = new Label { Text = "Yazan Doktor:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, 100), AutoSize = true };
            pnlDetay.Controls.Add(lbl1);
            lblDoktor = new Label { Text = "--", Font = new Font("Segoe UI", 12), ForeColor = TextDark, Location = new Point(25, 118), AutoSize = true };
            pnlDetay.Controls.Add(lblDoktor);

            Label lbl2 = new Label { Text = "Tarih:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, 150), AutoSize = true };
            pnlDetay.Controls.Add(lbl2);
            lblTarih = new Label { Text = "--", Font = new Font("Segoe UI", 12), ForeColor = TextDark, Location = new Point(25, 168), AutoSize = true };
            pnlDetay.Controls.Add(lblTarih);

            Label lbl3 = new Label { Text = "İlaçlar:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, 205), AutoSize = true };
            pnlDetay.Controls.Add(lbl3);

            pnlIlacKutu = new Panel { Location = new Point(25, 225), Size = new Size(350, 300), BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            pnlIlacKutu.Paint += (s, e) => { using (GraphicsPath p = TamKoseOval(pnlIlacKutu.Width, pnlIlacKutu.Height, 10)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1)) { pnlIlacKutu.Region = new Region(p); e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; e.Graphics.DrawPath(pen, p); } };
            lblIlaclar = new Label { Text = "Listeden bir reçete seçin...", Font = new Font("Segoe UI", 11), ForeColor = TextMuted, Location = new Point(15, 15), Size = new Size(320, 270), AutoSize = false };
            pnlIlacKutu.Controls.Add(lblIlaclar);
            pnlDetay.Controls.Add(pnlIlacKutu);

            OnResize(EventArgs.Empty);
        }

        private void DgvReceteler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvReceteler.Rows[e.RowIndex];
            try
            {
                lblDetayBaslik.Text = "Reçete #" + row.Cells[0].Value?.ToString();
                if (row.Cells.Count > 2) lblDoktor.Text = row.Cells[2].Value?.ToString() ?? "--";
                if (row.Cells.Count > 1) lblTarih.Text  = row.Cells[1].Value?.ToString() ?? "--";
                if (row.Cells.Count > 3) lblIlaclar.Text = row.Cells[3].Value?.ToString() ?? "--";
            }
            catch { }
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } };
                dgvReceteler.DataSource = db.GetTable("sp_ReceteleriGetir", prm);
                if (dgvReceteler.Columns.Count > 0)
                    dgvReceteler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex) { MessageBox.Show("Reçeteler yüklenemedi: " + ex.Message); }
        }

        private DataGridView ModernGridOlustur(Color headerColor) => new DataGridView
        {
            BackgroundColor = SafBeyaz, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#DBEAFE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = headerColor, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
            EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, RowHeadersVisible = false, RowTemplate = { Height = 46 },
            AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
