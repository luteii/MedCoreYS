using HastaneYonetim;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using HastaneYonetim.Moduller_Doktor;

namespace HastaneYonetim
{
    public partial class LoginForm : Form
    {
        TextBox txtKullaniciAdi;
        TextBox txtSifre;
        Button btnGiris;
        Panel pnlCard;

        // Modern, Basic ve Medikal Renk Paleti
        Color MedikalGriMavi = ColorTranslator.FromHtml("#F0F4F8");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color AntrasitMetin = ColorTranslator.FromHtml("#1E293B");
        Color MedikalMavi = ColorTranslator.FromHtml("#0284C7");
        Color AcikMavi = ColorTranslator.FromHtml("#38BDF8");

        public LoginForm()
        {
            MedikalGirisEkraniniKur();
        }

        private void MedikalGirisEkraniniKur()
        {
            // 1. Ana Form
            this.Text = "MedCore YS - Kullanıcı Girişi";
            this.ClientSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.ico");
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }
            this.MaximizeBox = false;

            // 2. Sol Taraf (Görsel Paneli) - DARALTILDI (400px)
            Panel pnlLeft = new Panel();
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Size = new Size(400, 650);
            pnlLeft.BackColor = SafBeyaz;
            this.Controls.Add(pnlLeft);

            PictureBox pbLogo = new PictureBox();
            pbLogo.Dock = DockStyle.Fill;
            pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pbLogo.BackColor = SafBeyaz;

            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Moduller_Login", "foto1.png");
            if (File.Exists(imagePath))
            {
                pbLogo.Image = Image.FromFile(imagePath);
            }
            else
            {
                Label lblHata = new Label();
                lblHata.Text = "Görsel Yüklenemedi.";
                lblHata.Dock = DockStyle.Fill;
                lblHata.TextAlign = ContentAlignment.MiddleCenter;
                pbLogo.Controls.Add(lblHata);
            }
            pnlLeft.Controls.Add(pbLogo);

            // 3. Sağ Taraf (Giriş Paneli) - GENİŞLETİLDİ (600px)
            Panel pnlRight = new Panel();
            pnlRight.Location = new Point(400, 0);
            pnlRight.Size = new Size(600, 650);
            pnlRight.BackColor = MedikalGriMavi;
            this.Controls.Add(pnlRight);

            // 4. Ortadaki Kart - GENİŞLETİLDİ VE ORTALANDI
            pnlCard = new Panel();
            pnlCard.Size = new Size(460, 520);
            pnlCard.Location = new Point(70, 80);
            pnlCard.BackColor = SafBeyaz;

            pnlCard.Region = new Region(OvalKoseOlustur(pnlCard.ClientRectangle, 20));
            pnlRight.Controls.Add(pnlCard);

            // 5. Marka/Logo Başlığı
            Label lblTitle = new Label();
            lblTitle.Text = "MedCore YS";
            lblTitle.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblTitle.ForeColor = MedikalMavi;
            lblTitle.AutoSize = false;
            lblTitle.Size = new Size(460, 50);
            lblTitle.Location = new Point(0, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlCard.Controls.Add(lblTitle);

            // 6. Alt Başlık 
            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Sisteme giriş yapmak için bilgilerinizi girin";
            lblSubtitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblSubtitle.ForeColor = AntrasitMetin;
            lblSubtitle.AutoSize = false;
            lblSubtitle.Size = new Size(460, 25);
            lblSubtitle.Location = new Point(0, 115);
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlCard.Controls.Add(lblSubtitle);

            // 7. Kullanıcı Adı Etiketi (TC KİMLİK NO)
            Label lblKullanici = new Label();
            lblKullanici.Text = "TC KİMLİK NO";
            lblKullanici.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblKullanici.ForeColor = AntrasitMetin;
            lblKullanici.Location = new Point(70, 175);
            lblKullanici.AutoSize = true;
            pnlCard.Controls.Add(lblKullanici);

            // 8. Kullanıcı Adı Kutusu 
            Panel pnlTxt1 = new Panel();
            pnlTxt1.Location = new Point(70, 200);
            pnlTxt1.Size = new Size(320, 45);
            pnlTxt1.BackColor = SafBeyaz;
            pnlTxt1.Paint += OvalPanel_Paint;
            pnlCard.Controls.Add(pnlTxt1);

            txtKullaniciAdi = new TextBox();
            txtKullaniciAdi.Location = new Point(15, 12);
            txtKullaniciAdi.Size = new Size(290, 25);
            txtKullaniciAdi.Font = new Font("Segoe UI", 12);
            txtKullaniciAdi.BorderStyle = BorderStyle.None;
            txtKullaniciAdi.BackColor = SafBeyaz;
            txtKullaniciAdi.ForeColor = AntrasitMetin;
            txtKullaniciAdi.MaxLength = 11;
            txtKullaniciAdi.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            txtKullaniciAdi.KeyDown += TxtKullaniciAdi_KeyDown;
            pnlTxt1.Controls.Add(txtKullaniciAdi);

            // 9. Şifre Etiketi
            Label lblSifre = new Label();
            lblSifre.Text = "ŞİFRE";
            lblSifre.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblSifre.ForeColor = AntrasitMetin;
            lblSifre.Location = new Point(70, 265);
            lblSifre.AutoSize = true;
            pnlCard.Controls.Add(lblSifre);

            // 10. Şifre Kutusu 
            Panel pnlTxt2 = new Panel();
            pnlTxt2.Location = new Point(70, 290);
            pnlTxt2.Size = new Size(320, 45);
            pnlTxt2.BackColor = SafBeyaz;
            pnlTxt2.Paint += OvalPanel_Paint;
            pnlCard.Controls.Add(pnlTxt2);

            txtSifre = new TextBox();
            txtSifre.Location = new Point(15, 12);
            txtSifre.Size = new Size(290, 25);
            txtSifre.Font = new Font("Segoe UI", 12);
            txtSifre.PasswordChar = '•';
            txtSifre.BorderStyle = BorderStyle.None;
            txtSifre.BackColor = SafBeyaz;
            txtSifre.ForeColor = AntrasitMetin;
            txtSifre.KeyDown += TxtSifre_KeyDown;
            pnlTxt2.Controls.Add(txtSifre);

            // 11. Modern Gradyanlı Giriş Butonu
            btnGiris = new Button();
            btnGiris.Text = "Giriş Yap";
            btnGiris.Location = new Point(70, 375);
            btnGiris.Size = new Size(320, 55);
            btnGiris.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnGiris.FlatStyle = FlatStyle.Flat;
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Cursor = Cursors.Hand;
            btnGiris.ForeColor = SafBeyaz;
            btnGiris.BackColor = Color.Transparent;

            btnGiris.Paint += BtnGiris_Oval_Paint;
            btnGiris.MouseEnter += (s, e) => { btnGiris.Invalidate(); };
            btnGiris.MouseLeave += (s, e) => { btnGiris.Invalidate(); };
            btnGiris.Click += BtnGiris_Click;
            pnlCard.Controls.Add(btnGiris);

            // ==========================================
            // 12. YENİ KAYIT OLUŞTUR (LinkLabel)
            // ==========================================
            LinkLabel lblKayitOl = new LinkLabel();
            lblKayitOl.Text = "Sisteme kayıtlı değil misiniz? Yeni Kayıt Oluşturun";
            lblKayitOl.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            lblKayitOl.ForeColor = ColorTranslator.FromHtml("#64748B");
            lblKayitOl.LinkColor = MedikalMavi;
            lblKayitOl.ActiveLinkColor = ColorTranslator.FromHtml("#1E3A8A");
            lblKayitOl.LinkBehavior = LinkBehavior.AlwaysUnderline;

            lblKayitOl.AutoSize = false;
            lblKayitOl.Size = new Size(460, 25);
            lblKayitOl.Location = new Point(0, 445);
            lblKayitOl.TextAlign = ContentAlignment.MiddleCenter;
            lblKayitOl.LinkArea = new LinkArea(31, 20);

            lblKayitOl.LinkClicked += (s, e) => {
                HastaKayitForm kayitFormu = new HastaKayitForm();
                kayitFormu.ShowDialog();
            };

            pnlCard.Controls.Add(lblKayitOl);
        }

        private GraphicsPath OvalKoseOlustur(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void OvalPanel_Paint(object sender, PaintEventArgs e)
        {
            Control pnl = (Control)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = OvalKoseOlustur(new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1), 10))
            using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1))
            {
                pnl.Region = new Region(path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void BtnGiris_Oval_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            bool isHovered = btn.ClientRectangle.Contains(btn.PointToClient(Cursor.Position));

            Color startColor = isHovered ? AcikMavi : MedikalMavi;
            Color endColor = isHovered ? MedikalMavi : AcikMavi;

            Rectangle rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
            using (GraphicsPath path = OvalKoseOlustur(rect, 15))
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Horizontal))
            {
                btn.Region = new Region(path);
                e.Graphics.FillPath(brush, path);
            }

            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, rect, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // ==========================================
        // YENİ VERİTABANI BAĞLANTILI GİRİŞ SİSTEMİ
        // ==========================================

        private void TxtKullaniciAdi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Bip sesini engelle
                txtSifre.Focus();
            }
        }

        private void TxtSifre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnGiris.PerformClick();
            }
        }

        private void BtnGiris_Click(object sender, EventArgs e)
        {
            string tc = txtKullaniciAdi.Text.Trim();
            string sifre = txtSifre.Text.Trim();
            string hashedSifre = SecurityHelper.HashPassword(sifre);

            if (string.IsNullOrWhiteSpace(tc) || string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Lütfen TC Kimlik numaranızı ve şifrenizi boş bırakmayınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Dictionary<string, object> prm = new Dictionary<string, object>()
                {
                    { "@tc_no", tc },
                    { "@sifre", hashedSifre }
                };

                SqlHelper db = new SqlHelper();
                DataTable dt = db.GetTable("sp_KullaniciGiris", prm);

                if (dt.Rows.Count > 0)
                {
                    bool hesapAktif = Convert.ToBoolean(dt.Rows[0]["hesap_aktif_mi"]);
                    
                    if (!hesapAktif)
                    {
                        MessageBox.Show("Hesabınız pasife alınmıştır. Lütfen Admin ile iletişime geçin.", "Hesap Kilitli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int rolId = Convert.ToInt32(dt.Rows[0]["rol_ID"]);
                    
                    if (rolId == 10 && dt.Rows[0]["son_giris_tarihi"] != DBNull.Value)
                    {
                        DateTime sonGiris = Convert.ToDateTime(dt.Rows[0]["son_giris_tarihi"]);
                        if ((DateTime.Now - sonGiris).TotalDays >= 5)
                        {
                            db.ExecuteNonQuery("sp_HesapPasifeAl", new Dictionary<string, object> { { "@kullanici_id", Convert.ToInt32(dt.Rows[0]["kullanici_id"]) } });
                            MessageBox.Show("Hesabınız 5 günden uzun süre işlem yapılmadığı için güvenlik amacıyla pasife alınmıştır. Lütfen Admin ile iletişime geçin.", "Hesap Kilitli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Son giriş tarihini güncelle
                    db.ExecuteNonQuery("sp_KullaniciSonGirisGuncelle", new Dictionary<string, object> { { "@kullanici_id", Convert.ToInt32(dt.Rows[0]["kullanici_id"]) } });

                    // --- SİSTEM HAFIZASINA KULLANICI BİLGİLERİNİ YAZMA KISMI ---
                    Program.AktifKullaniciID = Convert.ToInt32(dt.Rows[0]["kullanici_id"]);
                    Program.AktifKullaniciAdSoyad = dt.Rows[0]["ad_soyad"].ToString();
                    // ------------------------------------------------------------

                    Logger.Log("Giriş İşlemi", $"{Program.AktifKullaniciAdSoyad} sisteme başarıyla giriş yaptı.", Program.AktifKullaniciID);

                    this.Hide();
                    Form hedefForm = null;

                    if (rolId == 1) // ADMİN
                    {
                        hedefForm = new AdminDashboard();
                    }
                    else if (rolId == 2) // DOKTOR
                    {
                        hedefForm = new DoktorDashboard();
                    }
                    else if (rolId == 10) // HASTA
                    {
                        hedefForm = new HastaDashboard();
                    }
                    else if (rolId == 5) // LABORANT
                    {
                        hedefForm = new HastaneYonetim.Moduller_Laborant.LaborantDashboard();
                    }
                    else
                    {
                        MessageBox.Show("Bu rol için panel henüz aktif değil.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Show();
                        return;
                    }

                    hedefForm.FormClosed += (s, args) => this.Close();

                    hedefForm.Show();
                }
                else
                {
                    Logger.Log("Hatalı Giriş", $"'{tc}' TC kimlik no ile hatalı giriş denemesi yapıldı.", null);
                    MessageBox.Show("TC Kimlik No veya Şifre hatalı! Lütfen tekrar deneyiniz.", "Giriş Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giriş işlemi sırasında sistemde bir hata oluştu: " + ex.Message, "Sistem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

