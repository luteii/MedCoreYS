using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class BolumleriListeleEkrani : UserControl
    {
        DataGridView dgvBolumler;
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color AcikMavi = ColorTranslator.FromHtml("#E0F2FE");
        Color KoyuMavi = ColorTranslator.FromHtml("#0284C7");

        public BolumleriListeleEkrani()
        {
            this.Size = new Size(1000, 700);
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
        }

        private void EkraniKur()
        {
            Label lblTitle = new Label
            {
                Text = "Bölüm / Klinik Listesi",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Hastanede kayıtlı olan tüm poliklinikler ve bölümler burada yer alır.",
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

            // DataGridView Kurulumu (Placeholder)
            dgvBolumler = new DataGridView
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvBolumler.ColumnHeadersDefaultCellStyle.BackColor = AcikMavi;
            dgvBolumler.ColumnHeadersDefaultCellStyle.ForeColor = KoyuMavi;
            dgvBolumler.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvBolumler.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvBolumler.ColumnHeadersHeight = 45;

            dgvBolumler.DefaultCellStyle.BackColor = SafBeyaz;
            dgvBolumler.DefaultCellStyle.ForeColor = TextDark;
            dgvBolumler.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvBolumler.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F1F5F9");
            dgvBolumler.DefaultCellStyle.SelectionForeColor = KoyuMavi;
            dgvBolumler.RowTemplate.Height = 40;
            dgvBolumler.GridColor = ColorTranslator.FromHtml("#E2E8F0");

            dgvBolumler.DataBindingComplete += DgvBolumler_DataBindingComplete;
            pnlGridKapsayici.Controls.Add(dgvBolumler);

            if (!this.DesignMode)
            {
                BolumleriGetir();
            }
        }

        private void DgvBolumler_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvBolumler.Columns.Count > 0)
            {
                dgvBolumler.Columns[0].Width = 150; // Bölüm ID
                if (dgvBolumler.Columns.Count > 1)
                {
                    dgvBolumler.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Bölüm Adı
                }
            }
            dgvBolumler.ClearSelection();
        }

        private void BolumleriGetir()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                dgvBolumler.DataSource = db.GetTable("sp_BolumleriGetir");
                
                if (dgvBolumler.Columns.Count > 0)
                {
                    dgvBolumler.Columns[0].HeaderText = "Bölüm ID";
                    dgvBolumler.Columns[1].HeaderText = "Bölüm Adı";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bölüm listesi yükleme hatası: " + ex.Message);
            }
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

