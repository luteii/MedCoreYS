using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class YetkilendirmeEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color KoyuMavi = ColorTranslator.FromHtml("#0284C7");

        TextBox txtTc;
        ComboBox cmbRol;

        public YetkilendirmeEkrani()
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
                Text = "Yetkilendirme Ayarları",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Kullanıcıların sisteme erişim rollerini (Doktor, Sekreter, Admin) buradan değiştirebilirsiniz.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(40, 80)
            };
            this.Controls.Add(lblSubtitle);

            Panel pnlForm = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(500, 300),
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

            Label lblRol = new Label { Text = "Yeni Rol:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(30, 90), AutoSize = true };
            cmbRol = new ComboBox { Location = new Point(150, 87), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRol.Items.Add("Seçiniz");
            cmbRol.Items.AddRange(new string[] { "Doktor", "Sekreter", "Admin" });
            cmbRol.SelectedIndex = 0;
            pnlForm.Controls.Add(lblRol);
            pnlForm.Controls.Add(cmbRol);

            Button btnGuncelle = new Button
            {
                Text = "🔑 Rolü Güncelle",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(150, 150),
                BackColor = KoyuMavi,
                ForeColor = SafBeyaz,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGuncelle.FlatAppearance.BorderSize = 0;
            btnGuncelle.Click += BtnGuncelle_Click;
            pnlForm.Controls.Add(btnGuncelle);
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTc.Text))
            {
                MessageBox.Show("Lütfen rolü güncellenecek kullanıcının TC Kimlik Numarasını giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlHelper db = new SqlHelper();

                // Kullanıcının sistemde olup olmadığını kontrol et
                var tcParam = new Dictionary<string, object> { { "@tc_no", txtTc.Text.Trim() } };
                object idObj = db.ExecuteScalar("sp_KullaniciIdGetirByTc", tcParam);
                
                if (idObj == null)
                {
                    MessageBox.Show("Girdiğiniz TC Kimlik Numarasına ait bir kullanıcı bulunamadı!", "Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                int rolId = 1;
                if (cmbRol.SelectedItem.ToString() == "Sekreter") rolId = 2;
                else if (cmbRol.SelectedItem.ToString() == "Admin") rolId = 3;

                var param = new Dictionary<string, object>
                {
                    { "@tc_no", txtTc.Text.Trim() },
                    { "@rol_ID", rolId }
                };

                db.ExecuteNonQuery("sp_KullaniciYetkiGuncelle", param);

                MessageBox.Show($"Kullanıcının rolü başarıyla '{cmbRol.SelectedItem.ToString()}' olarak güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtTc.Clear();
                cmbRol.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı rolü güncellenirken bir hata oluştu:\n\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

