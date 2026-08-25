using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class OnlineOdemeEkrani : UserControl
    {
        Color AnaZemin   = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz   = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark   = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted  = ColorTranslator.FromHtml("#64748B");
        Color OdemeYes   = ColorTranslator.FromHtml("#059669");
        Color OdemeAcik  = ColorTranslator.FromHtml("#34D399");

        Panel pnlKart; Label lblTitle;
        TextBox txtKartNo, txtKartSahibi, txtSKT, txtCVV, txtTutar;

        public OnlineOdemeEkrani()
        {
            this.BackColor = AnaZemin; this.Dock = DockStyle.Fill; this.DoubleBuffered = true;
            EkraniKur();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlKart == null) return;
            int w = Math.Min(720, this.ClientSize.Width - 80);
            int h = 580;
            int x = (this.ClientSize.Width - w) / 2;
            int y = (this.ClientSize.Height - h) / 2;
            if (y < 80) y = 80;
            pnlKart.Bounds = new Rectangle(x, y, w, h);
            if (lblTitle != null) lblTitle.Location = new Point(x, y - 50);
        }

        private void EkraniKur()
        {
            lblTitle = new Label { Text = "Online Ödeme", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true };
            this.Controls.Add(lblTitle);

            pnlKart = new Panel { BackColor = SafBeyaz };
            pnlKart.Resize += (s, e) => OvalKirp(pnlKart, 20);
            pnlKart.Paint  += (s, e) => InceCerceveCiz(pnlKart, e.Graphics, 20);
            this.Controls.Add(pnlKart);

            // Kart Önizleme
            Panel pnlKartOn = new Panel { Location = new Point(40, 30), Size = new Size(640, 160), BackColor = Color.Transparent };
            pnlKartOn.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = TamKoseOval(pnlKartOn.Width, pnlKartOn.Height, 18))
                using (LinearGradientBrush br = new LinearGradientBrush(
                    new Rectangle(0, 0, pnlKartOn.Width, pnlKartOn.Height),
                    ColorTranslator.FromHtml("#1E3A8A"), ColorTranslator.FromHtml("#3B82F6"),
                    LinearGradientMode.ForwardDiagonal))
                {
                    pnlKartOn.Region = new Region(path);
                    e.Graphics.FillPath(br, path);
                }
                // Kart numarası
                string kartGoster = string.IsNullOrWhiteSpace(txtKartNo?.Text) ? "•••• •••• •••• ••••" : FormatKartNo(txtKartNo.Text);
                TextRenderer.DrawText(e.Graphics, kartGoster, new Font("Courier New", 18, FontStyle.Bold), new Rectangle(25, 70, 590, 40), Color.White, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                // Kart sahibi
                string sahip = string.IsNullOrWhiteSpace(txtKartSahibi?.Text) ? "AD SOYAD" : txtKartSahibi.Text.ToUpper();
                TextRenderer.DrawText(e.Graphics, sahip, new Font("Segoe UI", 11), new Rectangle(25, 120, 400, 25), Color.FromArgb(200, 255, 255, 255), TextFormatFlags.Left);
                // SKT
                string skt = string.IsNullOrWhiteSpace(txtSKT?.Text) ? "MM/YY" : txtSKT.Text;
                TextRenderer.DrawText(e.Graphics, "SKT: " + skt, new Font("Segoe UI", 11), new Rectangle(480, 120, 150, 25), Color.FromArgb(200, 255, 255, 255), TextFormatFlags.Left);
                // Chip ikonu
                TextRenderer.DrawText(e.Graphics, "▣", new Font("Segoe UI", 22), new Rectangle(25, 25, 50, 40), ColorTranslator.FromHtml("#FBBF24"), TextFormatFlags.Left);
            };
            pnlKart.Controls.Add(pnlKartOn);

            // Form Alanları
            EtiketEkle(pnlKart, "Kart Numarası", 210);
            txtKartNo = FormAlaniOlustur(pnlKart, 235, false, false, pnlKartOn);
            txtKartNo.MaxLength = 19;
            txtKartNo.TextChanged += (s, e) => { txtKartNo.Text = FormatKartNo(txtKartNo.Text); txtKartNo.SelectionStart = txtKartNo.Text.Length; pnlKartOn.Invalidate(); };

            EtiketEkle(pnlKart, "Kart Üzerindeki Ad Soyad", 285);
            txtKartSahibi = FormAlaniOlustur(pnlKart, 310, false, false, pnlKartOn);
            txtKartSahibi.TextChanged += (s, e) => pnlKartOn.Invalidate();

            EtiketEkle(pnlKart, "Son Kullanma Tarihi (AA/YY)", 360);
            txtSKT = FormAlaniOlustur(pnlKart, 385, false, false, pnlKartOn, 200);
            txtSKT.MaxLength = 5;

            EtiketEkle(pnlKart, "CVV", 435);
            txtCVV = FormAlaniOlustur(pnlKart, 460, false, true, pnlKartOn, 200);
            txtCVV.MaxLength = 3;

            EtiketEkle(pnlKart, "Ödenecek Tutar (₺)", 360, 340);
            txtTutar = FormAlaniOlustur(pnlKart, 385, false, false, pnlKartOn, 200, 340);

            // Ödeme Butonu
            Button btnOde = new Button { Text = "🔐  Güvenli Ödeme Yap", Location = new Point(40, 520), Size = new Size(640, 50), Font = new Font("Segoe UI", 13, FontStyle.Bold), FlatStyle = FlatStyle.Flat, ForeColor = SafBeyaz, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btnOde.FlatAppearance.BorderSize = 0;
            btnOde.Paint += (s, e) =>
            {
                Button b = (Button)s; bool hov = b.ClientRectangle.Contains(b.PointToClient(System.Windows.Forms.Cursor.Position));
                using (GraphicsPath path = TamKoseOval(b.Width, b.Height, 12))
                using (LinearGradientBrush br = new LinearGradientBrush(new Rectangle(0, 0, b.Width, b.Height), hov ? OdemeAcik : OdemeYes, hov ? OdemeYes : OdemeAcik, LinearGradientMode.Horizontal))
                { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; b.Region = new Region(path); e.Graphics.FillPath(br, path); }
                TextRenderer.DrawText(e.Graphics, b.Text, b.Font, new Rectangle(0, 0, b.Width, b.Height), b.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            btnOde.MouseEnter += (s, e) => btnOde.Invalidate();
            btnOde.MouseLeave += (s, e) => btnOde.Invalidate();
            btnOde.Click += BtnOde_Click;
            pnlKart.Controls.Add(btnOde);

            OnResize(EventArgs.Empty);
        }

        private string FormatKartNo(string raw)
        {
            raw = raw.Replace(" ", "");
            if (raw.Length > 16) raw = raw.Substring(0, 16);
            string formatted = "";
            for (int i = 0; i < raw.Length; i++) { if (i > 0 && i % 4 == 0) formatted += " "; formatted += raw[i]; }
            return formatted;
        }

        private void BtnOde_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKartNo.Text) || string.IsNullOrWhiteSpace(txtKartSahibi.Text))
            {
                MessageBox.Show("Lütfen tüm kart bilgilerini doldurun!", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("✅ Ödemeniz başarıyla gerçekleştirildi!\n(Demo modunda veritabanı işlemi simüle edilmektedir.)", "Ödeme Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EtiketEkle(Panel parent, string metin, int y, int x = 40)
        {
            parent.Controls.Add(new Label { Text = metin, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(x, y), AutoSize = true });
        }

        private TextBox FormAlaniOlustur(Panel parent, int y, bool cokSatir, bool sifre, Panel kartPnl, int genislik = 640, int x = 40)
        {
            Panel pnlTxt = new Panel { Location = new Point(x, y), Size = new Size(genislik, 38), BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            pnlTxt.Paint += (s, e) => { using (GraphicsPath p2 = TamKoseOval(pnlTxt.Width, pnlTxt.Height, 10)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1)) { pnlTxt.Region = new Region(p2); e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; e.Graphics.DrawPath(pen, p2); } };
            parent.Controls.Add(pnlTxt);
            TextBox txt = new TextBox { Location = new Point(12, 8), Size = new Size(genislik - 24, 22), Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.None, BackColor = ColorTranslator.FromHtml("#F8FAFC"), ForeColor = TextDark, Multiline = cokSatir, PasswordChar = sifre ? '●' : '\0' };
            pnlTxt.Controls.Add(txt);
            return txt;
        }

        private void OvalKirp(Panel pnl, int r) { if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); } }
        private void InceCerceveCiz(Panel pnl, Graphics g, int r) { g.SmoothingMode = SmoothingMode.AntiAlias; using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, r)) using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1)) g.DrawPath(pen, p); }
        private GraphicsPath TamKoseOval(int w, int h, int radius) { radius = Math.Min(radius, Math.Min(w / 2, h / 2)); if (radius <= 0) radius = 1; GraphicsPath path = new GraphicsPath(); float c = radius * 2F; path.StartFigure(); path.AddArc(0, 0, c, c, 180, 90); path.AddArc(w - c, 0, c, c, 270, 90); path.AddArc(w - c, h - c, c, c, 0, 90); path.AddArc(0, h - c, c, c, 90, 90); path.CloseFigure(); return path; }
    }
}
