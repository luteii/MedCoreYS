using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class LogKayitlariEkrani : UserControl
    {
        DataGridView dgvLoglar;
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color AcikMavi = ColorTranslator.FromHtml("#E0F2FE");
        Color KoyuMavi = ColorTranslator.FromHtml("#0284C7");

        public LogKayitlariEkrani()
        {
            this.Size = new Size(1000, 700);
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
            this.Load += (s, e) => LoglariYukle();
        }

        private void EkraniKur()
        {
            // Başlık
            Label lblTitle = new Label
            {
                Text = "Sistem Log Kayıtları",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            // Alt Bilgi
            Label lblSubtitle = new Label
            {
                Text = "Sistemde yapılan tüm kullanıcı girişleri ve önemli işlemler burada listelenir.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(40, 80)
            };
            this.Controls.Add(lblSubtitle);

            // Grid Kapsayıcı Panel
            Panel pnlGridKapsayici = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(840, 560),
                BackColor = SafBeyaz,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right
            };
            pnlGridKapsayici.Paint += (s, e) => 
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalKose(pnlGridKapsayici.Width - 1, pnlGridKapsayici.Height - 1, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    e.Graphics.DrawPath(pen, path);
            };
            pnlGridKapsayici.Resize += (s, e) => 
            {
                if (pnlGridKapsayici.Width > 0 && pnlGridKapsayici.Height > 0)
                {
                    using (GraphicsPath path = OvalKose(pnlGridKapsayici.Width, pnlGridKapsayici.Height, 15))
                        pnlGridKapsayici.Region = new Region(path);
                }
            };
            this.Controls.Add(pnlGridKapsayici);

            // Yenile Butonu
            Button btnYenile = new Button
            {
                Text = "🔄 Kayıtları Yenile",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(160, 35),
                Location = new Point(pnlGridKapsayici.Width - 180, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = KoyuMavi,
                ForeColor = SafBeyaz,
                Cursor = Cursors.Hand
            };
            btnYenile.FlatAppearance.BorderSize = 0;
            btnYenile.Click += (s, e) => LoglariYukle();
            this.Controls.Add(btnYenile);

            // DataGridView Kurulumu
            dgvLoglar = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(810, 530),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                BackgroundColor = SafBeyaz,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ScrollBars = ScrollBars.Both
            };
            dgvLoglar.DataBindingComplete += DgvLoglar_DataBindingComplete;

            // Modern Grid Stilleri
            dgvLoglar.ColumnHeadersDefaultCellStyle.BackColor = AcikMavi;
            dgvLoglar.ColumnHeadersDefaultCellStyle.ForeColor = KoyuMavi;
            dgvLoglar.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvLoglar.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvLoglar.ColumnHeadersHeight = 45;

            dgvLoglar.DefaultCellStyle.BackColor = SafBeyaz;
            dgvLoglar.DefaultCellStyle.ForeColor = TextDark;
            dgvLoglar.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvLoglar.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F1F5F9");
            dgvLoglar.DefaultCellStyle.SelectionForeColor = KoyuMavi;
            dgvLoglar.RowTemplate.Height = 40;
            dgvLoglar.GridColor = ColorTranslator.FromHtml("#E2E8F0");

            pnlGridKapsayici.Controls.Add(dgvLoglar);
        }

        private void LoglariYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                DataTable dt = db.GetTable("sp_LoglariGetir", null);
                dgvLoglar.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log kayıtları çekilirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DgvLoglar_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvLoglar.Columns.Count > 0)
            {
                if (dgvLoglar.Columns.Contains("Kayıt No")) dgvLoglar.Columns["Kayıt No"].Width = 80;
                if (dgvLoglar.Columns.Contains("TC Kimlik")) dgvLoglar.Columns["TC Kimlik"].Width = 120;
                if (dgvLoglar.Columns.Contains("Kullanıcı")) dgvLoglar.Columns["Kullanıcı"].Width = 150;
                if (dgvLoglar.Columns.Contains("Rol")) dgvLoglar.Columns["Rol"].Width = 100;
                if (dgvLoglar.Columns.Contains("İşlem Tipi")) dgvLoglar.Columns["İşlem Tipi"].Width = 120;
                if (dgvLoglar.Columns.Contains("Tarih")) dgvLoglar.Columns["Tarih"].Width = 150;

                // Açıklama kolonu kalan tüm alanı kaplasın
                if (dgvLoglar.Columns.Contains("Açıklama")) 
                    dgvLoglar.Columns["Açıklama"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            dgvLoglar.ClearSelection();
        }

        private GraphicsPath OvalKose(int width, int height, int radius)
        {
            radius = Math.Min(radius, Math.Min(width / 2, height / 2));
            if (radius <= 0) radius = 1;
            GraphicsPath path = new GraphicsPath();
            float c = radius * 2F;
            path.StartFigure();
            path.AddArc(0, 0, c, c, 180, 90);
            path.AddArc(width - c, 0, c, c, 270, 90);
            path.AddArc(width - c, height - c, c, c, 0, 90);
            path.AddArc(0, height - c, c, c, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

