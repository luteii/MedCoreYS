using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HastaneYonetim.Moduller_Doktor;
using HastaneYonetim.Moduller_Doktor.Recete;
using HastaneYonetim.Moduller_Doktor.Tahliller;

namespace HastaneYonetim
{
    public partial class DoktorDashboard : Form
    {
        // Modern Dashboard Renk Paleti (Doktor Paneli)
        Color IconBarStart = ColorTranslator.FromHtml("#0F172A");
        Color IconBarEnd = ColorTranslator.FromHtml("#1E3A8A");
        Color SubMenuStart = ColorTranslator.FromHtml("#1E293B");
        Color SubMenuEnd = ColorTranslator.FromHtml("#3B82F6");
        Color AnaZemin = ColorTranslator.FromHtml("#F0F4F8");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");

        // Bileşenler
        Panel pnlSidebar;
        Panel pnlTopBar;
        Panel pnlMain;
        Label lblSubMenuTitle;
        ToolTip ikonMesajlari;

        // Seçili Durum Takip Değişkenleri
        Button seciliAnaMenuButonu;
        Button seciliAltMenuButonu;
        Button btnVarsayilan; // İlk açılışta tıklanacak ana menü butonu

        public DoktorDashboard()
        {
            ikonMesajlari = new ToolTip();
            ikonMesajlari.AutoPopDelay = 5000;
            ikonMesajlari.InitialDelay = 200;
            ikonMesajlari.ReshowDelay = 100;

            ModernDoktorPaneliniKur();

            // ========================================================
            // İŞTE SİHİRLİ KISIM: FORM EKRANDA GÖRÜNDÜĞÜ ANDA TETİKLENİR
            // ========================================================
            this.Shown += (s, e) => {
                // 1. Önce "Genel Bakış" ana menü butonuna tıkla
                if (btnVarsayilan != null)
                {
                    btnVarsayilan.PerformClick();

                    // 2. Açılan alt menü butonlarını tara ve İLKİNE (Özet İstatistikler) tıkla
                    foreach (Control ctrl in pnlSidebar.Controls)
                    {
                        if (ctrl is Button btn && btn.Tag?.ToString() == "SubMenuBtn")
                        {
                            btn.PerformClick(); // Bu kod hem butonu mavi yapar hem sağ ekranı açar!
                            break;
                        }
                    }
                }
            };
        }

        private void ModernDoktorPaneliniKur()
        {
            this.Text = "MedCore YS - Doktor Çalışma Alanı";
            this.WindowState = FormWindowState.Maximized; // TAM EKRAN AÇILIR
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AnaZemin;
            this.MinimumSize = new Size(1024, 700);

            string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.ico");
            if (System.IO.File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }

            // Paneller
            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 350 };
            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };
            pnlMain = new Panel { Dock = DockStyle.Fill, BackColor = AnaZemin };

            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlTopBar);
            this.Controls.Add(pnlMain);
            this.Controls.SetChildIndex(pnlSidebar, 2);
            this.Controls.SetChildIndex(pnlTopBar, 1);
            this.Controls.SetChildIndex(pnlMain, 0);

            // Sidebar Arka Planı
            pnlSidebar.Resize += (s, e) => {
                if (pnlSidebar.BackgroundImage != null) pnlSidebar.BackgroundImage.Dispose();
                pnlSidebar.BackgroundImage = PnlSidebarArkaPlan(pnlSidebar.Width, pnlSidebar.Height);
            };
            pnlSidebar.BackgroundImage = PnlSidebarArkaPlan(pnlSidebar.Width, pnlSidebar.Height);

            // Doktor Avatarı
            Panel pnlAvatar = new Panel { Size = new Size(54, 54), Location = new Point(18, 12), BackColor = Color.Transparent, Cursor = Cursors.Hand };
            ikonMesajlari.SetToolTip(pnlAvatar, "Doktor Profili / Ayarlar");
            pnlAvatar.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush bg = new SolidBrush(ColorTranslator.FromHtml("#0284C7")))
                    e.Graphics.FillEllipse(bg, 0, 0, 53, 53);
                TextRenderer.DrawText(e.Graphics, "DR", new Font("Segoe UI", 16, FontStyle.Bold), new Rectangle(0, 0, 54, 54), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            pnlSidebar.Controls.Add(pnlAvatar);

            // MENÜ BUTONLARI
            btnVarsayilan = IkonButonuOlustur("⌂", "Genel Bakış", 110, (s, e) => AltMenuGuncelle("Genel Bakış", new string[] { "Özet İstatistikler|sp_DoktorIstatistik", "Günlük Takvim|Takvim" }));
            IkonButonuOlustur("✚", "Muayene İşlemleri", 180, (s, e) => AltMenuGuncelle("Muayene İşlemleri", new string[] { "Aktif Muayene|AktifMuayeneGoster", "Geçmiş Muayeneler|GecmisMuayeneGoster" }));
            IkonButonuOlustur("℞", "Reçete & İlaç", 250, (s, e) => AltMenuGuncelle("Reçete Yönetimi", new string[] { "Yeni Reçete Yaz|sp_ReceteEkle", "İlaç Veritabanı|sp_IlaclariGetir" }));
            IkonButonuOlustur("⚗", "Tahlil & Tetkik", 320, (s, e) => AltMenuGuncelle("Tahlil İşlemleri", new string[] { "Tahlil İste|sp_TahlilEkle", "Sonuç İncele|sp_TahlilSonuclari" }));
            IkonButonuOlustur("🛏", "Yatış İşlemleri", 390, (s, e) => AltMenuGuncelle("Yatış Yönetimi", new string[] { "Oda Durumları|sp_OdalarinDurumu", "Hasta Yatış Ver|sp_YatisVer" }));

            // Çıkış Butonu
            Button btnSolAltCikis = new Button { Text = "", Size = new Size(56, 56), Location = new Point(17, this.ClientSize.Height - 80), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btnSolAltCikis.FlatAppearance.BorderSize = 0;
            btnSolAltCikis.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnSolAltCikis.FlatAppearance.MouseDownBackColor = Color.Transparent;
            ikonMesajlari.SetToolTip(btnSolAltCikis, "Sistemden Çıkış Yap");

            btnSolAltCikis.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                if (btnSolAltCikis.ClientRectangle.Contains(btnSolAltCikis.PointToClient(Cursor.Position)))
                {
                    using (GraphicsPath path = TamKoseOval(btnSolAltCikis.Width, btnSolAltCikis.Height, 15))
                    using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                        e.Graphics.FillPath(hoverBrush, path);
                }
                TextRenderer.DrawText(e.Graphics, "⮡", new Font("Segoe UI Symbol", 20, FontStyle.Bold), btnSolAltCikis.ClientRectangle, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            btnSolAltCikis.MouseEnter += (s, e) => { btnSolAltCikis.Invalidate(); };
            btnSolAltCikis.MouseLeave += (s, e) => { btnSolAltCikis.Invalidate(); };
            btnSolAltCikis.Click += (s, e) => { Application.Restart(); };
            pnlSidebar.Controls.Add(btnSolAltCikis);

            // Sağ Taraf Başlığı
            lblSubMenuTitle = new Label { Text = "Genel Bakış", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, Location = new Point(105, 25), AutoSize = true };
            pnlSidebar.Controls.Add(lblSubMenuTitle);

            // Üst Çubuk 
            pnlTopBar.Paint += (s, e) => { e.Graphics.DrawLine(new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1), 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1); };

            // Giriş yapan doktorun adını Program.cs'den çekiyoruz
            string drIsim = string.IsNullOrEmpty(Program.AktifKullaniciAdSoyad) ? "Uzman Hekim" : Program.AktifKullaniciAdSoyad;
            Label lblKullaniciBilgi = new Label { Text = $"Dr. {drIsim} - Çalışma Paneli", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, 22) };
            pnlTopBar.Controls.Add(lblKullaniciBilgi);
        }

        private void AltMenuGuncelle(string baslik, string[] islemler)
        {
            lblSubMenuTitle.Text = baslik;
            seciliAltMenuButonu = null;

            for (int i = pnlSidebar.Controls.Count - 1; i >= 0; i--)
            {
                if (pnlSidebar.Controls[i] is Button && pnlSidebar.Controls[i].Tag?.ToString() == "SubMenuBtn")
                    pnlSidebar.Controls[i].Dispose();
            }

            int baslangicY = 85;
            foreach (string islem in islemler)
            {
                string islemAdi = islem.Split('|')[0];
                string dbProsedurAdi = islem.Split('|')[1];

                Button btnSub = new Button { Tag = "SubMenuBtn", Text = "", Location = new Point(100, baslangicY), Size = new Size(240, 45), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Cursor = Cursors.Hand };
                btnSub.FlatAppearance.BorderSize = 0;
                btnSub.FlatAppearance.MouseOverBackColor = Color.Transparent;
                btnSub.FlatAppearance.MouseDownBackColor = Color.Transparent;

                btnSub.Click += (s, e) => {
                    var onceki = seciliAltMenuButonu;
                    seciliAltMenuButonu = btnSub;
                    if (onceki != null) onceki.Invalidate();
                    btnSub.Invalidate();
                    ProsedurCalistir(dbProsedurAdi, islemAdi);
                };

                btnSub.Paint += (s, e) => {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    bool isHovered = btnSub.ClientRectangle.Contains(btnSub.PointToClient(Cursor.Position));
                    bool isSelected = (btnSub == seciliAltMenuButonu);

                    if (isHovered || isSelected)
                    {
                        using (GraphicsPath path = TamKoseOval(btnSub.Width, btnSub.Height, 12))
                        using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(isSelected ? 60 : 30, 255, 255, 255)))
                            e.Graphics.FillPath(hoverBrush, path);
                    }
                    TextRenderer.DrawText(e.Graphics, "  •   " + islemAdi, new Font("Segoe UI", 10F, FontStyle.Bold), btnSub.ClientRectangle, Color.White, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                };

                btnSub.MouseEnter += (s, e) => { btnSub.Invalidate(); };
                btnSub.MouseLeave += (s, e) => { btnSub.Invalidate(); };

                pnlSidebar.Controls.Add(btnSub);
                baslangicY += 50;
            }
        }

        // ==========================================
        // ROUTER: SAĞ TARAFTA İLGİLİ EKRANI AÇAR
        // =========================================
        private void ProsedurCalistir(string komutAdi, string islemAdi)
        {
            pnlMain.Controls.Clear(); // Önceki ekranı temizle

            // 1. Özet İstatistikler Seçildiyse
            if (komutAdi == "sp_DoktorIstatistik")
            {
                GenelBakisEkrani ozetEkran = new GenelBakisEkrani();
                ozetEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(ozetEkran);
            }
            // 2. GÜNLÜK TAKVİM SEÇİLDİYSE (Muhtemelen atlanan veya yanlış yere giren kısım buydu)
            else if (komutAdi == "Takvim")
            {
                TakvimEkrani takvimEkran = new TakvimEkrani();
                takvimEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(takvimEkran);
            }
            // 3. Aktif Muayene Seçildiyse
            else if (komutAdi == "AktifMuayeneGoster")
            {
                AktifMuayeneEkrani aktifEkran = new AktifMuayeneEkrani();
                aktifEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(aktifEkran);
            }
            // 4. Geçmiş Muayeneler Seçildiyse
            else if (komutAdi == "GecmisMuayeneGoster")
            {
                GecmisMuayeneEkrani gecmisEkran = new GecmisMuayeneEkrani();
                gecmisEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(gecmisEkran);
            }
            // Yeni Reçete Yaz Seçildiyse
            else if (komutAdi == "sp_ReceteEkle")
            {
                YeniReceteEkrani receteEkran = new YeniReceteEkrani();
                receteEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(receteEkran);
            }
            // İlaç Veritabanı Seçildiyse
            else if (komutAdi == "sp_IlaclariGetir")
            {
                IlacVeritabaniEkrani ilacEkran = new IlacVeritabaniEkrani();
                ilacEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(ilacEkran);
            }
            // Tahlil İste Seçildiyse
            else if (komutAdi == "TahlilIste" || islemAdi == "Tahlil İste")
            {
                TahlilIsteEkrani tahlilEkran = new TahlilIsteEkrani();
                tahlilEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(tahlilEkran);
            }
            // Sonuç İncele Seçildiyse
            else if (komutAdi == "SonucIncele" || islemAdi == "Sonuç İncele")
            {
                SonucInceleEkrani sonucEkran = new SonucInceleEkrani();
                sonucEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(sonucEkran);
            }
            // 9. Oda Durumları Seçildiyse
            else if (komutAdi == "OdaDurumlari" || islemAdi == "Oda Durumları")
            {
                OdaDurumlariEkrani odaEkran = new OdaDurumlariEkrani();
                odaEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(odaEkran);
            }
            // 10. Hasta Yatış Ver Seçildiyse
            else if (komutAdi == "HastaYatisVer" || islemAdi == "Hasta Yatış Ver")
            {
                HastaYatisEkrani yatisEkran = new HastaYatisEkrani();
                yatisEkran.Dock = DockStyle.Fill;
                pnlMain.Controls.Add(yatisEkran);
            }
            // Henüz Tasarlanmamış Modüller
            else
            {
                Label lblPlaceholder = new Label
                {
                    Text = $"Seçilen İşlem: {islemAdi}\n\nBu modül henüz tasarlanmadı.",
                    Font = new Font("Segoe UI", 14, FontStyle.Regular),
                    ForeColor = ColorTranslator.FromHtml("#64748B"),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pnlMain.Controls.Add(lblPlaceholder);
            }
        }

        // ==========================================
        // YARDIMCI ÇİZİM METOTLARI
        // ==========================================
        private Bitmap PnlSidebarArkaPlan(int w, int h)
        {
            if (w <= 0 || h <= 0) return null;
            Bitmap bmp = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(AnaZemin);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = SagKoseOval(w, h, 25))
                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, w, h + 1), SubMenuStart, SubMenuEnd, LinearGradientMode.Vertical))
                    g.FillPath(brush, path);

                g.SmoothingMode = SmoothingMode.None;
                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, 90, h + 1), IconBarStart, IconBarEnd, LinearGradientMode.Vertical))
                    g.FillRectangle(brush, new Rectangle(0, 0, 90, h));

                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
                    g.DrawLine(pen, 105, 69, w - 20, 69);
            }
            return bmp;
        }

        private GraphicsPath SagKoseOval(int width, int height, int radius)
        {
            radius = Math.Min(radius, Math.Min(width / 2, height / 2));
            if (radius <= 0) radius = 1;
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddLine(0, 0, width - radius, 0);
            path.AddArc(width - curveSize, 0, curveSize, curveSize, 270, 90);
            path.AddLine(width, radius, width, height - radius);
            path.AddArc(width - curveSize, height - curveSize, curveSize, curveSize, 0, 90);
            path.AddLine(width - radius, height, 0, height);
            path.CloseFigure();
            return path;
        }

        private GraphicsPath TamKoseOval(int width, int height, int radius)
        {
            radius = Math.Min(radius, Math.Min(width / 2, height / 2));
            if (radius <= 0) radius = 1;
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;
            path.StartFigure();
            path.AddArc(0, 0, curveSize, curveSize, 180, 90);
            path.AddArc(width - curveSize, 0, curveSize, curveSize, 270, 90);
            path.AddArc(width - curveSize, height - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(0, height - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Button IkonButonuOlustur(string ikonSembolu, string ipucuMesaji, int yPos, EventHandler onClick)
        {
            Button btn = new Button { Text = "", Size = new Size(56, 56), Location = new Point(17, yPos), FlatStyle = FlatStyle.Flat, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;

            btn.Click += (s, e) => {
                var onceki = seciliAnaMenuButonu;
                seciliAnaMenuButonu = btn;
                if (onceki != null) onceki.Invalidate();
                btn.Invalidate();
                onClick?.Invoke(s, e);
            };

            ikonMesajlari.SetToolTip(btn, ipucuMesaji);

            btn.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool isHovered = btn.ClientRectangle.Contains(btn.PointToClient(Cursor.Position));
                bool isSelected = (btn == seciliAnaMenuButonu);

                if (isHovered || isSelected)
                {
                    using (GraphicsPath path = TamKoseOval(btn.Width, btn.Height, 15))
                    using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(isSelected ? 60 : 30, 255, 255, 255)))
                        e.Graphics.FillPath(hoverBrush, path);

                    if (isSelected)
                    {
                        using (GraphicsPath indPath = TamKoseOval(4, 20, 2))
                        using (SolidBrush indBrush = new SolidBrush(Color.White))
                        {
                            e.Graphics.TranslateTransform(0, (btn.Height - 20) / 2);
                            e.Graphics.FillPath(indBrush, indPath);
                            e.Graphics.ResetTransform();
                        }
                    }
                }
                TextRenderer.DrawText(e.Graphics, ikonSembolu, new Font("Segoe UI Symbol", 20), btn.ClientRectangle, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            btn.MouseEnter += (s, e) => { btn.Invalidate(); };
            btn.MouseLeave += (s, e) => { btn.Invalidate(); };

            pnlSidebar.Controls.Add(btn);
            return btn;
        }
    }
}