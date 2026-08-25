using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim
{
    public partial class AdminDashboard : Form
    {
        Color IconBarStart = ColorTranslator.FromHtml("#0F172A");
        Color IconBarEnd = ColorTranslator.FromHtml("#1E3A8A");

        Color SubMenuStart = ColorTranslator.FromHtml("#1E293B");
        Color SubMenuEnd = ColorTranslator.FromHtml("#3B82F6");

        Color AnaZemin = ColorTranslator.FromHtml("#F8FAFC");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");

        Panel pnlSidebar;
        Panel pnlTopBar;
        Panel pnlMain;
        Label lblSubMenuTitle;
        Label lblContentTitle;
        ToolTip ikonMesajlari;

        Button seciliAnaMenuButonu;
        Button seciliAltMenuButonu;

        public AdminDashboard()
        {
            ikonMesajlari = new ToolTip();
            ikonMesajlari.AutoPopDelay = 5000;
            ikonMesajlari.InitialDelay = 200;
            ikonMesajlari.ReshowDelay = 100;

            ModernAdminPaneliniKur();
        }

        private void ModernAdminPaneliniKur()
        {
            this.Text = "MedCore YS - Yönetici (Admin) Paneli";
            this.Size = new Size(1280, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AnaZemin;
            this.MinimumSize = new Size(1024, 700);
            
            string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.ico");
            if (System.IO.File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }

            pnlSidebar = new Panel { Dock = DockStyle.Left, Width = 350 };
            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };
            pnlMain = new Panel { Dock = DockStyle.Fill, BackColor = AnaZemin };

            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlTopBar);
            this.Controls.Add(pnlMain);

            this.Controls.SetChildIndex(pnlSidebar, 2);
            this.Controls.SetChildIndex(pnlTopBar, 1);
            this.Controls.SetChildIndex(pnlMain, 0);

            pnlSidebar.Resize += (s, e) => {
                if (pnlSidebar.BackgroundImage != null) pnlSidebar.BackgroundImage.Dispose();
                pnlSidebar.BackgroundImage = PnlSidebarArkaPlan(pnlSidebar.Width, pnlSidebar.Height);
            };
            pnlSidebar.BackgroundImage = PnlSidebarArkaPlan(pnlSidebar.Width, pnlSidebar.Height);

            // --- PROFESYONEL AVATAR (ADMIN) ---
            Panel pnlAvatar = new Panel();
            pnlAvatar.Size = new Size(54, 54);
            pnlAvatar.Location = new Point(18, 12);
            pnlAvatar.BackColor = Color.Transparent;
            pnlAvatar.Cursor = Cursors.Hand;
            ikonMesajlari.SetToolTip(pnlAvatar, "Yönetici Profili / Sistem Ayarları");

            pnlAvatar.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush bg = new SolidBrush(ColorTranslator.FromHtml("#EF4444")))
                    e.Graphics.FillEllipse(bg, 0, 0, 53, 53);
                TextRenderer.DrawText(e.Graphics, "AD", new Font("Segoe UI", 16, FontStyle.Bold), new Rectangle(0, 0, 54, 54), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            pnlSidebar.Controls.Add(pnlAvatar);

            // --- ADMIN MENÜSÜ İKON BUTONLARI ---
            Button btnVarsayilan = IkonButonuOlustur("⌂", "Genel Bakış", 110, (s, e) => AltMenuGuncelle("Yönetim Paneli", new string[] { "Sistem İstatistikleri|sp_AdminIstatistik", "Log Kayıtları|sp_LoglariGetir" }));

            IkonButonuOlustur("👥", "Kullanıcı Yönetimi", 180, (s, e) => AltMenuGuncelle("Kullanıcı İşlemleri", new string[] { "Personel Listesi|sp_DoktorlariGetir", "Yeni Personel Ekle|sp_KullaniciEkle", "Kullanıcı Düzenle|sp_KullaniciGuncelle", "Kullanıcı Sil|sp_KullaniciSil" }));
            IkonButonuOlustur("🏥", "Bölüm Yönetimi", 250, (s, e) => AltMenuGuncelle("Bölüm Yönetimi", new string[] { "Bölümleri Listele|sp_BolumleriGetir", "Yeni Bölüm Ekle|sp_BolumEkle", "Bölüm Sil|sp_BolumSil" }));
            IkonButonuOlustur("⚙", "Sistem Ayarları", 320, (s, e) => AltMenuGuncelle("Sistem Ayarları", new string[] { "Yetkilendirme|sp_RolleriGetir", "Veritabanı Yedekle|DbBackup" }));

            seciliAnaMenuButonu = btnVarsayilan;
            btnVarsayilan.Invalidate();

            // ==========================================
            // SOL ALT KÖŞEYE SABİTLENMİŞ ÇIKIŞ BUTONU (ANCHOR EKLENDİ)
            // ==========================================
            Button btnSolAltCikis = new Button();
            btnSolAltCikis.Text = "";
            btnSolAltCikis.Size = new Size(56, 56);
            // Form büyüse de küçülse de sol alt köşeye yapışık kalması için Anchor tanımlandı
            btnSolAltCikis.Location = new Point(17, this.ClientSize.Height - 80);
            btnSolAltCikis.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSolAltCikis.FlatStyle = FlatStyle.Flat;
            btnSolAltCikis.FlatAppearance.BorderSize = 0;
            btnSolAltCikis.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnSolAltCikis.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnSolAltCikis.BackColor = Color.Transparent;
            btnSolAltCikis.Cursor = Cursors.Hand;
            ikonMesajlari.SetToolTip(btnSolAltCikis, "Sistemden Çıkış Yap");

            btnSolAltCikis.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool isHovered = btnSolAltCikis.ClientRectangle.Contains(btnSolAltCikis.PointToClient(Cursor.Position));

                if (isHovered)
                {
                    using (GraphicsPath path = TamKoseOval(btnSolAltCikis.Width, btnSolAltCikis.Height, 15))
                    using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                        e.Graphics.FillPath(hoverBrush, path);
                }

                TextRenderer.DrawText(e.Graphics, "⮡", new Font("Segoe UI Symbol", 20, FontStyle.Bold), btnSolAltCikis.ClientRectangle, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            btnSolAltCikis.MouseEnter += (s, e) => { btnSolAltCikis.Invalidate(); };
            btnSolAltCikis.MouseLeave += (s, e) => { btnSolAltCikis.Invalidate(); };

            btnSolAltCikis.Click += (s, e) => {
                // Sistemi tamamen temizler ve uygulamayı sıfırdan (Login formundan) tekrar başlatır
                Application.Restart();
            };
            pnlSidebar.Controls.Add(btnSolAltCikis);

            // --- SAĞ TARAF BAŞLIĞI ---
            lblSubMenuTitle = new Label();
            lblSubMenuTitle.Text = "Yönetim Paneli";
            lblSubMenuTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblSubMenuTitle.ForeColor = Color.White;
            lblSubMenuTitle.BackColor = Color.Transparent;
            lblSubMenuTitle.Location = new Point(105, 25);
            lblSubMenuTitle.AutoSize = true;
            pnlSidebar.Controls.Add(lblSubMenuTitle);

            // ==========================================
            // ÜST ÇUBUK (Top Bar)
            // ==========================================
            pnlTopBar.Paint += (s, e) => {
                e.Graphics.DrawLine(new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1), 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
            };

            Label lblKullaniciBilgi = new Label();
            lblKullaniciBilgi.Text = "Sistem Yöneticisi Paneli";
            lblKullaniciBilgi.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblKullaniciBilgi.ForeColor = TextDark;
            lblKullaniciBilgi.AutoSize = true;
            lblKullaniciBilgi.Location = new Point(30, 22);
            pnlTopBar.Controls.Add(lblKullaniciBilgi);

            // ==========================================
            // ANA ÇALIŞMA ALANI 
            // ==========================================
            lblContentTitle = new Label();
            lblContentTitle.Text = "İşlem yapmak için sol menüyü kullanabilirsiniz.";
            lblContentTitle.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            lblContentTitle.ForeColor = ColorTranslator.FromHtml("#64748B");
            lblContentTitle.Dock = DockStyle.Fill;
            lblContentTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlMain.Controls.Add(lblContentTitle);

            AltMenuGuncelle("Yönetim Paneli", new string[] {
                "Sistem İstatistikleri|sp_AdminIstatistik",
                "Log Kayıtları|sp_LoglariGetir"
            });

            this.Shown += (s, e) =>
            {
                // Form görünür olduğunda ilk menü öğesini otomatik seç (PerformClick sadece form görünürken çalışır)
                if (pnlSidebar.Controls.Count > 0)
                {
                    foreach (Control c in pnlSidebar.Controls)
                    {
                        if (c is Button btn && btn.Tag?.ToString() == "SubMenuBtn")
                        {
                            btn.PerformClick();
                            break;
                        }
                    }
                }
            };
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

                Button btnSub = new Button();
                btnSub.Tag = "SubMenuBtn";
                btnSub.Text = "";
                btnSub.Location = new Point(100, baslangicY);
                btnSub.Size = new Size(240, 45);
                btnSub.FlatStyle = FlatStyle.Flat;
                btnSub.FlatAppearance.BorderSize = 0;
                btnSub.FlatAppearance.MouseOverBackColor = Color.Transparent;
                btnSub.FlatAppearance.MouseDownBackColor = Color.Transparent;
                btnSub.BackColor = Color.Transparent;
                btnSub.Cursor = Cursors.Hand;

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

        private void ProsedurCalistir(string procAdi, string islemAdi)
        {
            pnlMain.Controls.Clear();
            switch (islemAdi)
            {
                case "Sistem İstatistikleri":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.SistemIstatistikleriEkrani());
                    break;
                case "Log Kayıtları":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.LogKayitlariEkrani());
                    break;
                case "Personel Listesi":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.PersonelListesiEkrani());
                    break;
                case "Yeni Personel Ekle":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.KullaniciEkleEkrani());
                    break;
                case "Kullanıcı Düzenle":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.KullaniciDuzenleEkrani());
                    break;
                case "Kullanıcı Sil":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.KullaniciSilEkrani());
                    break;
                case "Bölümleri Listele":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.BolumleriListeleEkrani());
                    break;
                case "Yeni Bölüm Ekle":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.BolumEkleEkrani());
                    break;
                case "Bölüm Sil":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.BolumSilEkrani());
                    break;
                case "Yetkilendirme":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.YetkilendirmeEkrani());
                    break;
                case "Veritabanı Yedekle":
                    pnlMain.Controls.Add(new HastaneYonetim.Moduller_Admin.VeritabaniYedekleEkrani());
                    break;
                default:
                    lblContentTitle.Text = $"Seçilen İşlem: {islemAdi}\n(Bu modül henüz yapım aşamasındadır.)";
                    lblContentTitle.ForeColor = ColorTranslator.FromHtml("#0284C7");
                    pnlMain.Controls.Add(lblContentTitle);
                    break;
            }
        }

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
                {
                    g.FillPath(brush, path);
                }

                g.SmoothingMode = SmoothingMode.None;
                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, 90, h + 1), IconBarStart, IconBarEnd, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, 90, h));
                }
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen pen = new Pen(Color.FromArgb(40, 255, 255, 255), 1))
                {
                    g.DrawLine(pen, 105, 69, w - 20, 69);
                }
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
            Button btn = new Button();
            btn.Text = "";
            btn.Size = new Size(56, 56);
            btn.Location = new Point(17, yPos);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btn.BackColor = Color.Transparent;
            btn.Cursor = Cursors.Hand;

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