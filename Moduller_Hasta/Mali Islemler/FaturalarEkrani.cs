using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class FaturalarEkrani : UserControl
    {
        Color AnaZemin   = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz   = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark   = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted  = ColorTranslator.FromHtml("#64748B");
        Color FaturaYes  = ColorTranslator.FromHtml("#059669");
        Color FaturaAc   = ColorTranslator.FromHtml("#34D399");

        Panel pnlGrid, pnlOzet;
        Label lblTitle, lblToplamBakiye, lblOdenenTopla, lblBekleyenTopla;
        DataGridView dgvFaturalar;

        public FaturalarEkrani()
        {
            this.BackColor = AnaZemin; this.Dock = DockStyle.Fill; this.DoubleBuffered = true;
            EkraniKur(); this.Load += (s, e) => VerileriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlGrid == null) return;
            int margin = 40, spacing = 20;
            int w = this.ClientSize.Width - margin * 2;
            int h = this.ClientSize.Height - 100;
            int ozetH = 130;
            pnlOzet.Bounds = new Rectangle(margin, 80, w, ozetH);
            pnlGrid.Bounds = new Rectangle(margin, 80 + ozetH + spacing, w, h - ozetH - spacing);
            if (dgvFaturalar != null) dgvFaturalar.Bounds = new Rectangle(0, 50, w, h - ozetH - spacing - 50);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Faturalarım", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(40, 30) };
            this.Controls.Add(lblTitle);

            // Özet Kartları Paneli
            pnlOzet = new Panel { BackColor = Color.Transparent };
            this.Controls.Add(pnlOzet);

            Panel kart1 = OzetKartiOlustur("💰  Toplam Bakiye", "₺0.00", FaturaYes, out lblToplamBakiye, 0);
            Panel kart2 = OzetKartiOlustur("✅  Ödenen",        "₺0.00", ColorTranslator.FromHtml("#3B82F6"), out lblOdenenTopla, 1);
            Panel kart3 = OzetKartiOlustur("⏳  Bekleyen",      "₺0.00", ColorTranslator.FromHtml("#F59E0B"), out lblBekleyenTopla, 2);
            pnlOzet.Controls.Add(kart1);
            pnlOzet.Controls.Add(kart2);
            pnlOzet.Controls.Add(kart3);
            pnlOzet.Resize += (s, e) =>
            {
                int cw = (pnlOzet.Width - 40) / 3;
                kart1.Bounds = new Rectangle(0, 0, cw, 130);
                kart2.Bounds = new Rectangle(cw + 20, 0, cw, 130);
                kart3.Bounds = new Rectangle((cw + 20) * 2, 0, cw, 130);
            };

            // Fatura Listesi
            pnlGrid = new Panel { BackColor = SafBeyaz };
            pnlGrid.Resize += (s, e) => OvalKirp(pnlGrid, 16);
            pnlGrid.Paint  += (s, e) => InceCerceveCiz(pnlGrid, e.Graphics, 16);
            this.Controls.Add(pnlGrid);
            pnlGrid.Controls.Add(new Label { Text = "Fatura Listesi", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 15), AutoSize = true });

            dgvFaturalar = new DataGridView
            {
                BackgroundColor = SafBeyaz, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#D1FAE5"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = FaturaYes, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, RowHeadersVisible = false,
                RowTemplate = { Height = 46 }, AllowUserToAddRows = false, ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvFaturalar.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                string col = dgvFaturalar.Columns[e.ColumnIndex].Name.ToLower();
                if (col.Contains("durum"))
                {
                    string v = e.Value?.ToString() ?? "";
                    if (v.ToLower().Contains("öden")) e.CellStyle.ForeColor = FaturaYes;
                    else if (v.ToLower().Contains("bekle")) e.CellStyle.ForeColor = ColorTranslator.FromHtml("#F59E0B");
                }
            };
            pnlGrid.Controls.Add(dgvFaturalar);
            OnResize(EventArgs.Empty);
        }

        private Panel OzetKartiOlustur(string baslik, string deger, Color renk, out Label degerLabel, int index)
        {
            Panel pnl = new Panel { BackColor = SafBeyaz };
            pnl.Resize += (s, e) => OvalKirp(pnl, 16);
            pnl.Paint  += (s, e) =>
            {
                InceCerceveCiz(pnl, e.Graphics, 16);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush b = new SolidBrush(Color.FromArgb(20, renk.R, renk.G, renk.B)))
                    e.Graphics.FillRectangle(b, new Rectangle(0, 0, 8, pnl.Height));
            };
            Panel ikonPnl = new Panel { Location = new Point(20, 35), Size = new Size(50, 50), BackColor = Color.Transparent };
            ikonPnl.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush bg = new SolidBrush(Color.FromArgb(25, renk.R, renk.G, renk.B)))
                    e.Graphics.FillEllipse(bg, 0, 0, 49, 49);
            };
            pnl.Controls.Add(ikonPnl);
            pnl.Controls.Add(new Label { Text = baslik, Font = new Font("Segoe UI", 10), ForeColor = TextMuted, Location = new Point(80, 30), AutoSize = true });
            degerLabel = new Label { Text = deger, Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = renk, Location = new Point(80, 55), AutoSize = true };
            pnl.Controls.Add(degerLabel);
            return pnl;
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } };
                dgvFaturalar.DataSource = db.GetTable("sp_FaturalariGetir", prm);
            }
            catch (Exception ex) { MessageBox.Show("Faturalar yüklenemedi: " + ex.Message); }
        }

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
