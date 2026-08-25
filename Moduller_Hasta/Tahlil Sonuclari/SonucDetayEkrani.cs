using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class SonucDetayEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color TahlilMor = ColorTranslator.FromHtml("#7C3AED");

        Panel pnlGrid, pnlDetay;
        Label lblTitle, lblDetayBaslik, lblParametre, lblDeger, lblReferans, lblYorum;
        DataGridView dgvTahlilter;

        public SonucDetayEkrani()
        {
            this.BackColor = AnaZemin; this.Dock = DockStyle.Fill; this.DoubleBuffered = true;
            EkraniKur(); this.Load += (s, e) => VerileriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlGrid == null) return;
            int margin = 40, spacing = 20, w = this.ClientSize.Width - margin * 2, h = this.ClientSize.Height - 100;
            int gridW = (int)(w * 0.55), detayW = w - gridW - spacing;
            pnlGrid.Bounds  = new Rectangle(margin, 80, gridW, h);
            pnlDetay.Bounds = new Rectangle(margin + gridW + spacing, 80, detayW, h);
            if (dgvTahlilter != null) dgvTahlilter.Bounds = new Rectangle(0, 50, gridW, h - 50);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Sonuç Detayları", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(40, 30) };
            this.Controls.Add(lblTitle);

            pnlGrid = new Panel { BackColor = SafBeyaz };
            pnlGrid.Resize += (s, e) => OvalKirp(pnlGrid, 16);
            pnlGrid.Paint  += (s, e) => InceCerceveCiz(pnlGrid, e.Graphics, 16);
            this.Controls.Add(pnlGrid);
            pnlGrid.Controls.Add(new Label { Text = "Tahlil Listesi", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 15), AutoSize = true });

            dgvTahlilter = new DataGridView
            {
                BackgroundColor = SafBeyaz, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#EDE9FE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = TahlilMor, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, RowHeadersVisible = false,
                RowTemplate = { Height = 46 }, AllowUserToAddRows = false, ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvTahlilter.CellClick += DgvTahlilter_CellClick;
            pnlGrid.Controls.Add(dgvTahlilter);

            // Detay Paneli
            pnlDetay = new Panel { BackColor = SafBeyaz };
            pnlDetay.Resize += (s, e) => OvalKirp(pnlDetay, 16);
            pnlDetay.Paint  += (s, e) => InceCerceveCiz(pnlDetay, e.Graphics, 16);
            this.Controls.Add(pnlDetay);

            lblDetayBaslik = new Label { Text = "Sonuç Detayı", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = TahlilMor, Location = new Point(25, 25), AutoSize = true };
            pnlDetay.Controls.Add(lblDetayBaslik);
            pnlDetay.Controls.Add(new Panel { Location = new Point(25, 58), Size = new Size(350, 2), BackColor = ColorTranslator.FromHtml("#E2E8F0") });

            string[] etiketler = { "Parametre:", "Ölçülen Değer:", "Referans Aralığı:", "Yorum:" };
            int[] yPoslar = { 75, 145, 215, 285 };

            for (int i = 0; i < etiketler.Length; i++)
                pnlDetay.Controls.Add(new Label { Text = etiketler[i], Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, yPoslar[i]), AutoSize = true });

            lblParametre = new Label { Text = "--", Font = new Font("Segoe UI", 12), ForeColor = TextDark, Location = new Point(25, 93), Size = new Size(350, 40), AutoSize = false };
            lblDeger     = new Label { Text = "--", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = TahlilMor, Location = new Point(25, 163), Size = new Size(350, 40), AutoSize = false };
            lblReferans  = new Label { Text = "--", Font = new Font("Segoe UI", 11), ForeColor = TextDark, Location = new Point(25, 233), Size = new Size(350, 40), AutoSize = false };
            lblYorum     = new Label { Text = "--", Font = new Font("Segoe UI", 11), ForeColor = TextDark, Location = new Point(25, 303), Size = new Size(350, 120), AutoSize = false };

            pnlDetay.Controls.Add(lblParametre);
            pnlDetay.Controls.Add(lblDeger);
            pnlDetay.Controls.Add(lblReferans);
            pnlDetay.Controls.Add(lblYorum);

            OnResize(EventArgs.Empty);
        }

        private void DgvTahlilter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvTahlilter.Rows[e.RowIndex];
            try
            {
                lblDetayBaslik.Text = "Sonuç #" + row.Cells[0].Value?.ToString();
                if (row.Cells.Count > 1) lblParametre.Text = row.Cells[1].Value?.ToString() ?? "--";
                if (row.Cells.Count > 2) lblDeger.Text     = row.Cells[2].Value?.ToString() ?? "--";
                if (row.Cells.Count > 3) lblReferans.Text  = row.Cells[3].Value?.ToString() ?? "--";
                if (row.Cells.Count > 4) lblYorum.Text     = row.Cells[4].Value?.ToString() ?? "--";

                // Değer rengi
                string deger = lblDeger.Text;
                if (deger.ToLower().Contains("normal")) lblDeger.ForeColor = ColorTranslator.FromHtml("#10B981");
                else if (deger.ToLower().Contains("yüksek")) lblDeger.ForeColor = ColorTranslator.FromHtml("#EF4444");
                else lblDeger.ForeColor = TahlilMor;
            }
            catch { }
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } };
                dgvTahlilter.DataSource = db.GetTable("sp_TahlilSonuclariGetir", prm);
            }
            catch (Exception ex) { MessageBox.Show("Tahlil sonuçları yüklenemedi: " + ex.Message); }
        }

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
