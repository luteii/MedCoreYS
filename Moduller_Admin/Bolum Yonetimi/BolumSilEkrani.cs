using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class BolumSilEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color Kirmizi = ColorTranslator.FromHtml("#EF4444");

        ComboBox cmbBolumler;

        public BolumSilEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
            
            if (!this.DesignMode)
            {
                this.Load += (s, e) => BolumleriYukle();
            }
        }

        private void EkraniKur()
        {
            Label lblTitle = new Label
            {
                Text = "Bölüm / Klinik Sil",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Hastaneden kaldırılacak bölümü seçiniz ve onaylayınız.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(40, 80)
            };
            this.Controls.Add(lblSubtitle);

            Panel pnlForm = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(600, 250),
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

            Label lblBolum = new Label { Text = "Bölüm Seç:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(30, 60), AutoSize = true };
            
            cmbBolumler = new ComboBox 
            { 
                Location = new Point(150, 57), 
                Size = new Size(300, 30), 
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            pnlForm.Controls.Add(lblBolum);
            pnlForm.Controls.Add(cmbBolumler);

            Label lblUyari = new Label
            {
                Text = "⚠️ Dikkat: Bu işlem geri alınamaz. Eğer bu bölüme kayıtlı doktorlar\nveya hastalar varsa silme işlemi sistem tarafından engellenebilir.",
                Font = new Font("Segoe UI", 8),
                ForeColor = Kirmizi,
                AutoSize = true,
                Location = new Point(130, 100)
            };
            pnlForm.Controls.Add(lblUyari);

            Button btnSil = new Button
            {
                Text = "🗑️ Bölümü Sil",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(150, 160),
                BackColor = Kirmizi,
                ForeColor = SafBeyaz,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSil.FlatAppearance.BorderSize = 0;
            btnSil.Click += BtnSil_Click;
            pnlForm.Controls.Add(btnSil);
        }

        private void BolumleriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                DataTable dt = db.GetTable("sp_BolumleriGetir");
                
                cmbBolumler.SetDataSourceWithChooseOption(dt, "BolumAdi", "bolum_id");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bölümler yüklenirken bir hata oluştu:\n" + ex.Message, "Veri Yükleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (cmbBolumler.SelectedValue == null)
            {
                MessageBox.Show("Lütfen silinecek bölümü seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bolumId = Convert.ToInt32(cmbBolumler.SelectedValue);
            string bolumAdi = cmbBolumler.Text;

            DialogResult dr = MessageBox.Show($"'{bolumAdi}' isimli bölümü sistemden tamamen silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes) return;

            try
            {
                SqlHelper db = new SqlHelper();
                var param = new Dictionary<string, object>
                {
                    { "@bolum_id", bolumId }
                };

                db.ExecuteNonQuery("sp_BolumSil", param);

                MessageBox.Show($"'{bolumAdi}' isimli bölüm hastaneden başarıyla silindi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BolumleriYukle(); // Listeyi yenile
            }
            catch (System.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 547)
            {
                MessageBox.Show("Bu bölüme atanmış doktorlar veya randevular olduğu için bölüm silinemez. Önce ilişkili kayıtları temizlemelisiniz.", "Silme Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                {
                    MessageBox.Show("Bu bölüme atanmış doktorlar veya randevular olduğu için bölüm silinemez.", "Silme Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Bölüm silinirken bir hata oluştu:\n\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

