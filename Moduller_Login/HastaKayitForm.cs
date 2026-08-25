using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim
{
    public partial class HastaKayitForm : Form
    {
        // Modern Renk Paleti (LoginForm ile uyumlu)
        Color MedikalGriMavi = ColorTranslator.FromHtml("#F0F4F8");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color AntrasitMetin = ColorTranslator.FromHtml("#1E293B");
        Color MedikalMavi = ColorTranslator.FromHtml("#0284C7");
        Color AcikMavi = ColorTranslator.FromHtml("#38BDF8");

        // Form Elemanları
        TextBox txtAdSoyad, txtTcNo, txtSifre, txtTelefon;
        DateTimePicker dtpDogumTarihi;
        ComboBox cmbCinsiyet;
        Button btnKayitOl, btnIptal;

        public HastaKayitForm()
        {
            KayitEkraniniKur();
        }

        private void KayitEkraniniKur()
        {
            this.Text = "MedCore YS - Yeni Hasta Kaydı";
            this.ClientSize = new Size(500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = MedikalGriMavi;

            // Ortadaki Beyaz Kart
            Panel pnlCard = new Panel();
            pnlCard.Size = new Size(420, 620);
            pnlCard.Location = new Point(40, 40);
            pnlCard.BackColor = SafBeyaz;
            pnlCard.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalKoseOlustur(new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1), 20))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1))
                {
                    pnlCard.Region = new Region(path);
                    e.Graphics.DrawPath(pen, path);
                }
            };
            this.Controls.Add(pnlCard);

            // Başlık
            Label lblTitle = new Label();
            lblTitle.Text = "Yeni Kayıt Oluştur";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = MedikalMavi;
            lblTitle.Location = new Point(0, 20);
            lblTitle.Size = new Size(420, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlCard.Controls.Add(lblTitle);

            // ==========================================
            // GİRDİ ALANLARI (OVAL PANELLİ TASARIM)
            // ==========================================
            int startY = 80;
            int gap = 65;

            txtAdSoyad = OvalTextBoxOlustur(pnlCard, "AD SOYAD", startY);

            txtTcNo = OvalTextBoxOlustur(pnlCard, "TC KİMLİK NO (11 Hane)", startY + gap);
            txtTcNo.MaxLength = 11;
            txtTcNo.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };

            txtSifre = OvalTextBoxOlustur(pnlCard, "ŞİFRE BELİRLEYİN", startY + (gap * 2));
            txtSifre.PasswordChar = '•'; // Şifre alanının karakterlerini gizleme özelliği eklendi

            txtTelefon = OvalTextBoxOlustur(pnlCard, "TELEFON (Örn: 05551112233)", startY + (gap * 3));

            // Doğum Tarihi Oval Wrapper
            Label lblDogum = new Label { Text = "DOĞUM TARİHİ", Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = AntrasitMetin, Location = new Point(50, startY + (gap * 4)), AutoSize = true };
            pnlCard.Controls.Add(lblDogum);
            Panel pnlDogum = new Panel { Location = new Point(50, startY + (gap * 4) + 20), Size = new Size(320, 40), BackColor = SafBeyaz };
            pnlDogum.Paint += OvalPanel_Paint;
            pnlCard.Controls.Add(pnlDogum);

            dtpDogumTarihi = new DateTimePicker { Location = new Point(10, 8), Size = new Size(300, 25), Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Short };
            pnlDogum.Controls.Add(dtpDogumTarihi);

            // Cinsiyet Oval Wrapper
            Label lblCinsiyet = new Label { Text = "CİNSİYET", Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = AntrasitMetin, Location = new Point(50, startY + (gap * 5)), AutoSize = true };
            pnlCard.Controls.Add(lblCinsiyet);
            Panel pnlCinsiyet = new Panel { Location = new Point(50, startY + (gap * 5) + 20), Size = new Size(320, 40), BackColor = SafBeyaz };
            pnlCinsiyet.Paint += OvalPanel_Paint;
            pnlCard.Controls.Add(pnlCinsiyet);

            cmbCinsiyet = new ComboBox { Location = new Point(10, 8), Size = new Size(300, 25), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            cmbCinsiyet.Items.Add("Seçiniz");
            cmbCinsiyet.Items.AddRange(new string[] { "Kadın", "Erkek" });
            cmbCinsiyet.SelectedIndex = 0;
            pnlCinsiyet.Controls.Add(cmbCinsiyet);

            // ==========================================
            // OVAL BUTONLAR
            // ==========================================

            // Kayıt Butonu (Giriş Butonu ile Aynı Gradyan Tasarım)
            btnKayitOl = new Button { Text = "Kaydı Tamamla", Location = new Point(50, 485), Size = new Size(320, 50), Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, ForeColor = SafBeyaz, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btnKayitOl.FlatAppearance.BorderSize = 0;
            btnKayitOl.Paint += BtnKayitOl_Oval_Paint;
            btnKayitOl.MouseEnter += (s, e) => { btnKayitOl.Invalidate(); };
            btnKayitOl.MouseLeave += (s, e) => { btnKayitOl.Invalidate(); };
            btnKayitOl.Click += BtnKayitOl_Click; // Tıklama olayı kusursuz şekilde bağlı
            pnlCard.Controls.Add(btnKayitOl);

            // İptal Butonu (Kırmızı Dış Hatlı Oval Tasarım)
            btnIptal = new Button { Text = "İptal Et ve Geri Dön", Location = new Point(50, 545), Size = new Size(320, 40), Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, ForeColor = ColorTranslator.FromHtml("#EF4444"), BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btnIptal.FlatAppearance.BorderSize = 0;
            btnIptal.Paint += BtnIptal_Oval_Paint;
            btnIptal.MouseEnter += (s, e) => { btnIptal.Invalidate(); };
            btnIptal.MouseLeave += (s, e) => { btnIptal.Invalidate(); };
            btnIptal.Click += (s, e) => { this.Close(); };
            pnlCard.Controls.Add(btnIptal);
        }

        // ==========================================
        // ÇİZİM VE YARDIMCI METOTLAR
        // ==========================================
        private TextBox OvalTextBoxOlustur(Panel parent, string etiket, int yPos)
        {
            Label lbl = new Label { Text = etiket, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = AntrasitMetin, Location = new Point(50, yPos), AutoSize = true };
            parent.Controls.Add(lbl);

            Panel pnlTxt = new Panel();
            pnlTxt.Location = new Point(50, yPos + 20);
            pnlTxt.Size = new Size(320, 40);
            pnlTxt.BackColor = SafBeyaz;
            pnlTxt.Paint += OvalPanel_Paint;
            parent.Controls.Add(pnlTxt);

            TextBox txt = new TextBox { Location = new Point(15, 10), Size = new Size(290, 25), Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.None, BackColor = SafBeyaz, ForeColor = AntrasitMetin };
            pnlTxt.Controls.Add(txt);

            return txt;
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

        private void BtnKayitOl_Oval_Paint(object sender, PaintEventArgs e)
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

        private void BtnIptal_Oval_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bool isHovered = btn.ClientRectangle.Contains(btn.PointToClient(Cursor.Position));
            Rectangle rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);

            using (GraphicsPath path = OvalKoseOlustur(rect, 15))
            {
                btn.Region = new Region(path);
                if (isHovered)
                {
                    using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#FEE2E2")))
                        e.Graphics.FillPath(brush, path);
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(SafBeyaz))
                        e.Graphics.FillPath(brush, path);
                }

                using (Pen pen = new Pen(ColorTranslator.FromHtml("#EF4444"), 2))
                    e.Graphics.DrawPath(pen, path);
            }
            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, rect, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // ==========================================
        // VERİTABANI BAĞLANTILI TIKLAMA OLAYI (CLICK)
        // ==========================================
        private void BtnKayitOl_Click(object sender, EventArgs e)
        {
            // 1. Boş Alan Kontrolleri
            if (string.IsNullOrWhiteSpace(txtAdSoyad.Text) ||
                string.IsNullOrWhiteSpace(txtTcNo.Text) ||
                string.IsNullOrWhiteSpace(txtSifre.Text) ||
                string.IsNullOrWhiteSpace(txtTelefon.Text))
            {
                MessageBox.Show("Lütfen tüm alanları eksiksiz doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. TC Kimlik Uzunluk Kontrolü
            if (txtTcNo.Text.Length != 11)
            {
                MessageBox.Show("TC Kimlik Numarası 11 haneli olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 3. Dictionary Veri Yapısı ile Parametreleri Tanımlama
                Dictionary<string, object> prm = new Dictionary<string, object>()
                {
                    { "@ad_soyad", txtAdSoyad.Text.Trim() },
                    { "@tc_no", txtTcNo.Text.Trim() },
                    { "@sifre", SecurityHelper.HashPassword(txtSifre.Text.Trim()) },
                    { "@telefon", txtTelefon.Text.Trim() },
                    { "@dogum_tarihi", dtpDogumTarihi.Value.Date },
                    { "@cinsiyet", cmbCinsiyet.SelectedItem?.ToString() ?? "Kadın" }
                };

                // 4. SqlHelper Sınıfı ile Stored Procedure Çalıştırma
                SqlHelper db = new SqlHelper();
                db.ExecuteNonQuery("sp_HastaKaydol", prm);

                MessageBox.Show("Kayıt işleminiz başarıyla gerçekleştirildi. Giriş yapabilirsiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Kayıt penceresini kapat
                
                    // Sistemi tamamen temizler ve uygulamayı sıfırdan (Login formundan) tekrar başlatır
            Application.Restart();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayıt sırasında bir hata oluştu: " + ex.Message, "Kayıt Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}