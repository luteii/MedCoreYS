using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class KullaniciSilEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color Kirmizi = ColorTranslator.FromHtml("#EF4444");

        TextBox txtTc;

        public KullaniciSilEkrani()
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
                Text = "Kullanıcı Sil",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Sistemden kaydı silinecek kullanıcının TC Kimlik Numarasını giriniz.",
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

            Label lblTc = new Label { Text = "TC Kimlik No:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(30, 40), AutoSize = true };
            txtTc = new TextBox { Location = new Point(150, 37), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), MaxLength = 11 };
            txtTc.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            pnlForm.Controls.Add(lblTc);
            pnlForm.Controls.Add(txtTc);

            Label lblUyari = new Label
            {
                Text = "⚠️ Dikkat: Bu işlem geri alınamaz. Kullanıcı silindiğinde sisteme girişi tamamen engellenecektir.",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Kirmizi,
                AutoSize = true,
                Location = new Point(30, 90),
                MaximumSize = new Size(420, 0)
            };
            pnlForm.Controls.Add(lblUyari);

            Button btnSil = new Button
            {
                Text = "🗑️ Kaydı Sil",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(150, 150),
                BackColor = Kirmizi,
                ForeColor = SafBeyaz,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSil.FlatAppearance.BorderSize = 0;
            btnSil.Click += BtnSil_Click;
            pnlForm.Controls.Add(btnSil);
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTc.Text))
            {
                MessageBox.Show("Lütfen silmek istediğiniz kişinin TC Kimlik Numarasını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult onay = MessageBox.Show(txtTc.Text + " TC numaralı kullanıcıyı silmek istediğinize emin misiniz?", "Kullanıcı Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (onay == DialogResult.Yes)
            {
                try
                {
                    SqlHelper db = new SqlHelper();
                    
                    // Önce TC'den kullanici_id'yi buluyoruz
                    var tcParam = new Dictionary<string, object> { { "@tc_no", txtTc.Text.Trim() } };
                    object idObj = db.ExecuteScalar("sp_KullaniciIdGetirByTc", tcParam);
                    
                    if (idObj == null)
                    {
                        MessageBox.Show("Girdiğiniz TC Kimlik Numarasına ait bir kullanıcı bulunamadı!", "Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    int kullaniciId = Convert.ToInt32(idObj);

                    var param = new Dictionary<string, object>
                    {
                        { "@kullanici_id", kullaniciId }
                    };

                    db.ExecuteNonQuery("sp_KullaniciSil", param);

                    MessageBox.Show("Kullanıcı sistemden başarıyla silindi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtTc.Clear();
                }
                catch (System.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 547)
                {
                    // Foreign Key ihlali
                    MessageBox.Show("Bu kullanıcıya bağlı randevu, hasta, log veya işlem kayıtları bulunduğu için kullanıcı sistemden TAMAMEN silinemez.\n\nSistemin veri bütünlüğünün bozulmaması için bu silme işlemi engellendi.", "Silme İşlemi Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    // SQL Server exception numbers for Foreign Key constraint violation is 547.
                    // But if Microsoft.Data.SqlClient is used, it might be different, though both usually have Number = 547.
                    if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("FOREIGN KEY"))
                    {
                        MessageBox.Show("Bu kullanıcıya bağlı randevu, hasta veya işlem kayıtları bulunduğu için kullanıcı sistemden silinemez.", "Silme İşlemi Engellendi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı silinirken bir hata oluştu:\n\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

