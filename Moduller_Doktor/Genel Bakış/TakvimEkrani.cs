using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HastaneYonetim.Moduller_Doktor
{
    public partial class TakvimEkrani : UserControl
    {
        // Renk Paleti (Ana tasarımla birebir uyumlu ve eksiksiz)
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color MedikalMavi = ColorTranslator.FromHtml("#4318FF");
        Color AcikMavi = ColorTranslator.FromHtml("#38BDF8");
        Color BasariYesili = ColorTranslator.FromHtml("#05CD99");
        Color UyariTuruncusu = ColorTranslator.FromHtml("#FFCE20");

        Label lblTitle;
        Panel pnlGunlukTablo, pnlHaftalikGrafik, pnlDurumGrafik;
        DataGridView dgvGunluk;
        Chart chartHaftalik, chartDurum;

        public TakvimEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;

            EkraniInsaEt();

            // Verileri ekran tam yüklendiğinde çeker (Hata Önleyici)
            this.Load += (s, e) => VerileriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            YerlesimiGuncelle();
        }

        private void YerlesimiGuncelle()
        {
            // YENİ EKLENEN GÜVENLİK KİLİDİ: Paneller oluşmadan boyutlandırmayı engeller
            if (pnlGunlukTablo == null || pnlHaftalikGrafik == null || pnlDurumGrafik == null)
                return;

            int margin = 30;
            int spacing = 25;
            int w = this.ClientSize.Width - (margin * 2);
            int h = this.ClientSize.Height - (margin * 2);

            if (w < 900) w = 900;
            if (h < 600) h = 600;

            if (lblTitle != null) lblTitle.Location = new Point(margin, margin);

            int topOffset = margin + 50;
            int availableHeight = h - topOffset + margin;

            // Sol taraf %60 (Günlük Saatli Takvim)
            int solGenislik = (int)(w * 0.60);

            // Sağ taraf %40 (Grafikler)
            int sagGenislik = w - solGenislik - spacing;

            pnlGunlukTablo.Bounds = new Rectangle(margin, topOffset, solGenislik, availableHeight);

            // Sağ Tarafı Üst ve Alt Olarak İkiye Böl
            int grafikYukseklik = (availableHeight - spacing) / 2;
            pnlHaftalikGrafik.Bounds = new Rectangle(margin + solGenislik + spacing, topOffset, sagGenislik, grafikYukseklik);
            pnlDurumGrafik.Bounds = new Rectangle(margin + solGenislik + spacing, topOffset + grafikYukseklik + spacing, sagGenislik, grafikYukseklik);

            // İç bileşenleri panellere uydurma
            if (dgvGunluk != null) dgvGunluk.Bounds = new Rectangle(0, 50, solGenislik, availableHeight - 50);
            if (chartHaftalik != null) chartHaftalik.Bounds = new Rectangle(10, 45, sagGenislik - 20, grafikYukseklik - 55);
            if (chartDurum != null) chartDurum.Bounds = new Rectangle(10, 45, sagGenislik - 20, grafikYukseklik - 55);
        }

        private void EkraniInsaEt()
        {
            lblTitle = new Label
            {
                Text = "Günlük Takvim ve Çalışma Planı",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // 1. SOL: Günlük Takvim Tablosu
            pnlGunlukTablo = GrafikPaneliOlustur($"Bugünün Randevuları ({DateTime.Now.ToString("dd MMMM yyyy")})");
            dgvGunluk = ModernDataGridOlustur();
            dgvGunluk.DataBindingComplete += DgvGunluk_DataBindingComplete;
            pnlGunlukTablo.Controls.Add(dgvGunluk);
            this.Controls.Add(pnlGunlukTablo);

            // 2. SAĞ ÜST: Haftalık Yoğunluk (Sütun Grafiği)
            pnlHaftalikGrafik = GrafikPaneliOlustur("Haftalık Randevu Dağılımı");
            OlusturHaftalikGrafik(pnlHaftalikGrafik);
            this.Controls.Add(pnlHaftalikGrafik);

            // 3. SAĞ ALT: Randevu Durumları (Pasta Grafiği)
            pnlDurumGrafik = GrafikPaneliOlustur("Bugünkü Randevu Durumları");
            OlusturDurumGrafigi(pnlDurumGrafik);
            this.Controls.Add(pnlDurumGrafik);
        }

        private void DgvGunluk_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvGunluk.Columns.Count > 0)
            {
                dgvGunluk.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvGunluk.Columns[0].Width = 70; // Saat
                if (dgvGunluk.Columns.Count > 3)
                {
                    dgvGunluk.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgvGunluk.Columns[3].Width = 140; // Durum
                }
            }
            dgvGunluk.ClearSelection();
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object>() {
                    { "@kullanici_id", Program.AktifKullaniciID }
                };

                // Günlük Tabloyu Doldur
                DataTable dtGunluk = db.GetTable("sp_DoktorGunlukTakvim", prm);
                dgvGunluk.DataSource = dtGunluk;

                // Haftalık Grafiği Doldur
                DataTable dtHaftalik = db.GetTable("sp_DoktorHaftalikGrafik", prm);
                chartHaftalik.Series[0].Points.Clear();
                foreach (DataRow row in dtHaftalik.Rows)
                {
                    chartHaftalik.Series[0].Points.AddXY(row["Gun"].ToString(), Convert.ToInt32(row["RandevuSayisi"]));
                }

                // Pasta Grafiğini Doldur (Bugünkü tablodan hesapla)
                int tamamlanan = 0;
                int bekleyen = 0;
                foreach (DataRow row in dtGunluk.Rows)
                {
                    if (row["Durum"].ToString() == "Tamamlandı") tamamlanan++;
                    else bekleyen++;
                }

                chartDurum.Series[0].Points.Clear();

                // Eğer hiç randevu yoksa grafik boş kalmasın diye uyarı
                if (tamamlanan == 0 && bekleyen == 0)
                {
                    chartDurum.Series[0].Points.AddXY("Randevu Yok", 1);
                    chartDurum.Series[0].Points[0].Color = ColorTranslator.FromHtml("#E2E8F0");
                }
                else
                {
                    chartDurum.Series[0].Points.AddXY("Tamamlandı", tamamlanan);
                    chartDurum.Series[0].Points.AddXY("Bekleyen", bekleyen);
                    chartDurum.Series[0].Points[0].Color = BasariYesili;
                    chartDurum.Series[0].Points[1].Color = UyariTuruncusu;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Takvim verileri yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // GÖRSEL ÜRETİCİLER
        // ==========================================
        private Panel GrafikPaneliOlustur(string baslik)
        {
            Panel pnl = new Panel { BackColor = SafBeyaz };
            pnl.Resize += (s, e) => OvalKirp(pnl, 20);
            pnl.Paint += (s, e) => InceCerceveCiz(pnl, e.Graphics, 20);

            Label lbl = new Label { Text = baslik, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 15), AutoSize = true };
            pnl.Controls.Add(lbl);

            return pnl;
        }

        private void OlusturHaftalikGrafik(Panel parent)
        {
            chartHaftalik = new Chart { BackColor = Color.Transparent };
            ChartArea ca = new ChartArea { BackColor = Color.Transparent };
            ca.AxisX.MajorGrid.LineWidth = 0;
            ca.AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#E2E8F0");
            ca.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            ca.AxisX.LabelStyle.ForeColor = TextMuted;
            ca.AxisY.LabelStyle.ForeColor = TextMuted;
            ca.AxisX.LineColor = ColorTranslator.FromHtml("#CBD5E1");
            ca.AxisY.LineColor = Color.Transparent;
            chartHaftalik.ChartAreas.Add(ca);

            Series s1 = new Series("Randevular")
            {
                ChartType = SeriesChartType.Column,
                Color = MedikalMavi,
                BackSecondaryColor = AcikMavi,
                BackGradientStyle = GradientStyle.TopBottom,
                BorderWidth = 0
            };
            s1["PixelPointWidth"] = "20";

            chartHaftalik.Series.Add(s1);
            parent.Controls.Add(chartHaftalik);
        }

        private void OlusturDurumGrafigi(Panel parent)
        {
            chartDurum = new Chart { BackColor = Color.Transparent };
            ChartArea ca = new ChartArea { BackColor = Color.Transparent };
            chartDurum.ChartAreas.Add(ca);

            Series s1 = new Series("Durum")
            {
                ChartType = SeriesChartType.Doughnut
            };
            s1["DoughnutRadius"] = "50";
            s1.BorderColor = SafBeyaz;
            s1.BorderWidth = 3;

            Legend l = new Legend { BackColor = Color.Transparent, ForeColor = TextDark, Font = new Font("Segoe UI", 10) };
            chartDurum.Legends.Add(l);
            chartDurum.Series.Add(s1);
            parent.Controls.Add(chartDurum);
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
                g.DrawPath(pen, path);
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