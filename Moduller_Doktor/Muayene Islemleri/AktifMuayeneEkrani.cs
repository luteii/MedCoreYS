using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Doktor
{
    public partial class AktifMuayeneEkrani : UserControl
    {
        // Renk Paleti
        Color AnaZemin = ColorTranslator.FromHtml("#F0F4F8");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color MedikalMavi = ColorTranslator.FromHtml("#0284C7");
        Color AcikMavi = ColorTranslator.FromHtml("#38BDF8");

        // UI Elemanları
        DataGridView dgvAktif;
        Label lblKartBaslik;
        Label lblTitle;
        TextBox txtSikayet, txtTeshis, txtRecete;
        Panel pnlIcerikKapsayici;
        Panel pnlCard;
        Panel pnlGridKapsayici;

        int seciliRandevuID = 0;

        public AktifMuayeneEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;

            // Pencere boyutu değiştiğinde kusursuz hizalamayı koruması için
            this.Resize += (s, e) => { YerlesimiHizala(); };

            EkraniKur();
            VerileriYukle();
        }

        private void YerlesimiHizala()
        {
            if (pnlIcerikKapsayici == null || lblTitle == null)
                return;

            // Tablo + Sağ Kart grubunu ekranın tam ortasına yerleştir
            int x = (this.ClientSize.Width - pnlIcerikKapsayici.Width) / 2;
            int y = (this.ClientSize.Height - pnlIcerikKapsayici.Height) / 2;

            // Üst boşluk biraz daha dengeli olsun
            if (y < 80)
                y = 80;

            pnlIcerikKapsayici.Location = new Point(x, y);

            // Başlık tablonun solundan başlasın
            lblTitle.Location = new Point(x, y - 50);
        }

        private void EkraniKur()
        {
            // 1. Ana Başlık
            lblTitle = new Label
            {
                Text = "Aktif Muayeneler (Bekleyen Hastalar)",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // 2. Kapsayıcı Panel (Tablo + Sağ Kart Yan Yana)
            pnlIcerikKapsayici = new Panel
            {
                Size = new Size(1080, 620),
                BackColor = Color.Transparent
            };
            this.Controls.Add(pnlIcerikKapsayici);

            // 3. Sol Taraf - Köşe Eğimi Verilmiş Tablo Kapsayıcısı
            pnlGridKapsayici = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(560, 620),
                BackColor = SafBeyaz
            };

            pnlGridKapsayici.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = TamKoseOval(pnlGridKapsayici.Width, pnlGridKapsayici.Height, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1))
                {
                    pnlGridKapsayici.Region = new Region(path);
                    e.Graphics.DrawPath(pen, path);
                }
            };
            pnlIcerikKapsayici.Controls.Add(pnlGridKapsayici);

            dgvAktif = ModernDataGridOlustur(new Point(1, 1), new Size(558, 618));
            pnlGridKapsayici.Controls.Add(dgvAktif);

            dgvAktif.CellClick += (s, e) => {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvAktif.Rows[e.RowIndex];
                    seciliRandevuID = Convert.ToInt32(row.Cells[0].Value);
                    string hastaAd = row.Cells[2].Value.ToString();

                    lblKartBaslik.Text = $"Muayene: {hastaAd}";

                    txtSikayet.Text = "";
                    txtTeshis.Text = "";
                    txtRecete.Text = "";
                }
            };

            // 4. Sağ Taraf - Muayene Detay Kartı
            pnlCard = new Panel
            {
                Location = new Point(580, 0),
                Size = new Size(500, 620),
                BackColor = SafBeyaz
            };

            pnlCard.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = TamKoseOval(pnlCard.Width, pnlCard.Height, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1))
                {
                    pnlCard.Region = new Region(path);
                    e.Graphics.DrawPath(pen, path);
                }
            };
            pnlIcerikKapsayici.Controls.Add(pnlCard);

            lblKartBaslik = new Label
            {
                Text = "Lütfen Listeden Hasta Seçin",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = MedikalMavi,
                Location = new Point(25, 20),
                AutoSize = true
            };
            pnlCard.Controls.Add(lblKartBaslik);

            // Veri Giriş Alanları 
            txtSikayet = OvalTextBoxOlustur(pnlCard, "HASTANIN ŞİKAYETİ", 70, false);
            txtTeshis = OvalTextBoxOlustur(pnlCard, "ÖN TEŞHİS", 150, false);
            txtRecete = OvalTextBoxOlustur(pnlCard, "REÇETE / NOTLAR", 230, true);

            // Kaydet Butonu
            Button btnKaydet = new Button
            {
                Text = "Muayeneyi Tamamla",
                Location = new Point(25, 545),
                Size = new Size(450, 55),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                ForeColor = SafBeyaz,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            btnKaydet.FlatAppearance.BorderSize = 0;
            btnKaydet.Paint += BtnGradient_Oval_Paint;
            btnKaydet.MouseEnter += (s, e) => { btnKaydet.Invalidate(); };
            btnKaydet.MouseLeave += (s, e) => { btnKaydet.Invalidate(); };

            btnKaydet.Click += (s, e) => {
                if (seciliRandevuID == 0)
                {
                    MessageBox.Show("Lütfen önce sol taraftaki listeden bir hasta seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    Dictionary<string, object> prm = new Dictionary<string, object>()
                    {
                        { "@randevu_id", seciliRandevuID },
                        { "@sikayet", txtSikayet.Text },
                        { "@teshis", txtTeshis.Text },
                        { "@notlar", txtRecete.Text }
                    };

                    SqlHelper db = new SqlHelper();
                    db.ExecuteNonQuery("sp_MuayeneKaydet", prm);

                    MessageBox.Show("Muayene başarıyla kaydedildi ve randevu tamamlandı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    seciliRandevuID = 0;
                    lblKartBaslik.Text = "Lütfen Listeden Hasta Seçin";
                    txtSikayet.Text = "";
                    txtTeshis.Text = "";
                    txtRecete.Text = "";

                    VerileriYukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kayıt sırasında bir hata oluştu: " + ex.Message, "Sistem Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            pnlCard.Controls.Add(btnKaydet);

            // İlk yerleşimi tetikle
            YerlesimiHizala();
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();
                var prm = new Dictionary<string, object>
                {
                    { "@kullanici_id", Program.AktifKullaniciID }
                };
                DataTable dt = db.GetTable("sp_AktifMuayeneleriGetir", prm);
                dgvAktif.DataSource = dt;

                if (dgvAktif.Columns.Count > 0)
                {
                    // Gizli ID
                    dgvAktif.Columns[1].Visible = false;

                    // Kayıt No
                    dgvAktif.Columns[0].Width = 80;

                    // Hasta Adı Soyadı
                    dgvAktif.Columns[2].Width = 230;

                    // Randevu Tarihi / Saati
                    dgvAktif.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    // Hücre hizaları
                    dgvAktif.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgvAktif.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Başlık hizaları
                    foreach (DataGridViewColumn col in dgvAktif.Columns)
                    {
                        col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }

                    // Yazılar ortalansın
                    dgvAktif.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                    // Satırların otomatik boyutlanmasını engelle
                    dgvAktif.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Randevular yüklenirken bir hata oluştu: " + ex.Message, "Veritabanı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TextBox OvalTextBoxOlustur(Panel parent, string etiket, int yPos, bool cokSatirli)
        {
            Label lbl = new Label { Text = etiket, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = TextDark, Location = new Point(25, yPos), AutoSize = true };
            parent.Controls.Add(lbl);

            int yukseklik = cokSatirli ? 110 : 40;
            Panel pnlTxt = new Panel
            {
                Location = new Point(25, yPos + 20),
                Size = new Size(450, yukseklik),
                BackColor = SafBeyaz
            };
            pnlTxt.Paint += OvalPanel_Border_Paint;
            parent.Controls.Add(pnlTxt);

            TextBox txt = new TextBox
            {
                Location = new Point(15, 10),
                Size = new Size(420, yukseklik - 20),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.None,
                BackColor = SafBeyaz,
                ForeColor = TextDark,
                Multiline = cokSatirli
            };
            pnlTxt.Controls.Add(txt);
            return txt;
        }

        private DataGridView ModernDataGridOlustur(Point konum, Size boyut)
        {
            DataGridView dgv = new DataGridView();
            dgv.Location = konum;
            dgv.Size = boyut;
            dgv.BackgroundColor = SafBeyaz;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#E0F2FE");
            dgv.DefaultCellStyle.SelectionForeColor = TextDark;
            dgv.DefaultCellStyle.BackColor = SafBeyaz;
            dgv.DefaultCellStyle.ForeColor = TextDark;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = MedikalMavi;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = SafBeyaz;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 50;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 48;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            return dgv;
        }

        private void OvalPanel_Border_Paint(object sender, PaintEventArgs e)
        {
            Control pnl = (Control)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = TamKoseOval(pnl.Width, pnl.Height, 10))
            using (Pen pen = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1))
            {
                pnl.Region = new Region(path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void BtnGradient_Oval_Paint(object sender, PaintEventArgs e)
        {
            Button btn = (Button)sender;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bool isHovered = btn.ClientRectangle.Contains(btn.PointToClient(Cursor.Position));
            Color startColor = isHovered ? AcikMavi : MedikalMavi;
            Color endColor = isHovered ? MedikalMavi : AcikMavi;

            Rectangle rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
            using (GraphicsPath path = TamKoseOval(btn.Width, btn.Height, 15))
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Horizontal))
            {
                btn.Region = new Region(path);
                e.Graphics.FillPath(brush, path);
            }
            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, rect, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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