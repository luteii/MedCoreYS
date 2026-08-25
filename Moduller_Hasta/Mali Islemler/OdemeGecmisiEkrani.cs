using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class OdemeGecmisiEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color OdemeYes  = ColorTranslator.FromHtml("#059669");

        Panel pnlGrid; Label lblTitle; DataGridView dgvOdemeler;

        public OdemeGecmisiEkrani()
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
            if (dgvOdemeler != null) dgvOdemeler.Bounds = new Rectangle(0, 50, pnlGrid.Width, h - 50);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Ödeme Geçmişi", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(40, 30) };
            this.Controls.Add(lblTitle);

            pnlGrid = new Panel { BackColor = SafBeyaz };
            pnlGrid.Resize += (s, e) => OvalKirp(pnlGrid, 16);
            pnlGrid.Paint  += (s, e) => InceCerceveCiz(pnlGrid, e.Graphics, 16);
            this.Controls.Add(pnlGrid);
            pnlGrid.Controls.Add(new Label { Text = "💳  Yapılan Ödemeler", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = OdemeYes, Location = new Point(20, 15), AutoSize = true });

            dgvOdemeler = new DataGridView
            {
                BackgroundColor = SafBeyaz, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#D1FAE5"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = OdemeYes, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, RowHeadersVisible = false,
                RowTemplate = { Height = 46 }, AllowUserToAddRows = false, ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            pnlGrid.Controls.Add(dgvOdemeler);
            OnResize(EventArgs.Empty);
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } };
                dgvOdemeler.DataSource = db.GetTable("sp_OdemeleriGetir", prm);
            }
            catch (Exception ex) { MessageBox.Show("Ödemeler yüklenemedi: " + ex.Message); }
        }

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
