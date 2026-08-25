using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class KullaniciDuzenleEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color KoyuMavi = ColorTranslator.FromHtml("#0284C7");
        Color Turuncu = ColorTranslator.FromHtml("#F59E0B");

        ComboBox cmbKullanici;
        TextBox txtTcNo, txtAdSoyad, txtSifre;
        ComboBox cmbRol;
        CheckBox chkHesapAktif;
        DataTable dtKullanicilar;
        string mevcutSifreHash = "";

        public KullaniciDuzenleEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
            this.Load += (s, e) => KullanicilariYukle();
        }

        private void EkraniKur()
        {
            Label lblTitle = new Label
            {
                Text = "Kullanıcı Bilgilerini Düzenle",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Mevcut kullanıcıların (Doktor, Sekreter, Admin) kişisel bilgilerini ve şifrelerini güncelleyebilirsiniz.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(40, 80)
            };
            this.Controls.Add(lblSubtitle);

            // Arama/Seçim Paneli
            Panel pnlArama = new Panel
            {
                Location = new Point(40, 130),
                Size = new Size(840, 80),
                BackColor = SafBeyaz
            };
            pnlArama.Paint += (s, e) => 
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalKose(pnlArama.Width - 1, pnlArama.Height - 1, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    e.Graphics.DrawPath(pen, path);
            };
            this.Controls.Add(pnlArama);

            Label lblKullaniciSec = new Label { Text = "Düzenlenecek Kullanıcı:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(100, 25), AutoSize = true };
            pnlArama.Controls.Add(lblKullaniciSec);

            cmbKullanici = new ComboBox { Location = new Point(300, 23), Size = new Size(400, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbKullanici.SelectedIndexChanged += CmbKullanici_SelectedIndexChanged;
            pnlArama.Controls.Add(cmbKullanici);

            // Düzenleme Paneli
            Panel pnlForm = new Panel
            {
                Location = new Point(40, 240),
                Size = new Size(840, 420),
                BackColor = SafBeyaz
            };
            pnlForm.Paint += (s, e) => 
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalKose(pnlForm.Width - 1, pnlForm.Height - 1, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    e.Graphics.DrawPath(pen, path);
            };
            this.Controls.Add(pnlForm);

            int yPos = 30;
            txtTcNo = FormAlaniOlustur(pnlForm, "TC Kimlik No:", ref yPos);
            txtTcNo.MaxLength = 11;
            txtTcNo.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            txtAdSoyad = FormAlaniOlustur(pnlForm, "Ad Soyad:", ref yPos);
            txtSifre = FormAlaniOlustur(pnlForm, "Şifre:", ref yPos);
            
            Label lblRol = new Label { Text = "Sistem Rolü:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(100, yPos), AutoSize = true };
            pnlForm.Controls.Add(lblRol);
            cmbRol = new ComboBox { Location = new Point(300, yPos), Size = new Size(400, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRol.Items.Add("Seçiniz");
            cmbRol.Items.AddRange(new string[] { "Doktor", "Sekreter", "Admin", "Hasta" });
            cmbRol.SelectedIndex = 0;
            pnlForm.Controls.Add(cmbRol);
            yPos += 50;
            
            chkHesapAktif = new CheckBox { Text = "Hesap Durumu: Aktif (Giriş Yapabilir)", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(300, yPos), AutoSize = true, Cursor = Cursors.Hand };
            pnlForm.Controls.Add(chkHesapAktif);
            yPos += 50;

            Button btnKaydet = new Button
            {
                Text = "💾 Değişiklikleri Kaydet",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(400, 45),
                Location = new Point(300, yPos),
                BackColor = Turuncu,
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
            Label lbl = new Label { Text = etiket, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextDark, Location = new Point(100, yPos), AutoSize = true };
            TextBox txt = new TextBox { Location = new Point(300, yPos - 3), Size = new Size(400, 30), Font = new Font("Segoe UI", 11) };
            ebeveyn.Controls.Add(lbl);
            ebeveyn.Controls.Add(txt);
            yPos += 50;
            return txt;
        }

        private void KullanicilariYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                dtKullanicilar = db.GetTable("sp_KullanicilariGetir");
                
                cmbKullanici.SetDataSourceWithChooseOption(dtKullanicilar, "ad_soyad", "kullanici_id");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcılar yüklenemedi: " + ex.Message);
            }
        }

        private void CmbKullanici_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbKullanici.SelectedIndex >= 0 && dtKullanicilar != null && cmbKullanici.SelectedValue is int)
            {
                DataRow[] rows = dtKullanicilar.Select($"kullanici_id = {cmbKullanici.SelectedValue}");
                if (rows.Length > 0)
                {
                    txtTcNo.Text = rows[0]["tc_no"].ToString();
                    txtAdSoyad.Text = rows[0]["ad_soyad"].ToString();
                    mevcutSifreHash = rows[0]["sifre"].ToString();
                    txtSifre.Text = ""; // Hash'i ekranda göstermiyoruz
                    
                    int rolId = Convert.ToInt32(rows[0]["rol_ID"]);
                    if (rolId == 1) cmbRol.SelectedIndex = 0; // Doktor
                    else if (rolId == 2) cmbRol.SelectedIndex = 1; // Sekreter
                    else if (rolId == 3) cmbRol.SelectedIndex = 2; // Admin
                    else if (rolId == 10) cmbRol.SelectedIndex = 3; // Hasta
                    else cmbRol.SelectedIndex = -1;
                    
                    if (rows[0]["hesap_aktif_mi"] != DBNull.Value)
                        chkHesapAktif.Checked = Convert.ToBoolean(rows[0]["hesap_aktif_mi"]);
                    else
                        chkHesapAktif.Checked = true;
                }
            }
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (cmbKullanici.SelectedValue == null)
            {
                MessageBox.Show("Lütfen düzenlemek için bir kullanıcı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTcNo.Text) || string.IsNullOrWhiteSpace(txtAdSoyad.Text))
            {
                MessageBox.Show("Lütfen Ad Soyad ve TC Kimlik alanlarını eksiksiz doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlHelper db = new SqlHelper();
                
                int rolId = 1;
                if (cmbRol.SelectedIndex == 1) rolId = 2;
                else if (cmbRol.SelectedIndex == 2) rolId = 3;
                else if (cmbRol.SelectedIndex == 3) rolId = 10;

                string gonderilecekSifre = string.IsNullOrWhiteSpace(txtSifre.Text) ? mevcutSifreHash : SecurityHelper.HashPassword(txtSifre.Text);

                var param = new Dictionary<string, object>
                {
                    { "@kullanici_id", cmbKullanici.SelectedValue },
                    { "@ad_soyad", txtAdSoyad.Text },
                    { "@tc_no", txtTcNo.Text },
                    { "@sifre", gonderilecekSifre },
                    { "@rol_ID", rolId },
                    { "@hesap_aktif_mi", chkHesapAktif.Checked }
                };

                db.ExecuteNonQuery("sp_KullaniciGuncelle", param);

                MessageBox.Show("Kullanıcı bilgileri başarıyla güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Seçili durumu kaybetmeden verileri tekrar yükle
                int seciliId = Convert.ToInt32(cmbKullanici.SelectedValue);
                KullanicilariYukle();
                cmbKullanici.SelectedValue = seciliId;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kullanıcı güncellenirken bir hata oluştu:\n\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private GraphicsPath OvalKose(int w, int h, int r)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, r, r, 180, 90); path.AddArc(w - r, 0, r, r, 270, 90);
            path.AddArc(w - r, h - r, r, r, 0, 90); path.AddArc(0, h - r, r, r, 90, 90);
            path.CloseFigure(); return path;
        }
    }
}

