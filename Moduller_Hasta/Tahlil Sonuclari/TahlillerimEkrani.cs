using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class TahlillerimEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color TahlilMor = ColorTranslator.FromHtml("#7C3AED");
        Color TahlilAc  = ColorTranslator.FromHtml("#A78BFA");

        Panel pnlGrid; Label lblTitle; DataGridView dgvTahliller;

        public TahlillerimEkrani()
        {
            this.BackColor = AnaZemin; this.Dock = DockStyle.Fill; this.DoubleBuffered = true;
            EkraniKur(); this.Load += (s, e) => VerileriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlGrid == null) return;
            int margin = 40, h = this.ClientSize.Height - 100;
            pnlGrid.Bounds = new Rectangle(margin, 80, this.ClientSize.Width - margin * 2, h);
            if (dgvTahliller != null) dgvTahliller.Bounds = new Rectangle(0, 55, pnlGrid.Width, h - 55);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Tahlil Sonuçlarım", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(40, 30) };
            this.Controls.Add(lblTitle);

            pnlGrid = new Panel { BackColor = SafBeyaz };
            pnlGrid.Resize += (s, e) => OvalKirp(pnlGrid, 16);
            pnlGrid.Paint  += (s, e) => InceCerceveCiz(pnlGrid, e.Graphics, 16);
            this.Controls.Add(pnlGrid);

            Label lbl = new Label { Text = "⚗  Tahlil Listesi", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TahlilMor, Location = new Point(20, 15), AutoSize = true };
            pnlGrid.Controls.Add(lbl);

            dgvTahliller = new DataGridView
            {
                BackgroundColor = SafBeyaz, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#EDE9FE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = TahlilMor, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, RowHeadersVisible = false,
                RowTemplate = { Height = 46 }, AllowUserToAddRows = false, ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            // Sonuç durumuna göre renk
            dgvTahliller.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                string col = dgvTahliller.Columns[e.ColumnIndex].Name;
                if (col.ToLower().Contains("durum") || col.ToLower().Contains("sonuc"))
                {
                    string v = e.Value?.ToString() ?? "";
                    if (v.ToLower().Contains("normal")) e.CellStyle.ForeColor = ColorTranslator.FromHtml("#10B981");
                    else if (v.ToLower().Contains("yüksek") || v.ToLower().Contains("dusuk")) e.CellStyle.ForeColor = ColorTranslator.FromHtml("#EF4444");
                    else if (v.ToLower().Contains("bekle")) e.CellStyle.ForeColor = ColorTranslator.FromHtml("#F59E0B");
                }
            };
            pnlGrid.Controls.Add(dgvTahliller);
            OnResize(EventArgs.Empty);
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } };
                dgvTahliller.DataSource = db.GetTable("sp_TahlilleriGetir", prm);
            }
            catch (Exception ex) { MessageBox.Show("Tahliller yüklenemedi: " + ex.Message); }
        }

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
