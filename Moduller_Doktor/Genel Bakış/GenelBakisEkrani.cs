using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HastaneYonetim.Moduller_Doktor
{
    public partial class GenelBakisEkrani : UserControl
    {
        // Renk Paleti
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color MedikalMavi = ColorTranslator.FromHtml("#4318FF");
        Color AcikMavi = ColorTranslator.FromHtml("#38BDF8");
        Color BasariYesili = ColorTranslator.FromHtml("#05CD99");
        Color UyariTuruncusu = ColorTranslator.FromHtml("#FFCE20");

        Label lblTitle;
        Panel kart1, kart2, kart3, kart4;
        Panel pnlBarChart, pnlPieChart, pnlTablo1, pnlTablo2;
        Chart barChart, pieChart;
        DataGridView dgvHastalar, dgvTahliller;

        // Değer Etiketleri
        Label lblToplamHasta, lblBugunRandevu, lblDoluBosOda, lblBekleyenTahlil;

        public GenelBakisEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;

            EkraniInsaEt();

            // Verileri ekran arka planda oluşurken değil, tam çizildiği anda çeker (Hata Önleyici)
            this.Load += (s, e) => VerileriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            YerlesimiGuncelle();
        }

        private void YerlesimiGuncelle()
        {
            if (kart1 == null) return;

            int margin = 30;
            int spacing = 25;

            int w = this.ClientSize.Width - (margin * 2);
            int h = this.ClientSize.Height - (margin * 2);

            if (w < 900) w = 900;
            if (h < 600) h = 600;

            lblTitle.Location = new Point(margin, margin);

            int topOffset = margin + 50;
            int cardW = (w - (spacing * 3)) / 4;
            int cardH = 120;

            kart1.Bounds = new Rectangle(margin, topOffset, cardW, cardH);
            kart2.Bounds = new Rectangle(margin + cardW + spacing, topOffset, cardW, cardH);
            kart3.Bounds = new Rectangle(margin + (cardW + spacing) * 2, topOffset, cardW, cardH);
            kart4.Bounds = new Rectangle(margin + (cardW + spacing) * 3, topOffset, cardW, cardH);

            int row2Y = topOffset + cardH + spacing;
            int row2H = (int)((h - topOffset - cardH - spacing * 2) * 0.55);

            int barW = (int)(w * 0.65) - (spacing / 2);
            int pieW = w - barW - spacing;

            pnlBarChart.Bounds = new Rectangle(margin, row2Y, barW, row2H);
            pnlPieChart.Bounds = new Rectangle(margin + barW + spacing, row2Y, pieW, row2H);

            if (barChart != null) barChart.Bounds = new Rectangle(10, 45, barW - 20, row2H - 55);
            if (pieChart != null) pieChart.Bounds = new Rectangle(10, 45, pieW - 20, row2H - 55);

            int row3Y = row2Y + row2H + spacing;
            int row3H = h - row3Y + margin;
            int tableW = (w - spacing) / 2;

            pnlTablo1.Bounds = new Rectangle(margin, row3Y, tableW, row3H);
            pnlTablo2.Bounds = new Rectangle(margin + tableW + spacing, row3Y, tableW, row3H);

            if (dgvHastalar != null) dgvHastalar.Bounds = new Rectangle(0, 50, tableW, row3H - 50);
            if (dgvTahliller != null) dgvTahliller.Bounds = new Rectangle(0, 50, tableW, row3H - 50);
        }

        private void EkraniInsaEt()
        {
            string drIsim = string.IsNullOrEmpty(Program.AktifKullaniciAdSoyad) ? "Uzman Hekim" : Program.AktifKullaniciAdSoyad;
            lblTitle = new Label
            {
                Text = $"Hoş Geldiniz, Dr. {drIsim}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Kartlar (2. Kart Bugünkü Randevular olarak değiştirildi!)
            kart1 = KartOlustur("Toplam Hastanız", "0", "👥", MedikalMavi, out lblToplamHasta);
            kart2 = KartOlustur("Bugünkü Randevular", "0", "📅", AcikMavi, out lblBugunRandevu);
            kart3 = KartOlustur("Dolu / Boş Oda", "0 / 0", "🛏", BasariYesili, out lblDoluBosOda);
            kart4 = KartOlustur("Bekleyen Tahlil", "0", "⚗", UyariTuruncusu, out lblBekleyenTahlil);

            this.Controls.Add(kart1);
            this.Controls.Add(kart2);
            this.Controls.Add(kart3);
            this.Controls.Add(kart4);

            pnlBarChart = GrafikPaneliOlustur("Haftalık Hasta Trafiğiniz");
            OlusturBarGrafik(pnlBarChart);
            this.Controls.Add(pnlBarChart);

            pnlPieChart = GrafikPaneliOlustur("Hasta Cinsiyet Dağılımı");
            OlusturPieGrafik(pnlPieChart);
            this.Controls.Add(pnlPieChart);

            pnlTablo1 = GrafikPaneliOlustur("Sıradaki Bekleyen Hastalar");
            dgvHastalar = ModernDataGridOlustur();
            pnlTablo1.Controls.Add(dgvHastalar);
            this.Controls.Add(pnlTablo1);

            pnlTablo2 = GrafikPaneliOlustur("Tahlil Sonuçları (Güncel)");
            dgvTahliller = ModernDataGridOlustur();
            pnlTablo2.Controls.Add(dgvTahliller);
            this.Controls.Add(pnlTablo2);
        }

        // ==========================================
        // VERİTABANI BAĞLANTISI VE VERİ DOLDURMA
        // ==========================================
        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                // SQL prosedürlerine oturumdaki (login olan) doktorun IDsini gönderiyoruz
                Dictionary<string, object> prm = new Dictionary<string, object>() {
                    { "@kullanici_id", Program.AktifKullaniciID }
                };

                // 1. KARTLARI DOLDUR
                DataTable dtKart = db.GetTable("sp_DoktorDashboard_Kartlar", prm);
                if (dtKart.Rows.Count > 0)
                {
                    lblToplamHasta.Text = dtKart.Rows[0]["ToplamHasta"].ToString();
                    lblBugunRandevu.Text = dtKart.Rows[0]["BugunRandevu"].ToString();
                    lblDoluBosOda.Text = dtKart.Rows[0]["OdaDurum"].ToString();
                    lblBekleyenTahlil.Text = dtKart.Rows[0]["BekleyenTahlil"].ToString();
                }

                // 2. SÜTUN GRAFİĞİ DOLDUR
                DataTable dtBar = db.GetTable("sp_DoktorDashboard_GrafikBar", prm);
                barChart.Series[0].Points.Clear(); // Sahte verileri temizle
                foreach (DataRow row in dtBar.Rows)
                {
                    barChart.Series[0].Points.AddXY(row["Gun"].ToString(), Convert.ToInt32(row["MuayeneSayisi"]));
                }

                // 3. PASTA GRAFİĞİ DOLDUR
                DataTable dtPie = db.GetTable("sp_DoktorDashboard_GrafikPie", prm);
                pieChart.Series[0].Points.Clear(); // Sahte verileri temizle
                foreach (DataRow row in dtPie.Rows)
                {
                    pieChart.Series[0].Points.AddXY(row["Cinsiyet"].ToString(), Convert.ToInt32(row["Sayi"]));
                }

                // Pasta grafik renklerini cinsiyete göre sabitle (Opsiyonel)
                if (pieChart.Series[0].Points.Count == 2)
                {
                    pieChart.Series[0].Points[0].Color = MedikalMavi;
                    pieChart.Series[0].Points[1].Color = AcikMavi;
                }

                // 4. TABLOLARI DOLDUR (Sütunlar otomatik SQL'den gelecek)
                dgvHastalar.DataSource = db.GetTable("sp_DoktorDashboard_TabloBekleyen", prm);
                dgvTahliller.DataSource = db.GetTable("sp_DoktorDashboard_TabloTahlil", prm);

                // Tablo sütunları yüklendikten sonra tasarımlarını toparla
                if (dgvHastalar.Columns.Count > 0)
                {
                    dgvHastalar.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // Otomatik esnemeyi durdur
                    dgvHastalar.Columns[0].Width = 70; // Saat sütunu (Şimdi güvenle boyutlandırılabilir)
                }

                if (dgvTahliller.Columns.Count > 0)
                {
                    dgvTahliller.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // Otomatik esnemeyi durdur
                    dgvTahliller.Columns[0].Width = 90; // Tarih sütunu
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard verileri çekilirken bir hata oluştu: " + ex.Message, "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- GÖRSEL BİLEŞEN ÜRETİCİLERİ ---
        private Panel KartOlustur(string baslik, string deger, string ikon, Color renk, out Label degerLabel)
        {
            Panel pnl = new Panel { BackColor = SafBeyaz };
            pnl.Resize += (s, e) => OvalKirp(pnl, 20);
            pnl.Paint += (s, e) => InceCerceveCiz(pnl, e.Graphics, 20);

            Panel pnlIkon = new Panel { Location = new Point(20, 25), Size = new Size(60, 60), BackColor = Color.Transparent };
            pnlIkon.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush bg = new SolidBrush(Color.FromArgb(30, renk.R, renk.G, renk.B)))
                    e.Graphics.FillEllipse(bg, 0, 0, 60, 60);
                TextRenderer.DrawText(e.Graphics, ikon, new Font("Segoe UI Emoji", 20), new Rectangle(0, 0, 60, 60), renk, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            pnl.Controls.Add(pnlIkon);

            Label lblBaslik = new Label { Text = baslik, Font = new Font("Segoe UI", 11, FontStyle.Regular), ForeColor = TextMuted, Location = new Point(95, 25), AutoSize = true };
            pnl.Controls.Add(lblBaslik);

            degerLabel = new Label { Text = deger, Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = TextDark, Location = new Point(90, 45), AutoSize = true };
            pnl.Controls.Add(degerLabel);

            return pnl;
        }

        private Panel GrafikPaneliOlustur(string baslik)
        {
            Panel pnl = new Panel { BackColor = SafBeyaz };
            pnl.Resize += (s, e) => OvalKirp(pnl, 20);
            pnl.Paint += (s, e) => InceCerceveCiz(pnl, e.Graphics, 20);

            Label lbl = new Label { Text = baslik, Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 15), AutoSize = true };
            pnl.Controls.Add(lbl);

            return pnl;
        }

        private void OlusturBarGrafik(Panel parent)
        {
            barChart = new Chart();
            barChart.BackColor = Color.Transparent;

            ChartArea ca = new ChartArea();
            ca.BackColor = Color.Transparent;
            ca.AxisX.MajorGrid.LineWidth = 0;
            ca.AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#E2E8F0");
            ca.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            ca.AxisX.LabelStyle.ForeColor = TextMuted;
            ca.AxisY.LabelStyle.ForeColor = TextMuted;
            ca.AxisX.LineColor = ColorTranslator.FromHtml("#CBD5E1");
            ca.AxisY.LineColor = Color.Transparent;
            barChart.ChartAreas.Add(ca);

            Series s1 = new Series("Muayeneler");
            s1.ChartType = SeriesChartType.Column;
            s1.Color = MedikalMavi;
            s1.BackSecondaryColor = AcikMavi;
            s1.BackGradientStyle = GradientStyle.TopBottom;
            s1.BorderWidth = 0;
            s1["PixelPointWidth"] = "25";

            barChart.Series.Add(s1);
            parent.Controls.Add(barChart);
        }

        private void OlusturPieGrafik(Panel parent)
        {
            pieChart = new Chart();
            pieChart.BackColor = Color.Transparent;

            ChartArea ca = new ChartArea();
            ca.BackColor = Color.Transparent;
            pieChart.ChartAreas.Add(ca);

            Series s1 = new Series("Cinsiyet");
            s1.ChartType = SeriesChartType.Doughnut;
            s1["DoughnutRadius"] = "60";

            s1.BorderColor = SafBeyaz;
            s1.BorderWidth = 3;

            Legend l = new Legend();
            l.BackColor = Color.Transparent;
            l.ForeColor = TextDark;
            l.Font = new Font("Segoe UI", 11);
            pieChart.Legends.Add(l);
            pieChart.Series.Add(s1);
            parent.Controls.Add(pieChart);
        }

        private DataGridView ModernDataGridOlustur()
        {
            DataGridView dgv = new DataGridView();
            dgv.BackgroundColor = SafBeyaz;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#E0F2FE");
            dgv.DefaultCellStyle.SelectionForeColor = TextDark;
            dgv.DefaultCellStyle.BackColor = SafBeyaz;
            dgv.DefaultCellStyle.ForeColor = TextDark;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = SafBeyaz;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 45;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Margin = new Padding(0);
            return dgv;
        }

        private void OvalKirp(Panel pnl, int radius)
        {
            if (pnl.Width > 0 && pnl.Height > 0)
            {
                using (GraphicsPath path = TamKoseOval(pnl.Width, pnl.Height, radius))
                {
                    if (pnl.Region != null) pnl.Region.Dispose();
                    pnl.Region = new Region(path);
                }
                pnl.Invalidate();
            }
        }

        private void InceCerceveCiz(Panel pnl, Graphics g, int radius)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = TamKoseOval(pnl.Width, pnl.Height, radius))
            using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
            {
                g.DrawPath(pen, path);
            }
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
    }
}