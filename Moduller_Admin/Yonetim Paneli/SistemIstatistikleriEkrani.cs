using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class SistemIstatistikleriEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color VurguRengi = ColorTranslator.FromHtml("#3B82F6");

        Label lblKullaniciSayisi, lblBolumSayisi, lblRandevuSayisi, lblHataSayisi;
        Panel pnlGrafikContainer, pnlSonHareketlerContainer;

        public SistemIstatistikleriEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
            
            if (!this.DesignMode)
            {
                this.Load += (s, e) => LoadStatistics();
            }
        }

        private void EkraniKur()
        {
            // Ana Taşıyıcı
            TableLayoutPanel tlpAna = new TableLayoutPanel();
            tlpAna.Dock = DockStyle.Fill;
            tlpAna.ColumnCount = 1;
            tlpAna.RowCount = 3;
            tlpAna.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Başlık kısmı
            tlpAna.RowStyles.Add(new RowStyle(SizeType.Percent, 30));   // Üst Kartlar
            tlpAna.RowStyles.Add(new RowStyle(SizeType.Percent, 70));   // Grafikler
            tlpAna.Padding = new Padding(30, 20, 30, 20);
            this.Controls.Add(tlpAna);

            // 1. SATIR: BAŞLIKLAR
            Panel pnlBaslik = new Panel { Dock = DockStyle.Fill };
            Label lblTitle = new Label
            {
                Text = "Sistem İstatistikleri (Özet)",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(0, 0)
            };
            Label lblSubtitle = new Label
            {
                Text = "Veritabanı metrikleri, günlük kullanım oranları ve son sistem olayları.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(0, 35)
            };
            pnlBaslik.Controls.Add(lblTitle);
            pnlBaslik.Controls.Add(lblSubtitle);
            tlpAna.Controls.Add(pnlBaslik, 0, 0);

            // 2. SATIR: KARTLAR (4 Sütunlu)
            TableLayoutPanel tlpKartlar = new TableLayoutPanel();
            tlpKartlar.Dock = DockStyle.Fill;
            tlpKartlar.ColumnCount = 4;
            tlpKartlar.RowCount = 1;
            for (int i = 0; i < 4; i++)
                tlpKartlar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            
            lblKullaniciSayisi = new Label { Text = "0", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent };
            lblBolumSayisi = new Label { Text = "0", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent };
            lblRandevuSayisi = new Label { Text = "0", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent };
            lblHataSayisi = new Label { Text = "0", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, BackColor = Color.Transparent };

            tlpKartlar.Controls.Add(IstatistikKartiOlustur("👥", "Toplam Kullanıcı", lblKullaniciSayisi, ColorTranslator.FromHtml("#3B82F6")), 0, 0);
            tlpKartlar.Controls.Add(IstatistikKartiOlustur("🏥", "Kayıtlı Bölüm", lblBolumSayisi, ColorTranslator.FromHtml("#10B981")), 1, 0);
            tlpKartlar.Controls.Add(IstatistikKartiOlustur("📅", "Toplam Randevu", lblRandevuSayisi, ColorTranslator.FromHtml("#F59E0B")), 2, 0);
            tlpKartlar.Controls.Add(IstatistikKartiOlustur("⚠️", "Bugünkü Randevu", lblHataSayisi, ColorTranslator.FromHtml("#EF4444")), 3, 0);
            
            tlpAna.Controls.Add(tlpKartlar, 0, 1);

            // 3. SATIR: GRAFİKLER (2 Sütunlu)
            TableLayoutPanel tlpGrafikler = new TableLayoutPanel();
            tlpGrafikler.Dock = DockStyle.Fill;
            tlpGrafikler.ColumnCount = 2;
            tlpGrafikler.RowCount = 1;
            tlpGrafikler.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F)); // Grafiğe biraz daha fazla alan
            tlpGrafikler.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            
            pnlGrafikContainer = CreateCardPanel();
            Label lblChartTitle = new Label { Text = "Bölüm Randevu Yoğunluk Grafiği", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true, BackColor = Color.Transparent };
            pnlGrafikContainer.Controls.Add(lblChartTitle);
            
            pnlSonHareketlerContainer = CreateCardPanel();
            Label lblLogTitle = new Label { Text = "Son Sistem Etkinlikleri", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true, BackColor = Color.Transparent };
            pnlSonHareketlerContainer.Controls.Add(lblLogTitle);

            Panel pnlGrafikMargin = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 15, 10, 0) };
            pnlGrafikMargin.Controls.Add(pnlGrafikContainer);

            Panel pnlLogMargin = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 15, 0, 0) };
            pnlLogMargin.Controls.Add(pnlSonHareketlerContainer);

            tlpGrafikler.Controls.Add(pnlGrafikMargin, 0, 0);
            tlpGrafikler.Controls.Add(pnlLogMargin, 1, 0);

            tlpAna.Controls.Add(tlpGrafikler, 0, 2);
        }

        private Panel CreateCardPanel()
        {
            Panel pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SafBeyaz
            };
            pnl.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalKose(pnl.Width - 1, pnl.Height - 1, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    e.Graphics.DrawPath(pen, path);
            };
            pnl.Resize += (s, e) =>
            {
                if (pnl.Width > 0 && pnl.Height > 0)
                {
                    using (GraphicsPath path = OvalKose(pnl.Width, pnl.Height, 15))
                        pnl.Region = new Region(path);
                }
                pnl.Invalidate();
            };
            return pnl;
        }

        private Panel IstatistikKartiOlustur(string ikon, string baslik, Label lblDeger, Color ikonRengi)
        {
            Panel pnlMargin = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            Panel kart = CreateCardPanel();

            Label lblIkon = new Label { Text = ikon, Font = new Font("Segoe UI Emoji", 26), ForeColor = ikonRengi, Location = new Point(15, 20), AutoSize = true, BackColor = Color.Transparent };
            kart.Controls.Add(lblIkon);

            Label lblBas = new Label { Text = baslik, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(100, 25), AutoSize = true, BackColor = Color.Transparent };
            kart.Controls.Add(lblBas);

            lblDeger.Location = new Point(100, 50);
            kart.Controls.Add(lblDeger);

            pnlMargin.Controls.Add(kart);
            return pnlMargin;
        }

        private void LoadStatistics()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                
                // Toplam Kullanıcı
                object userCountObj = db.ExecuteScalar("sp_Istatistik_KullaniciSayisi");
                lblKullaniciSayisi.Text = userCountObj?.ToString() ?? "0";

                // Kayıtlı Bölüm
                object bolumCountObj = db.ExecuteScalar("sp_Istatistik_BolumSayisi");
                lblBolumSayisi.Text = bolumCountObj?.ToString() ?? "0";

                // Toplam Randevu
                object randevuCountObj = db.ExecuteScalar("sp_Istatistik_RandevuSayisi");
                lblRandevuSayisi.Text = randevuCountObj?.ToString() ?? "0";

                // Bugünkü Randevular
                object bugunkuRandevuObj = db.ExecuteScalar("sp_Istatistik_BugunRandevu");
                lblHataSayisi.Text = bugunkuRandevuObj?.ToString() ?? "0";

                // Grafikleri Yükle
                LoadCharts(db);
                LoadTimeline(db);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanına bağlanırken bir hata oluştu:\n\n" + ex.Message, "Veri Yükleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCharts(SqlHelper db)
        {
            DataTable dtBolumler = new DataTable();
            try
            {
                dtBolumler = db.GetTable("sp_Istatistik_BolumYogunluklari");
            }
            catch { }

            int maxVal = 1; 
            foreach (DataRow row in dtBolumler.Rows)
            {
                int val = Convert.ToInt32(row["RandevuSayisi"]);
                if (val > maxVal) maxVal = val;
            }

            if (maxVal == 1 && dtBolumler.Rows.Count == 0)
            {
                if(dtBolumler.Columns.Count == 0)
                {
                    dtBolumler.Columns.Add("bolum_adi");
                    dtBolumler.Columns.Add("RandevuSayisi", typeof(int));
                }
                dtBolumler.Rows.Add("Dahiliye (Örnek)", 145);
                dtBolumler.Rows.Add("Göz Hastalıkları", 120);
                dtBolumler.Rows.Add("Nöroloji", 95);
                dtBolumler.Rows.Add("Kardiyoloji", 80);
                dtBolumler.Rows.Add("Ortopedi", 65);
                maxVal = 145;
            }

            Panel pnlCustomGraph = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20, 60, 20, 20), BackColor = Color.Transparent };
            
            pnlCustomGraph.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                int baslangicY = 70;
                int satirYuksekligi = 55;
                int textGenislik = 140;
                int numaraGenislik = 60;

                int calismaAlaniW = pnlCustomGraph.Width - 40;
                int barMaxGenislik = calismaAlaniW - textGenislik - numaraGenislik - 20;
                
                if (barMaxGenislik < 10) return;

                for(int i=0; i<dtBolumler.Rows.Count; i++)
                {
                    DataRow row = dtBolumler.Rows[i];
                    string ad = row["bolum_adi"].ToString();
                    if (ad.Length > 18) ad = ad.Substring(0, 15) + "...";
                    
                    int val = Convert.ToInt32(row["RandevuSayisi"]);
                    float barYuzde = (float)val / maxVal;
                    if (barYuzde < 0.05f && val > 0) barYuzde = 0.05f; 
                    int drawW = (int)(barYuzde * barMaxGenislik);
                    if (drawW == 0 && val > 0) drawW = 5;

                    int cY = baslangicY + (i * satirYuksekligi);

                    TextRenderer.DrawText(e.Graphics, ad, new Font("Segoe UI", 11), new Rectangle(20, cY, textGenislik, 24), TextDark, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                    Rectangle bgRect = new Rectangle(20 + textGenislik, cY, barMaxGenislik, 24);
                    using (GraphicsPath pBg = OvalKose(bgRect.Width, bgRect.Height, 10))
                    {
                        e.Graphics.TranslateTransform(bgRect.X, bgRect.Y);
                        using (SolidBrush bBg = new SolidBrush(ColorTranslator.FromHtml("#E2E8F0")))
                            e.Graphics.FillPath(bBg, pBg);
                        e.Graphics.ResetTransform();
                    }

                    if (drawW > 0)
                    {
                        Rectangle fgRect = new Rectangle(20 + textGenislik, cY, drawW, 24);
                        using (GraphicsPath pFg = OvalKose(fgRect.Width, fgRect.Height, 10))
                        {
                            e.Graphics.TranslateTransform(fgRect.X, fgRect.Y);
                            using (LinearGradientBrush lgb = new LinearGradientBrush(new Rectangle(0,0,fgRect.Width,fgRect.Height), ColorTranslator.FromHtml("#38BDF8"), ColorTranslator.FromHtml("#0284C7"), LinearGradientMode.Horizontal))
                                e.Graphics.FillPath(lgb, pFg);
                            e.Graphics.ResetTransform();
                        }
                    }

                    Rectangle valRect = new Rectangle(20 + textGenislik + barMaxGenislik + 10, cY, numaraGenislik, 24);
                    TextRenderer.DrawText(e.Graphics, val.ToString(), new Font("Segoe UI", 11, FontStyle.Bold), valRect, VurguRengi, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
            };
            
            pnlCustomGraph.Resize += (s, e) => pnlCustomGraph.Invalidate();
            pnlGrafikContainer.Controls.Add(pnlCustomGraph);
        }

        private void LoadTimeline(SqlHelper db)
        {
            DataTable dtLoglar = new DataTable();
            try
            {
                dtLoglar = db.GetTable("sp_Istatistik_SonHareketler");
            }
            catch { }

            if (dtLoglar.Rows.Count == 0)
            {
                if(dtLoglar.Columns.Count == 0)
                {
                    dtLoglar.Columns.Add("islem_tipi");
                    dtLoglar.Columns.Add("islem_tarihi", typeof(DateTime));
                    dtLoglar.Columns.Add("aciklama");
                }
                dtLoglar.Rows.Add("Sistem (Örnek)", DateTime.Now, "Sistem başlatıldı.");
                dtLoglar.Rows.Add("Kullanıcı", DateTime.Now.AddMinutes(-15), "Yeni doktor eklendi.");
                dtLoglar.Rows.Add("Randevu", DateTime.Now.AddHours(-1), "Ahmet Yılmaz randevu aldı.");
                dtLoglar.Rows.Add("Hata", DateTime.Now.AddHours(-3), "Veritabanı bağlantı hatası giderildi.");
                dtLoglar.Rows.Add("Rapor", DateTime.Now.AddDays(-1), "Aylık istatistik raporu oluşturuldu.");
            }

            TableLayoutPanel tlpLogs = new TableLayoutPanel();
            tlpLogs.Dock = DockStyle.Fill;
            tlpLogs.RowCount = dtLoglar.Rows.Count;
            tlpLogs.ColumnCount = 2;
            tlpLogs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35));
            tlpLogs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlpLogs.Padding = new Padding(20, 60, 20, 20);

            int rowIndex = 0;
            foreach (DataRow row in dtLoglar.Rows)
            {
                tlpLogs.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / dtLoglar.Rows.Count));

                string tip = row["islem_tipi"].ToString();
                DateTime tarih = Convert.ToDateTime(row["islem_tarihi"]);
                string aciklama = row["aciklama"].ToString();

                Color dotColor = ColorTranslator.FromHtml("#3B82F6"); 
                if (tip == "Hata" || tip == "Silme") dotColor = ColorTranslator.FromHtml("#EF4444"); 
                else if (tip == "Randevu" || tip == "Kayıt") dotColor = ColorTranslator.FromHtml("#10B981"); 
                else if (tip == "Kullanıcı" || tip == "Güncelleme") dotColor = ColorTranslator.FromHtml("#F59E0B"); 

                Panel pnlDot = new Panel
                {
                    Size = new Size(16, 16),
                    Margin = new Padding(5, 10, 0, 0),
                    BackColor = dotColor
                };
                pnlDot.Paint += (s, e) => {
                    using (GraphicsPath path = OvalKose(pnlDot.Width, pnlDot.Height, 8))
                        pnlDot.Region = new Region(path);
                };
                tlpLogs.Controls.Add(pnlDot, 0, rowIndex);

                Panel pnlMetin = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                Label lblZaman = new Label { Text = tarih.ToString("dd.MM HH:mm"), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(0, 0), AutoSize = true, BackColor = Color.Transparent };
                Label lblMetin = new Label { Text = aciklama, Font = new Font("Segoe UI", 10), ForeColor = TextDark, Location = new Point(0, 20), AutoSize = true, BackColor = Color.Transparent, MaximumSize = new Size(250, 0) };
                
                pnlMetin.Controls.Add(lblZaman);
                pnlMetin.Controls.Add(lblMetin);
                tlpLogs.Controls.Add(pnlMetin, 1, rowIndex);

                rowIndex++;
            }
            pnlSonHareketlerContainer.Controls.Add(tlpLogs);
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
