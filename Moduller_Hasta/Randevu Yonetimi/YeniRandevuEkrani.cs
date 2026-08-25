using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Hasta
{
    public partial class YeniRandevuEkrani : UserControl
    {
        Color AnaZemin  = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz  = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark  = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color HastaMavi = ColorTranslator.FromHtml("#3B82F6");
        Color AcikMavi  = ColorTranslator.FromHtml("#38BDF8");

        Panel       pnlKart;
        Label       lblTitle;
        ComboBox    cmbBolum, cmbDoktor;
        DateTimePicker dtpTarih;
        ComboBox    cmbSaat;
        TextBox     txtNot;

        public YeniRandevuEkrani()
        {
            this.BackColor    = AnaZemin;
            this.Dock         = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
            this.Load += (s, e) => BolumleriYukle();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlKart == null) return;
            int w = Math.Min(700, this.ClientSize.Width - 80);
            int h = 580;
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
                Text      = "Yeni Randevu Al",
                Font      = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize  = true
            };
            this.Controls.Add(lblTitle);

            pnlKart = new Panel { BackColor = SafBeyaz };
            pnlKart.Resize += (s, e) => OvalKirp(pnlKart, 20);
            pnlKart.Paint  += (s, e) => InceCerceveCiz(pnlKart, e.Graphics, 20);
            this.Controls.Add(pnlKart);

            // Başlık
            Panel pnlUstBar = new Panel { Location = new Point(0, 0), Size = new Size(700, 65), BackColor = Color.Transparent };
            pnlUstBar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    int r = 20;
                    path.AddArc(0, 0, r * 2, r * 2, 180, 90);
                    path.AddArc(pnlUstBar.Width - r * 2, 0, r * 2, r * 2, 270, 90);
                    path.AddLine(pnlUstBar.Width, pnlUstBar.Height, 0, pnlUstBar.Height);
                    path.CloseFigure();
                    using (LinearGradientBrush br = new LinearGradientBrush(
                        new Rectangle(0, 0, pnlUstBar.Width, pnlUstBar.Height),
                        HastaMavi, AcikMavi, LinearGradientMode.Horizontal))
                        e.Graphics.FillPath(br, path);
                }
                TextRenderer.DrawText(e.Graphics, "📅  Randevu Formu",
                    new Font("Segoe UI", 14, FontStyle.Bold),
                    new Rectangle(0, 0, pnlUstBar.Width, pnlUstBar.Height),
                    Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            pnlKart.Controls.Add(pnlUstBar);

            // Bölüm Seçimi
            EtiketEkle(pnlKart, "🏥  Bölüm Seçin", 85);
            cmbBolum = ComboBoxOlustur(pnlKart, 110);
            cmbBolum.SelectedIndexChanged += CmbBolum_SelectedIndexChanged;

            // Doktor Seçimi
            EtiketEkle(pnlKart, "👨‍⚕️  Doktor Seçin", 175);
            cmbDoktor = ComboBoxOlustur(pnlKart, 200);

            // Tarih
            EtiketEkle(pnlKart, "📅  Randevu Tarihi", 265);
            dtpTarih = new DateTimePicker
            {
                Location       = new Point(40, 290),
                Size           = new Size(620, 38),
                Font           = new Font("Segoe UI", 11),
                MinDate        = DateTime.Today.AddDays(1),
                MaxDate        = DateTime.Today.AddMonths(3),
                Format         = DateTimePickerFormat.Long
            };
            pnlKart.Controls.Add(dtpTarih);

            // Saat
            EtiketEkle(pnlKart, "🕐  Saat", 340);
            cmbSaat = new ComboBox { Location = new Point(40, 365), Size = new Size(300, 38), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            cmbSaat.Items.Add("Seçiniz");
            for (int h2 = 8; h2 <= 17; h2++) { cmbSaat.Items.Add($"{h2:00}:00"); cmbSaat.Items.Add($"{h2:00}:30"); }
            cmbSaat.SelectedIndex = 0;
            pnlKart.Controls.Add(cmbSaat);

            // Not
            EtiketEkle(pnlKart, "📝  Notunuz (İsteğe Bağlı)", 415);
            txtNot = new TextBox
            {
                Location    = new Point(40, 440),
                Size        = new Size(620, 60),
                Font        = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor   = ColorTranslator.FromHtml("#F8FAFC"),
                Multiline   = true
            };
            pnlKart.Controls.Add(txtNot);

            // Buton
            Button btn = OvalButonOlustur("✅  Randevu Al", new Point(40, 520), 620);
            btn.Click += BtnRandevuAl_Click;
            pnlKart.Controls.Add(btn);

            OnResize(EventArgs.Empty);
        }

        private void BolumleriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                DataTable dt = db.GetTable("sp_BolumleriGetir", null);
                
                foreach (DataRow row in dt.Rows)
                {
                    if (row["BolumAdi"] != DBNull.Value)
                    {
                        string[] words = row["BolumAdi"].ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < words.Length; i++)
                        {
                            if (words[i].Length > 0)
                            {
                                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                            }
                        }
                        row["BolumAdi"] = string.Join(" ", words);
                    }
                }

                cmbBolum.SelectedIndexChanged -= CmbBolum_SelectedIndexChanged;
                cmbBolum.SetDataSourceWithChooseOption(dt, "BolumAdi", "bolum_id");
                cmbBolum.SelectedIndexChanged += CmbBolum_SelectedIndexChanged;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void CmbBolum_SelectedIndexChanged(object sender, EventArgs e)
        {
            DoktorlariYukle();
        }

        public class ComboItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public override string ToString() => Text;
        }

        private void DoktorlariYukle()
        {
            cmbDoktor.DataSource = null;
            cmbDoktor.Items.Clear();

            if (cmbBolum.SelectedValue == null || !(cmbBolum.SelectedValue is int) || (int)cmbBolum.SelectedValue <= 0)
            {
                cmbDoktor.Items.Add(new ComboItem { Text = "Seçiniz", Value = -1 });
                cmbDoktor.SelectedIndex = 0;
                return;
            }
            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object> { { "@bolum_id", cmbBolum.SelectedValue } };
                DataTable dt = db.GetTable("sp_BolumDoktorlariGetir", prm);
                
                cmbDoktor.Items.Add(new ComboItem { Text = "Seçiniz", Value = -1 });

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = "";
                        if (dt.Columns.Contains("DoktorAdi") && row["DoktorAdi"] != DBNull.Value)
                            name = row["DoktorAdi"].ToString();
                        else if (dt.Columns.Contains("ad_soyad") && row["ad_soyad"] != DBNull.Value)
                            name = row["ad_soyad"].ToString();
                        else if (dt.Columns.Count > 1)
                            name = row[1].ToString();
                        
                        int id = -1;
                        if (dt.Columns.Contains("kullanici_id") && row["kullanici_id"] != DBNull.Value)
                            id = Convert.ToInt32(row["kullanici_id"]);
                        else if (dt.Columns.Count > 0)
                            id = Convert.ToInt32(row[0]);

                        cmbDoktor.Items.Add(new ComboItem { Text = name, Value = id });
                    }
                }
                
                cmbDoktor.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnRandevuAl_Click(object sender, EventArgs e)
        {
            int doktorId = -1;
            if (cmbDoktor.SelectedItem is ComboItem item)
            {
                doktorId = item.Value;
            }
            else if (cmbDoktor.SelectedValue != null && cmbDoktor.SelectedValue is int val)
            {
                doktorId = val;
            }

            if (doktorId <= 0 || cmbSaat.SelectedItem == null || cmbSaat.SelectedItem.ToString() == "Seçiniz")
            {
                MessageBox.Show("Lütfen doktor ve saat seçin!", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                string saatStr = cmbSaat.SelectedItem.ToString();
                DateTime randevuTarihi = dtpTarih.Value.Date.Add(TimeSpan.Parse(saatStr));
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prm = new Dictionary<string, object>
                {
                    { "@hasta_id",        Program.AktifKullaniciID },
                    { "@doktor_id",       doktorId },
                    { "@randevu_tarihi",  randevuTarihi },
                    { "@notlar",          txtNot.Text }
                };
                db.ExecuteNonQuery("sp_RandevuEkle", prm);
                MessageBox.Show("Randevunuz başarıyla alındı! 🎉", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNot.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Randevu alınamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EtiketEkle(Panel parent, string metin, int y)
        {
            parent.Controls.Add(new Label { Text = metin, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(40, y), AutoSize = true });
        }

        private ComboBox ComboBoxOlustur(Panel parent, int y)
        {
            ComboBox cmb = new ComboBox { Location = new Point(40, y), Size = new Size(620, 38), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            parent.Controls.Add(cmb);
            return cmb;
        }

        private Button OvalButonOlustur(string metin, Point konum, int genislik)
        {
            Button btn = new Button { Text = metin, Location = konum, Size = new Size(genislik, 48), Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, ForeColor = SafBeyaz, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            btn.Paint += (s, e) =>
            {
                Button b = (Button)s; bool hov = b.ClientRectangle.Contains(b.PointToClient(System.Windows.Forms.Cursor.Position));
                Rectangle r = new Rectangle(0, 0, b.Width - 1, b.Height - 1);
                using (GraphicsPath path = TamKoseOval(b.Width, b.Height, 12))
                using (LinearGradientBrush br = new LinearGradientBrush(r, hov ? AcikMavi : HastaMavi, hov ? HastaMavi : AcikMavi, LinearGradientMode.Horizontal))
                { e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; b.Region = new Region(path); e.Graphics.FillPath(br, path); }
                TextRenderer.DrawText(e.Graphics, b.Text, b.Font, r, b.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            btn.MouseEnter += (s, e) => btn.Invalidate();
            btn.MouseLeave += (s, e) => btn.Invalidate();
            return btn;
        }

        private void OvalKirp(Panel pnl, int radius)
        {
            if (pnl.Width > 0 && pnl.Height > 0) { using (GraphicsPath p = TamKoseOval(pnl.Width, pnl.Height, radius)) { pnl.Region?.Dispose(); pnl.Region = new Region(p); } pnl.Invalidate(); }
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
            GraphicsPath path = new GraphicsPath(); float c = radius * 2F;
            path.StartFigure();
            path.AddArc(0, 0, c, c, 180, 90); path.AddArc(width - c, 0, c, c, 270, 90);
            path.AddArc(width - c, height - c, c, c, 0, 90); path.AddArc(0, height - c, c, c, 90, 90);
            path.CloseFigure(); return path;
        }
    }
}
