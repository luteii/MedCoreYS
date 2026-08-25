using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class ProfilBilgilerEkrani : UserControl
    {
        // Renk Paleti
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color HastaMavi = ColorTranslator.FromHtml("#3B82F6");
        Color AcikMavi = ColorTranslator.FromHtml("#38BDF8");
        Color BasariYesil = ColorTranslator.FromHtml("#10B981");

        // UI Elemanları
        Label lblAdSoyad, lblRol, lblKanGrubu;
        Label lblTCKimlik, lblDogumTarihi, lblTelefon;
        Panel pnlAvatar;
        FlowLayoutPanel flpRandevular, flpTahliller;
        Label lblTarihSaat;

        public ProfilBilgilerEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
            this.Load += (s, e) => VerileriYukle();
        }

        private void EkraniKur()
        {
            // Ana Başlık
            Label lblTitle = new Label
            {
                Text = "Hasta Paneli / Özet",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            // =====================================
            // SOL SÜTUN - BİRLEŞTİRİLMİŞ PROFİL KARTI
            // Genişlik: 340, Yükseklik: 470
            // =====================================
            Panel pnlProfilKart = new Panel
            {
                Location = new Point(40, 85),
                Size = new Size(340, 480),
                BackColor = SafBeyaz
            };
            pnlProfilKart.Paint += (s, e) => InceCerceveCiz(pnlProfilKart, e.Graphics, 20);
            pnlProfilKart.Resize += (s, e) => OvalKirp(pnlProfilKart, 20);
            this.Controls.Add(pnlProfilKart);

            // Üst Kısım Arka Planı (Gradyan)
            Panel pnlProfilUst = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = Color.Transparent };
            pnlProfilUst.Paint += (s, e) =>
            {
                using (LinearGradientBrush br = new LinearGradientBrush(pnlProfilUst.ClientRectangle, ColorTranslator.FromHtml("#1E3A8A"), ColorTranslator.FromHtml("#3B82F6"), LinearGradientMode.Vertical))
                    e.Graphics.FillRectangle(br, pnlProfilUst.ClientRectangle);
            };
            pnlProfilKart.Controls.Add(pnlProfilUst);

            // Avatar (Ortalanmış: 340/2 - 50 = 120)
            pnlAvatar = new Panel { Location = new Point(120, 30), Size = new Size(100, 100), BackColor = Color.Transparent };
            pnlAvatar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush bg = new SolidBrush(SafBeyaz))
                    e.Graphics.FillEllipse(bg, 0, 0, 99, 99);
                using (Pen pen = new Pen(HastaMavi, 3))
                    e.Graphics.DrawEllipse(pen, 1, 1, 97, 97);

                string initials = GetInitials(Program.AktifKullaniciAdSoyad);
                TextRenderer.DrawText(e.Graphics, initials, new Font("Segoe UI", 26, FontStyle.Bold), new Rectangle(0, 0, 100, 100), HastaMavi, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            pnlProfilKart.Controls.Add(pnlAvatar);
            pnlAvatar.BringToFront();

            // İsim ve Rol
            lblAdSoyad = new Label { Text = "Yükleniyor...", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = TextDark, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 150), Size = new Size(340, 30) };
            pnlProfilKart.Controls.Add(lblAdSoyad);

            lblRol = new Label { Text = "Hasta", Font = new Font("Segoe UI", 10), ForeColor = TextMuted, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 185), Size = new Size(340, 20) };
            pnlProfilKart.Controls.Add(lblRol);

            // Ayraç Çizgisi
            Panel pnlAyrac = new Panel { Location = new Point(40, 220), Size = new Size(260, 1), BackColor = ColorTranslator.FromHtml("#E2E8F0") };
            pnlProfilKart.Controls.Add(pnlAyrac);

            // Kişisel Bilgiler Listesi
            lblTCKimlik = ProfilBilgiOlustur(pnlProfilKart, "🪪", "TC Kimlik No", 240);
            lblDogumTarihi = ProfilBilgiOlustur(pnlProfilKart, "🎂", "Doğum Tarihi", 300);
            lblTelefon = ProfilBilgiOlustur(pnlProfilKart, "📞", "Telefon Numarası", 360);
            lblKanGrubu = ProfilBilgiOlustur(pnlProfilKart, "🩸", "Kan Grubu", 420);
            lblKanGrubu.ForeColor = ColorTranslator.FromHtml("#EF4444");

            // Tarih Saat Kartı (Profilin Altında)
            Panel pnlTarihKart = new Panel
            {
                Location = new Point(40, 585),
                Size = new Size(340, 95),
                BackColor = SafBeyaz
            };
            pnlTarihKart.Paint += (s, e) => InceCerceveCiz(pnlTarihKart, e.Graphics, 15);
            pnlTarihKart.Resize += (s, e) => OvalKirp(pnlTarihKart, 15);
            this.Controls.Add(pnlTarihKart);

            Label lblTarihIkon = new Label { Text = "📅", Font = new Font("Segoe UI Emoji", 26), Location = new Point(20, 22), AutoSize = true };
            pnlTarihKart.Controls.Add(lblTarihIkon);

            lblTarihSaat = new Label { Text = DateTime.Now.ToString("dd MMMM yyyy\ndddd"), Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, Location = new Point(95, 26), AutoSize = true };
            pnlTarihKart.Controls.Add(lblTarihSaat);

            // =====================================
            // SAĞ SÜTUN - GENİŞLETİLMİŞ BİLDİRİM PANELLERİ
            // Genişlik: 530, Konum: 410
            // =====================================
            
            // Yaklaşan Randevular Kartı
            Panel pnlRandevular = new Panel
            {
                Location = new Point(410, 85),
                Size = new Size(530, 600),
                BackColor = SafBeyaz
            };
            pnlRandevular.Paint += (s, e) => InceCerceveCiz(pnlRandevular, e.Graphics, 15);
            pnlRandevular.Resize += (s, e) => OvalKirp(pnlRandevular, 15);
            this.Controls.Add(pnlRandevular);

            Label lblRandevuBaslik = new Label { Text = "Yaklaşan Randevular", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true };
            pnlRandevular.Controls.Add(lblRandevuBaslik);

            flpRandevular = new FlowLayoutPanel
            {
                Location = new Point(20, 60),
                Size = new Size(490, 520),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            pnlRandevular.Controls.Add(flpRandevular);

            // Son Tahliller Kartı
            Panel pnlTahliller = new Panel
            {
                Location = new Point(960, 85),
                Size = new Size(530, 600),
                BackColor = SafBeyaz
            };
            pnlTahliller.Paint += (s, e) => InceCerceveCiz(pnlTahliller, e.Graphics, 15);
            pnlTahliller.Resize += (s, e) => OvalKirp(pnlTahliller, 15);
            this.Controls.Add(pnlTahliller);

            Label lblTahlilBaslik = new Label { Text = "Son Tahlil İşlemleri", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true };
            pnlTahliller.Controls.Add(lblTahlilBaslik);

            flpTahliller = new FlowLayoutPanel
            {
                Location = new Point(20, 60),
                Size = new Size(490, 520),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            pnlTahliller.Controls.Add(flpTahliller);
        }

        private Label ProfilBilgiOlustur(Panel parent, string ikon, string etiket, int y)
        {
            Label lblIkon = new Label { Text = ikon, Font = new Font("Segoe UI Emoji", 14), ForeColor = TextMuted, Location = new Point(40, y + 5), AutoSize = true, BackColor = Color.Transparent };
            parent.Controls.Add(lblIkon);

            Label lblEtiket = new Label { Text = etiket, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(85, y), AutoSize = true, BackColor = Color.Transparent };
            parent.Controls.Add(lblEtiket);

            Label lblDeger = new Label { Text = "Yükleniyor...", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, Location = new Point(85, y + 20), AutoSize = true, BackColor = Color.Transparent };
            parent.Controls.Add(lblDeger);

            return lblDeger;
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object>
                {
                    { "@kullanici_id", Program.AktifKullaniciID }
                };

                // 1. Profil Bilgileri
                DataTable dtProfil = db.GetTable("sp_HastalariGetir", prm);
                if (dtProfil.Rows.Count > 0)
                {
                    DataRow row = dtProfil.Rows[0];
                    lblAdSoyad.Text = row["AdSoyad"]?.ToString() ?? "Bilinmiyor";
                    if (string.IsNullOrWhiteSpace(row["KanGrubu"]?.ToString()))
                    {
                        lblKanGrubu.Text = "Belirtilmemiş";
                        lblKanGrubu.ForeColor = ColorTranslator.FromHtml("#EF4444");
                    }
                    else
                    {
                        lblKanGrubu.Text = row["KanGrubu"].ToString();
                        lblKanGrubu.ForeColor = TextDark;
                    }
                    lblTCKimlik.Text = row["TCKimlik"]?.ToString() ?? "-";
                    lblDogumTarihi.Text = row["DogumTarihi"]?.ToString() ?? "-";
                    lblTelefon.Text = row["Telefon"]?.ToString() ?? "-";
                    pnlAvatar.Invalidate();
                }

                // 2. Randevular (Sadece 'Bekliyor' olanlar)
                DataTable dtRandevu = db.GetTable("sp_RandevulariGetir", prm);
                flpRandevular.Controls.Clear();
                int randevuSayisi = 0;
                foreach (DataRow row in dtRandevu.Rows)
                {
                    if (row["Durum"].ToString() == "Bekliyor")
                    {
                        DateTime randevuTarihi;
                        if (DateTime.TryParse(row["Tarih & Saat"].ToString(), out randevuTarihi))
                        {
                            if (randevuTarihi >= DateTime.Now.Date) // Gelecekteki veya bugünkü randevular
                            {
                                flpRandevular.Controls.Add(BildirimKartiOlustur("⏰", $"{row["Bölüm"]}", $"{randevuTarihi:dd.MM.yyyy HH:mm} - Dr. {row["Doktor Adı"]}", ColorTranslator.FromHtml("#F59E0B")));
                                randevuSayisi++;
                                if (randevuSayisi >= 4) break; // En fazla 4 tane göster (küçük ekrana sığsın)
                            }
                        }
                    }
                }
                if (randevuSayisi == 0)
                    flpRandevular.Controls.Add(new Label { Text = "Yaklaşan randevunuz bulunmamaktadır.", Font = new Font("Segoe UI", 10), ForeColor = TextMuted, AutoSize = true, Padding = new Padding(5, 10, 0, 0) });

                // 3. Tahliller
                DataTable dtTahlil = db.GetTable("sp_TahlilleriGetir", prm);
                flpTahliller.Controls.Clear();
                int tahlilSayisi = 0;
                foreach (DataRow row in dtTahlil.Rows)
                {
                    string durum = row["Durum"].ToString();
                    Color ikonRengi = durum == "Sonuçlandı" ? BasariYesil : ColorTranslator.FromHtml("#F59E0B");
                    string ikon = durum == "Sonuçlandı" ? "✅" : "⏳";
                    
                    flpTahliller.Controls.Add(BildirimKartiOlustur(ikon, $"{row["Tahlil Adı"]}", $"İstek: {row["İstek Tarihi"]} - {durum}", ikonRengi));
                    tahlilSayisi++;
                    if (tahlilSayisi >= 4) break;
                }
                if (tahlilSayisi == 0)
                    flpTahliller.Controls.Add(new Label { Text = "Son zamanlarda yapılan tahliliniz yok.", Font = new Font("Segoe UI", 10), ForeColor = TextMuted, AutoSize = true, Padding = new Padding(5, 10, 0, 0) });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard verileri yüklenemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Panel BildirimKartiOlustur(string ikon, string baslik, string detay, Color renk)
        {
            Panel kart = new Panel { Size = new Size(470, 70), Margin = new Padding(0, 0, 0, 10), BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            kart.Paint += (s, e) => InceCerceveCiz(kart, e.Graphics, 8);
            kart.Resize += (s, e) => OvalKirp(kart, 8);

            Label lblIkon = new Label { Text = ikon, Font = new Font("Segoe UI Emoji", 16), ForeColor = renk, Location = new Point(10, 15), AutoSize = true, BackColor = Color.Transparent };
            kart.Controls.Add(lblIkon);

            Label lblBas = new Label { Text = baslik, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextDark, Location = new Point(65, 10), AutoSize = false, Size = new Size(390, 24), AutoEllipsis = true, BackColor = Color.Transparent };
            kart.Controls.Add(lblBas);

            Label lblDetay = new Label { Text = detay, Font = new Font("Segoe UI", 9), ForeColor = TextMuted, Location = new Point(65, 36), AutoSize = false, Size = new Size(390, 24), AutoEllipsis = true, BackColor = Color.Transparent };
            kart.Controls.Add(lblDetay);

            return kart;
        }

        private string GetInitials(string adSoyad)
        {
            if (string.IsNullOrWhiteSpace(adSoyad)) return "?";
            string[] parts = adSoyad.Trim().Split(' ');
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[parts.Length - 1][0]}".ToUpper();
            return adSoyad.Substring(0, Math.Min(2, adSoyad.Length)).ToUpper();
        }

        private void OvalKirp(Panel pnl, int radius)
        {
            if (pnl.Width > 0 && pnl.Height > 0)
            {
                using (GraphicsPath path = TamKoseOval(pnl.Width, pnl.Height, radius))
                {
                    pnl.Region?.Dispose();
                    pnl.Region = new Region(path);
                }
                pnl.Invalidate();
            }
        }

        private void InceCerceveCiz(Panel pnl, Graphics g, int radius)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = TamKoseOval(pnl.Width - 1, pnl.Height - 1, radius))
            using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                g.DrawPath(pen, path);
        }

        private GraphicsPath TamKoseOval(int width, int height, int radius)
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
