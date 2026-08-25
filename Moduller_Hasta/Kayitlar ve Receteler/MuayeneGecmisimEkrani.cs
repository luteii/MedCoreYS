using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class MuayeneGecmisimEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color HastaMavi = ColorTranslator.FromHtml("#3B82F6");

        Panel pnlGrid, pnlDetay;
        Label lblTitle, lblDetayBaslik, lblSikayet, lblTeshis, lblNotlar;
        DataGridView dgvMuayeneler;

        public MuayeneGecmisimEkrani()
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
            if (dgvMuayeneler != null) dgvMuayeneler.Bounds = new Rectangle(0, 50, gridW, h - 50);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Muayene Geçmişim", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(40, 30) };
            this.Controls.Add(lblTitle);

            pnlGrid = new Panel { BackColor = SafBeyaz };
            pnlGrid.Resize += (s, e) => OvalKirp(pnlGrid, 16);
            pnlGrid.Paint  += (s, e) => InceCerceveCiz(pnlGrid, e.Graphics, 16);
            this.Controls.Add(pnlGrid);
            pnlGrid.Controls.Add(new Label { Text = "Geçmiş Muayeneler", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 15), AutoSize = true });

            dgvMuayeneler = new DataGridView
            {
                BackgroundColor = SafBeyaz, BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#DBEAFE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = HastaMavi, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 50, RowHeadersVisible = false,
                RowTemplate = { Height = 46 }, AllowUserToAddRows = false, ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvMuayeneler.CellClick += DgvMuayeneler_CellClick;
            pnlGrid.Controls.Add(dgvMuayeneler);

            // Sağ Detay
            pnlDetay = new Panel { BackColor = SafBeyaz };
            pnlDetay.Resize += (s, e) => OvalKirp(pnlDetay, 16);
            pnlDetay.Paint  += (s, e) => InceCerceveCiz(pnlDetay, e.Graphics, 16);
            this.Controls.Add(pnlDetay);

            lblDetayBaslik = new Label { Text = "Muayene Detayı", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = HastaMavi, Location = new Point(25, 25), AutoSize = true };
            pnlDetay.Controls.Add(lblDetayBaslik);

            pnlDetay.Controls.Add(new Panel { Location = new Point(25, 58), Size = new Size(350, 2), BackColor = ColorTranslator.FromHtml("#E2E8F0") });

            pnlDetay.Controls.Add(new Label { Text = "Şikayet:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, 75), AutoSize = true });
            lblSikayet = new Label { Text = "--", Font = new Font("Segoe UI", 11), ForeColor = TextDark, Location = new Point(25, 93), Size = new Size(350, 60), AutoSize = false };
            pnlDetay.Controls.Add(lblSikayet);

            pnlDetay.Controls.Add(new Label { Text = "Teşhis:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, 165), AutoSize = true });
            lblTeshis = new Label { Text = "--", Font = new Font("Segoe UI", 11), ForeColor = TextDark, Location = new Point(25, 183), Size = new Size(350, 60), AutoSize = false };
            pnlDetay.Controls.Add(lblTeshis);

            pnlDetay.Controls.Add(new Label { Text = "Notlar:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(25, 255), AutoSize = true });
            lblNotlar = new Label { Text = "--", Font = new Font("Segoe UI", 11), ForeColor = TextDark, Location = new Point(25, 273), Size = new Size(350, 100), AutoSize = false };
            pnlDetay.Controls.Add(lblNotlar);

            OnResize(EventArgs.Empty);
        }

        private void DgvMuayeneler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvMuayeneler.Rows[e.RowIndex];
            try
            {
                lblSikayet.Text = row.Cells["Şikayet"].Value?.ToString() ?? "--";
                lblTeshis.Text  = row.Cells["Teşhis"].Value?.ToString() ?? "--";
                lblNotlar.Text  = row.Cells["Notlar"].Value?.ToString() ?? "--";
                lblDetayBaslik.Text = "Muayene #" + row.Cells["Kayıt No"].Value?.ToString();
            }
            catch { }
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } };
                dgvMuayeneler.DataSource = db.GetTable("sp_HastaGecmisMuayeneleriGetir", prm);
                if (dgvMuayeneler.Columns.Contains("Şikayet")) dgvMuayeneler.Columns["Şikayet"].Visible = false;
                if (dgvMuayeneler.Columns.Contains("Teşhis")) dgvMuayeneler.Columns["Teşhis"].Visible = false;
                if (dgvMuayeneler.Columns.Contains("Notlar")) dgvMuayeneler.Columns["Notlar"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Muayene geçmişi yüklenemedi: " + ex.Message); }
        }

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
