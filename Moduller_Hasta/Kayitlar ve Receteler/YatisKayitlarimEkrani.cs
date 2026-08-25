using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class YatisKayitlarimEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color HastaMavi = ColorTranslator.FromHtml("#3B82F6");
        Color YatisRenk = ColorTranslator.FromHtml("#0EA5E9");

        Label lblTitle; Panel pnlGrid; DataGridView dgvYatislar;

        public YatisKayitlarimEkrani()
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
            if (dgvYatislar != null) dgvYatislar.Bounds = new Rectangle(0, 50, pnlGrid.Width, h - 50);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Yatış Kayıtlarım", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(40, 30) };
            this.Controls.Add(lblTitle);

            pnlGrid = new Panel { BackColor = SafBeyaz };
            pnlGrid.Resize += (s, e) => OvalKirp(pnlGrid, 16);
            pnlGrid.Paint  += (s, e) => InceCerceveCiz(pnlGrid, e.Graphics, 16);
            this.Controls.Add(pnlGrid);

            Label lbl = new Label { Text = "🛏  Hastane Yatış Geçmişi", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 15), AutoSize = true };
            pnlGrid.Controls.Add(lbl);

            dgvYatislar = new DataGridView
            {
                BackgroundColor = SafBeyaz, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#DBEAFE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = YatisRenk, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, RowHeadersVisible = false,
                RowTemplate = { Height = 46 }, AllowUserToAddRows = false, ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            pnlGrid.Controls.Add(dgvYatislar);
            OnResize(EventArgs.Empty);
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } };
                dgvYatislar.DataSource = db.GetTable("sp_YatislariGetir", prm);
            }
            catch (Exception ex) { MessageBox.Show("Yatış kayıtları yüklenemedi: " + ex.Message); }
        }

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
