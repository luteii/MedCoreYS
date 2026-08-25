using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class VeritabaniYedekleEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color KoyuMavi = ColorTranslator.FromHtml("#0284C7");
        Color Turuncu = ColorTranslator.FromHtml("#F59E0B");

        public VeritabaniYedekleEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
        }

        private void EkraniKur()
        {
            Label lblTitle = new Label
            {
                Text = "Sistem ve Veritabanı Yedekleme",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Hastane veritabanının tam bir yedeğini oluşturmak için butona tıklayınız.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(40, 80)
            };
            this.Controls.Add(lblSubtitle);

            Panel pnlKart = new Panel
            {
                Location = new Point(40, 110),
                Size = new Size(500, 250),
                BackColor = SafBeyaz
            };
            pnlKart.Paint += (s, e) => 
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalKose(pnlKart.Width - 1, pnlKart.Height - 1, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    e.Graphics.DrawPath(pen, path);
            };
            pnlKart.Resize += (s, e) => 
            {
                using (GraphicsPath path = OvalKose(pnlKart.Width, pnlKart.Height, 15))
                    pnlKart.Region = new Region(path);
            };
            this.Controls.Add(pnlKart);

            Label lblIkon = new Label { Text = "💾", Font = new Font("Segoe UI Emoji", 48), ForeColor = KoyuMavi, Location = new Point(200, 20), AutoSize = true };
            pnlKart.Controls.Add(lblIkon);

            Label lblUyari = new Label
            {
                Text = "Veritabanı boyutu büyükse yedekleme işlemi birkaç dakika sürebilir. Lütfen işlem tamamlanana kadar paneli kapatmayınız.",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Turuncu,
                AutoSize = true,
                Location = new Point(40, 125),
                MaximumSize = new Size(420, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlKart.Controls.Add(lblUyari);

            Button btnYedekle = new Button
            {
                Text = "⚙️ Yedeği Başlat",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(240, 45),
                Location = new Point(130, 185),
                BackColor = KoyuMavi,
                ForeColor = SafBeyaz,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnYedekle.FlatAppearance.BorderSize = 0;
            pnlKart.Controls.Add(btnYedekle);
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

