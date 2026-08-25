using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class BolumEkleEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color KoyuMavi = ColorTranslator.FromHtml("#0284C7");

        TextBox txtBolum;

        public BolumEkleEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
        }

        private void EkraniKur()
        {
            Label lblTitle = new Label
            {
                Text = "Yeni Bölüm / Klinik Ekle",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Hastaneye yeni bir poliklinik veya uzmanlık alanı eklemek için bölüm adını giriniz.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(40, 80)
            };
            this.Controls.Add(lblSubtitle);

            Panel pnlForm = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(500, 250),
                BackColor = SafBeyaz
            };
            pnlForm.Paint += (s, e) => 
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalKose(pnlForm.Width - 1, pnlForm.Height - 1, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    e.Graphics.DrawPath(pen, path);
            };
            pnlForm.Resize += (s, e) => 
            {
                using (GraphicsPath path = OvalKose(pnlForm.Width, pnlForm.Height, 15))
                    pnlForm.Region = new Region(path);
            };
            this.Controls.Add(pnlForm);

            Label lblBolum = new Label { Text = "Bölüm Adı:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(30, 60), AutoSize = true };
            txtBolum = new TextBox { Location = new Point(150, 57), Size = new Size(300, 30), Font = new Font("Segoe UI", 11) };
            pnlForm.Controls.Add(lblBolum);
            pnlForm.Controls.Add(txtBolum);

            Button btnEkle = new Button
            {
                Text = "➕ Bölümü Ekle",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(150, 140),
                BackColor = KoyuMavi,
                ForeColor = SafBeyaz,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEkle.FlatAppearance.BorderSize = 0;
            btnEkle.Click += BtnEkle_Click;
            pnlForm.Controls.Add(btnEkle);
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBolum.Text))
            {
                MessageBox.Show("Lütfen eklenecek bölüm adını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlHelper db = new SqlHelper();
                
                var param = new Dictionary<string, object>
                {
                    { "@bolum_adi", txtBolum.Text.Trim() }
                };

                db.ExecuteNonQuery("sp_BolumEkle", param);

                MessageBox.Show($"'{txtBolum.Text}' isimli bölüm hastaneye başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBolum.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bölüm eklenirken bir hata oluştu:\n\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

