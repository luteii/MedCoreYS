using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class KullaniciEkleEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color KoyuMavi = ColorTranslator.FromHtml("#0284C7");

        TextBox txtTcNo, txtAdSoyad, txtSifre;
        ComboBox cmbRol, cmbBolum;
        Label lblBolum;
        System.Data.DataTable dtBolumler;

        public KullaniciEkleEkrani()
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
                Text = "Yeni Kullanıcı / Personel Ekle",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Sisteme erişimi olacak yeni bir doktor, sekreter veya admin eklemek için aşağıdaki formu doldurun.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(40, 80)
            };
            this.Controls.Add(lblSubtitle);

            Panel pnlForm = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(500, 400),
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

            int yPos = 30;
            txtTcNo = FormAlaniOlustur(pnlForm, "TC Kimlik No:", ref yPos);
            txtTcNo.MaxLength = 11;
            txtTcNo.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            txtAdSoyad = FormAlaniOlustur(pnlForm, "Ad Soyad:", ref yPos);
            txtSifre = FormAlaniOlustur(pnlForm, "Şifre:", ref yPos);
            
            Label lblRol = new Label { Text = "Sistem Rolü:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(30, yPos), AutoSize = true };
            pnlForm.Controls.Add(lblRol);
            cmbRol = new ComboBox { Location = new Point(150, yPos), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            
            // Veritabanındaki rol ID'leriyle eşleştirebilmek için.
            // Doktor=1, Sekreter=2, Admin=3 (Veya DB durumunuza göre id'ler belirlenir, SP içine de verilebilir). 
            // Şimdilik stringleri tutup SP'ye int yollayacağız.
            cmbRol.Items.Add("Seçiniz");
            cmbRol.Items.AddRange(new string[] { "Doktor", "Sekreter", "Admin" });
            cmbRol.SelectedIndex = 0;
            cmbRol.SelectedIndexChanged += CmbRol_SelectedIndexChanged;
            pnlForm.Controls.Add(cmbRol);
            yPos += 50;
            
            lblBolum = new Label { Text = "Bölüm:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(30, yPos), AutoSize = true, Visible = true };
            pnlForm.Controls.Add(lblBolum);
            cmbBolum = new ComboBox { Location = new Point(150, yPos), Size = new Size(300, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, Visible = true };
            pnlForm.Controls.Add(cmbBolum);
            yPos += 50;
            
            BolumleriYukle();

            Button btnKaydet = new Button
            {
                Text = "💾 Personeli Kaydet",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(150, yPos),
                BackColor = KoyuMavi,
                ForeColor = SafBeyaz,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnKaydet.FlatAppearance.BorderSize = 0;
            btnKaydet.Click += BtnKaydet_Click;
            pnlForm.Controls.Add(btnKaydet);
        }

        private TextBox FormAlaniOlustur(Panel ebeveyn, string etiket, ref int yPos)
        {
            Label lbl = new Label { Text = etiket, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(30, yPos), AutoSize = true };
            TextBox txt = new TextBox { Location = new Point(150, yPos - 3), Size = new Size(300, 30), Font = new Font("Segoe UI", 11) };
            ebeveyn.Controls.Add(lbl);
            ebeveyn.Controls.Add(txt);
            yPos += 50;
            return txt;
        }

        private void BolumleriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                dtBolumler = db.GetTable("sp_BolumleriGetir");
                cmbBolum.SetDataSourceWithChooseOption(dtBolumler, "BolumAdi", "bolum_id");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bölümler yüklenemedi: " + ex.Message);
            }
        }

        private void CmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isDoktor = cmbRol.SelectedItem?.ToString() == "Doktor";
            lblBolum.Visible = isDoktor;
            cmbBolum.Visible = isDoktor;
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTcNo.Text) || string.IsNullOrWhiteSpace(txtAdSoyad.Text) || string.IsNullOrWhiteSpace(txtSifre.Text))
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlHelper db = new SqlHelper();
                
                int rolId = 2; // Doktor (Varsayılan)
                if (cmbRol.SelectedItem.ToString() == "Sekreter") rolId = 3;
                else if (cmbRol.SelectedItem.ToString() == "Admin") rolId = 1;

                var param = new Dictionary<string, object>
                {
                    { "@tc_no", txtTcNo.Text },
                    { "@ad_soyad", txtAdSoyad.Text },
                    { "@sifre", SecurityHelper.HashPassword(txtSifre.Text) },
                    { "@rol_ID", rolId }
                };
                
                if (rolId == 2 && cmbBolum.SelectedValue != null)
                {
                    param.Add("@bolum_ID", cmbBolum.SelectedValue);
                }

                db.ExecuteNonQuery("sp_KullaniciEkle", param);

                MessageBox.Show("Yeni personel başarıyla sisteme kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Formu Temizle
                txtTcNo.Clear();
                txtAdSoyad.Clear();
                txtSifre.Clear();
                cmbRol.SelectedIndex = 0;
                if(cmbBolum.Items.Count > 0) cmbBolum.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Personel kaydedilirken bir hata oluştu:\n\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

