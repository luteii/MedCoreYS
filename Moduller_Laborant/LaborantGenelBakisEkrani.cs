using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HastaneYonetim.Moduller_Laborant
{
    public partial class LaborantGenelBakisEkrani : UserControl
    {
        // Renk Paleti
        Color AnaZemin = ColorTranslator.FromHtml("#F8FAFC");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color MedikalTeal = ColorTranslator.FromHtml("#0D9488");
        Color TealAcik = ColorTranslator.FromHtml("#5EEAD4");
        Color Turuncu = ColorTranslator.FromHtml("#F59E0B");

        Label lblTitle;
        Panel kart1, kart2, kart3;
        Panel pnlBarChart, pnlPieChart;
        Chart barChart, pieChart;

        // Değer Etiketleri
        Label lblBekleyen, lblBugunTamamlanan, lblToplamTamamlanan;

        public LaborantGenelBakisEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;

            EkraniInsaEt();

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

            if (w < 800) w = 800;
            if (h < 500) h = 500;

            lblTitle.Location = new Point(margin, margin);

            int topOffset = margin + 50;
            int cardW = (w - (spacing * 2)) / 3;
            int cardH = 130;

            kart1.Bounds = new Rectangle(margin, topOffset, cardW, cardH);
            kart2.Bounds = new Rectangle(margin + cardW + spacing, topOffset, cardW, cardH);
            kart3.Bounds = new Rectangle(margin + (cardW + spacing) * 2, topOffset, cardW, cardH);

            int chartY = topOffset + cardH + spacing;
            int chartH = h - chartY + margin;

            int barW = (int)(w * 0.60) - (spacing / 2);
            int pieW = w - barW - spacing;

            pnlBarChart.Bounds = new Rectangle(margin, chartY, barW, chartH);
            pnlPieChart.Bounds = new Rectangle(margin + barW + spacing, chartY, pieW, chartH);

            if (barChart != null) barChart.Bounds = new Rectangle(10, 45, barW - 20, chartH - 55);
            if (pieChart != null) pieChart.Bounds = new Rectangle(10, 45, pieW - 20, chartH - 55);
        }

        private void EkraniInsaEt()
        {
            lblTitle = new Label { Text = "Laboratuvar Genel Bakış", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = TextDark, AutoSize = true };
            this.Controls.Add(lblTitle);

            // ================= KARTLAR =================
            kart1 = BilgiKartiOlustur("Bekleyen Tahliller", "0", Turuncu, out lblBekleyen);
            kart2 = BilgiKartiOlustur("Bugün Tamamlanan", "0", MedikalTeal, out lblBugunTamamlanan);
            kart3 = BilgiKartiOlustur("Toplam Tamamlanan", "0", ColorTranslator.FromHtml("#3B82F6"), out lblToplamTamamlanan);

            this.Controls.Add(kart1);
            this.Controls.Add(kart2);
            this.Controls.Add(kart3);

            // ================= GRAFİK PANELLERİ =================
            pnlBarChart = GrafikPaneliOlustur("Son 7 Günlük Tahlil Yoğunluğu");
            pnlPieChart = GrafikPaneliOlustur("İstenen Tahlillerin Dağılımı");

            this.Controls.Add(pnlBarChart);
            this.Controls.Add(pnlPieChart);

            // ================= GRAFİK KONTROLLERİ =================
            barChart = new Chart();
            barChart.BackColor = SafBeyaz;
            ChartArea ca1 = new ChartArea();
            ca1.BackColor = SafBeyaz;
            ca1.AxisX.MajorGrid.Enabled = false;
            ca1.AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#E2E8F0");
            ca1.AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
            ca1.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
            barChart.ChartAreas.Add(ca1);
            Series s1 = new Series();
            s1.ChartType = SeriesChartType.Column;
            s1.Color = MedikalTeal;
            s1.BorderWidth = 0;
            s1.IsValueShownAsLabel = true;
            s1.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            barChart.Series.Add(s1);
            pnlBarChart.Controls.Add(barChart);

            pieChart = new Chart();
            pieChart.BackColor = SafBeyaz;
            ChartArea ca2 = new ChartArea();
            ca2.BackColor = SafBeyaz;
            pieChart.ChartAreas.Add(ca2);
            Series s2 = new Series();
            s2.ChartType = SeriesChartType.Doughnut;
            s2.CustomProperties = "DoughnutRadius=40";
            s2.IsValueShownAsLabel = true;
            s2.LabelForeColor = SafBeyaz;
            s2.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            pieChart.Series.Add(s2);
            Legend l2 = new Legend();
            l2.Font = new Font("Segoe UI", 9);
            l2.Docking = Docking.Bottom;
            l2.Alignment = StringAlignment.Center;
            pieChart.Legends.Add(l2);
            pnlPieChart.Controls.Add(pieChart);
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                
                // 1. Özet Kartlar
                DataTable dtOzet = db.GetTable("sp_LaborantGenelBakisOzet");
                if (dtOzet.Rows.Count > 0)
                {
                    lblBekleyen.Text = dtOzet.Rows[0]["BekleyenSayisi"].ToString();
                    lblBugunTamamlanan.Text = dtOzet.Rows[0]["BugunTamamlanan"].ToString();
                    lblToplamTamamlanan.Text = dtOzet.Rows[0]["ToplamTamamlanan"].ToString();
                }

                // 2. Bar Chart
                DataTable dtHafta = db.GetTable("sp_LaborantGenelBakisHaftalikGidisat");
                barChart.Series[0].Points.Clear();
                foreach (DataRow row in dtHafta.Rows)
                {
                    string gun = Convert.ToDateTime(row["Tarih"]).ToString("dd MMM");
                    int miktar = Convert.ToInt32(row["IslemSayisi"]);
                    barChart.Series[0].Points.AddXY(gun, miktar);
                }

                // 3. Pie Chart
                DataTable dtDagilim = db.GetTable("sp_LaborantGenelBakisTahlilDagilimi");
                pieChart.Series[0].Points.Clear();
                foreach (DataRow row in dtDagilim.Rows)
                {
                    string tahlil = row["TahlilTuru"].ToString();
                    int miktar = Convert.ToInt32(row["Miktar"]);
                    pieChart.Series[0].Points.AddXY(tahlil, miktar);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel BilgiKartiOlustur(string baslik, string deger, Color iconRenk, out Label lblDeger)
        {
            Panel p = new Panel { BackColor = SafBeyaz };
            p.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(p.Width, p.Height, 20))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                { p.Region = new Region(path); e.Graphics.DrawPath(pen, path); }

                // Renkli yan çizgi
                using (SolidBrush b = new SolidBrush(iconRenk))
                {
                    e.Graphics.FillRectangle(b, new Rectangle(0, 20, 5, p.Height - 40));
                }
            };

            Label lblB = new Label { Text = baslik, Font = new Font("Segoe UI", 12, FontStyle.Regular), ForeColor = TextMuted, AutoSize = true, Location = new Point(20, 20) };
            p.Controls.Add(lblB);

            lblDeger = new Label { Text = deger, Font = new Font("Segoe UI", 26, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(20, 50) };
            p.Controls.Add(lblDeger);

            return p;
        }

        private Panel GrafikPaneliOlustur(string baslik)
        {
            Panel p = new Panel { BackColor = SafBeyaz };
            p.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(p.Width, p.Height, 20))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                { p.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };

            Label lblB = new Label { Text = baslik, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(20, 15) };
            p.Controls.Add(lblB);

            return p;
        }

        private GraphicsPath OvalPath(int w, int h, int r)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, r, r, 180, 90); path.AddArc(w - r, 0, r, r, 270, 90);
            path.AddArc(w - r, h - r, r, r, 0, 90); path.AddArc(0, h - r, r, r, 90, 90);
            path.CloseFigure(); return path;
        }
    }
}
