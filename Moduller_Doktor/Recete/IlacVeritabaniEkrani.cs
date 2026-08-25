using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Doktor.Recete
{
    public partial class IlacVeritabaniEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color MedikalMavi = ColorTranslator.FromHtml("#4318FF");

        Panel pnlMain, pnlArama;
        DataGridView dgvIlaclar;
        TextBox txtArama;
        DataTable dtIlaclar;

        public IlacVeritabaniEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniInsaEt();
            this.Load += (s, e) => VerileriYukle();
        }

        // TAŞMA SORUNUNU ÇÖZEN DİNAMİK BOYUTLANDIRMA
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlMain == null) return;

            int margin = 30;
            pnlMain.Bounds = new Rectangle(margin, 80, this.ClientSize.Width - (margin * 2), this.ClientSize.Height - 80 - margin);
            dgvIlaclar.Bounds = new Rectangle(20, 80, pnlMain.Width - 40, pnlMain.Height - 100);
        }

        private void EkraniInsaEt()
        {
            Label lblTitle = new Label { Text = "Sistem İlaç Veritabanı", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, 30) };
            this.Controls.Add(lblTitle);

            pnlMain = new Panel { BackColor = SafBeyaz };
            pnlMain.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(pnlMain.Width, pnlMain.Height, 20))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                { pnlMain.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };

            Label lblAra = new Label { Text = "İlaç Adı ile Ara:", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextMuted, AutoSize = true, Location = new Point(20, 30) };
            pnlMain.Controls.Add(lblAra);

            // Modern Arama Kutusu (Panel içine alınmış TextBox)
            pnlArama = new Panel { Location = new Point(150, 25), Size = new Size(300, 35), BackColor = SafBeyaz, Padding = new Padding(5) };
            pnlArama.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(pnlArama.Width, pnlArama.Height, 10))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1))
                { pnlArama.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };

            txtArama = new TextBox { Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.None, Dock = DockStyle.Fill, BackColor = SafBeyaz };
            txtArama.TextChanged += TxtArama_TextChanged;
            pnlArama.Controls.Add(txtArama);
            pnlMain.Controls.Add(pnlArama);

            dgvIlaclar = new DataGridView
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
            pnlMain.Controls.Add(dgvIlaclar);
            this.Controls.Add(pnlMain);
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                dtIlaclar = db.GetTable("sp_IlaclariGetir", new Dictionary<string, object>());
                dgvIlaclar.DataSource = dtIlaclar;
                if (dgvIlaclar.Columns.Count > 0) dgvIlaclar.Columns["ilac_ID"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("İlaçlar yüklenemedi: " + ex.Message); }
        }

        private void TxtArama_TextChanged(object sender, EventArgs e)
        {
            if (dtIlaclar != null)
            {
                DataView dv = dtIlaclar.DefaultView;
                dv.RowFilter = $"[İlaç Adı] LIKE '%{txtArama.Text}%'";
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