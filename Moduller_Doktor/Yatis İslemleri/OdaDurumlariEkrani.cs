using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Doktor
{
    public partial class OdaDurumlariEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");

        Panel pnlMain;
        DataGridView dgvOdalar;

        public OdaDurumlariEkrani()
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
            if (pnlMain == null) return;

            int margin = 30;
            pnlMain.Bounds = new Rectangle(margin, 80, this.ClientSize.Width - (margin * 2), this.ClientSize.Height - 80 - margin);
            dgvOdalar.Bounds = new Rectangle(20, 60, pnlMain.Width - 40, pnlMain.Height - 80);
        }

        private void EkraniInsaEt()
        {
            Label lblTitle = new Label { Text = "Servis Oda Durumları", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, 30) };
            this.Controls.Add(lblTitle);

            pnlMain = new Panel { BackColor = SafBeyaz };
            pnlMain.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(pnlMain.Width, pnlMain.Height, 20))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                { pnlMain.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };

            Label lblTabloBaslik = new Label { Text = "Tüm Odaların Güncel Listesi", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(20, 20) };
            pnlMain.Controls.Add(lblTabloBaslik);

            dgvOdalar = new DataGridView
            {
                BackgroundColor = SafBeyaz,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#E0F2FE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = SafBeyaz, ForeColor = TextMuted, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                ColumnHeadersHeight = 40,
                RowHeadersVisible = false,
                RowTemplate = { Height = 45 },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // "Dolu" ve "Boş" yazılarını renklendirmek için özel event
            dgvOdalar.CellFormatting += DgvOdalar_CellFormatting;
            pnlMain.Controls.Add(dgvOdalar);
            this.Controls.Add(pnlMain);
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                dgvOdalar.DataSource = db.GetTable("sp_OdaDurumlariGetir", new Dictionary<string, object>());
            }
            catch (Exception ex) { MessageBox.Show("Odalar yüklenemedi: " + ex.Message); }
        }

        private void DgvOdalar_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvOdalar.Columns[e.ColumnIndex].Name == "Mevcut Durum" && e.Value != null)
            {
                if (e.Value.ToString() == "Dolu") e.CellStyle.ForeColor = ColorTranslator.FromHtml("#EF4444"); // Kırmızı
                else if (e.Value.ToString() == "Boş") e.CellStyle.ForeColor = ColorTranslator.FromHtml("#05CD99"); // Yeşil
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