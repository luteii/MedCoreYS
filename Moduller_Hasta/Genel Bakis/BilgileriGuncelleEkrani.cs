using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class BilgileriGuncelleEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color HastaMavi = ColorTranslator.FromHtml("#3B82F6");
        Color AcikMavi  = ColorTranslator.FromHtml("#38BDF8");

        Panel  pnlKart;
        Label  lblTitle;
        TextBox txtTelefon, txtEmail, txtAdres, txtSifre, txtSifreTekrar;

        public BilgileriGuncelleEkrani()
        {
            this.BackColor    = AnaZemin;
            this.Dock         = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlKart == null) return;
            int w = Math.Min(700, this.ClientSize.Width - 80);
            int h = 600;
            int x = (this.ClientSize.Width - w) / 2;
            int y = (this.ClientSize.Height - h) / 2;
            if (y < 80) y = 80;
            pnlKart.Bounds = new Rectangle(x, y, w, h);
            if (lblTitle != null) lblTitle.Location = new Point(x, y - 50);
        }

        private void EkraniKur()
        {
            lblTitle = new Label
            {
                Text      = "Bilgilerimi Güncelle",
                Font      = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize  = true,
                Location  = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            pnlKart = new Panel { BackColor = SafBeyaz };
            pnlKart.Resize += (s, e) => OvalKirp(pnlKart, 20);
            pnlKart.Paint  += (s, e) => InceCerceveCiz(pnlKart, e.Graphics, 20);
            this.Controls.Add(pnlKart);

            // Kart başlık
            Label lblBaslik = new Label
            {
                Text      = "İletişim & Güvenlik Bilgileri",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = HastaMavi,
                Location  = new Point(40, 30),
                AutoSize  = true
            };
            pnlKart.Controls.Add(lblBaslik);

            Panel ayrac = new Panel { Location = new Point(40, 60), Size = new Size(620, 2), BackColor = ColorTranslator.FromHtml("#E2E8F0") };
            pnlKart.Controls.Add(ayrac);

            // Form Alanları
            txtTelefon    = FormAlaniOlustur(pnlKart, "📞  Telefon Numarası",  80, false);
            txtEmail      = FormAlaniOlustur(pnlKart, "✉   E-Posta Adresi",   155, false);
            txtAdres      = FormAlaniOlustur(pnlKart, "🏠  Adres",             230, true);
            txtSifre      = FormAlaniOlustur(pnlKart, "🔒  Yeni Şifre",        345, false, true);
            txtSifreTekrar= FormAlaniOlustur(pnlKart, "🔒  Şifre Tekrar",      420, false, true);

            // Kaydet Butonu
            Button btnKaydet = OvalButonOlustur("💾  Değişiklikleri Kaydet", new Point(40, 510), 620);
            btnKaydet.Click += BtnKaydet_Click;
            pnlKart.Controls.Add(btnKaydet);

            OnResize(EventArgs.Empty);
        }

        private TextBox FormAlaniOlustur(Panel parent, string etiket, int y, bool cokSatir, bool sifre = false)
        {
            Label lbl = new Label { Text = etiket, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(40, y), AutoSize = true };
            parent.Controls.Add(lbl);

            Panel pnlTxt = new Panel { Location = new Point(40, y + 22), Size = new Size(620, cokSatir ? 90 : 38), BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            pnlTxt.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = TamKoseOval(pnlTxt.Width, pnlTxt.Height, 10))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1))
                { pnlTxt.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };
            parent.Controls.Add(pnlTxt);

            TextBox txt = new TextBox
            {
                Location      = new Point(12, 8),
                Size          = new Size(596, cokSatir ? 74 : 22),
                Font          = new Font("Segoe UI", 11),
                BorderStyle   = BorderStyle.None,
                BackColor     = ColorTranslator.FromHtml("#F8FAFC"),
                ForeColor     = TextDark,
                Multiline     = cokSatir,
                PasswordChar  = sifre ? '●' : '\0'
            };
            pnlTxt.Controls.Add(txt);
            return txt;
        }

        private Button OvalButonOlustur(string metin, Point konum, int genislik)
        {
            Button btn = new Button
            {
                Text      = metin,
                Location  = konum,
                Size      = new Size(genislik, 48),
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                ForeColor = SafBeyaz,
                BackColor = Color.Transparent,
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Paint += (s, e) =>
            {
                Button b = (Button)s;
                bool hov = b.ClientRectangle.Contains(b.PointToClient(System.Windows.Forms.Cursor.Position));
                Rectangle r = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                using (GraphicsPath path = TamKoseOval(b.Width, b.Height, 12))
                using (LinearGradientBrush brush = new LinearGradientBrush(r,
                    hov ? AcikMavi : HastaMavi,
                    hov ? HastaMavi : AcikMavi,
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    b.Region = new Region(path);
                    e.Graphics.FillPath(brush, path);
                }
                TextRenderer.DrawText(e.Graphics, b.Text, b.Font, r, b.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            btn.MouseEnter += (s, e) => btn.Invalidate();
            btn.MouseLeave += (s, e) => btn.Invalidate();
            return btn;
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSifre.Text) && txtSifre.Text != txtSifreTekrar.Text)
            {
                MessageBox.Show("Şifreler eşleşmiyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object>
                {
                    { "@kullanici_id",  Program.AktifKullaniciID },
                    { "@telefon",       txtTelefon.Text },
                    { "@email",         txtEmail.Text },
                    { "@adres",         txtAdres.Text },
                    { "@yeni_sifre",    string.IsNullOrWhiteSpace(txtSifre.Text) ? (object)DBNull.Value : SecurityHelper.HashPassword(txtSifre.Text) }
                };
                db.ExecuteNonQuery("sp_HastaGuncelle", prm);
                MessageBox.Show("Bilgileriniz başarıyla güncellendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSifre.Text = "";
                txtSifreTekrar.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme sırasında hata: " + ex.Message, "Sistem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OvalKirp(Panel pnl, int radius)
        {
            if (pnl.Width > 0 && pnl.Height > 0)
            {
                using (GraphicsPath path = TamKoseOval(pnl.Width, pnl.Height, radius))
                { pnl.Region?.Dispose(); pnl.Region = new Region(path); }
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
