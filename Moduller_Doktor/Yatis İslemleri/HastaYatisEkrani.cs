using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Doktor
{
    public partial class HastaYatisEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color BasariYesili = ColorTranslator.FromHtml("#05CD99");

        Panel pnlForm, pnlListe;
        ComboBox cmbHasta, cmbOda;
        Button btnKaydet;
        DataGridView dgvYatanlar;

        public HastaYatisEkrani()
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
            if (pnlForm == null || pnlListe == null) return;

            int margin = 30;
            int spacing = 20;
            int w = this.ClientSize.Width - (margin * 2);
            int h = this.ClientSize.Height - 80 - margin;

            int solGenislik = 400;
            int sagGenislik = w - solGenislik - spacing;

            pnlForm.Bounds = new Rectangle(margin, 80, solGenislik, h);
            pnlListe.Bounds = new Rectangle(margin + solGenislik + spacing, 80, sagGenislik, h);

            if (dgvYatanlar != null) dgvYatanlar.Bounds = new Rectangle(20, 60, sagGenislik - 40, h - 80);
        }

        private void EkraniInsaEt()
        {
            Label lblTitle = new Label { Text = "Yatış İşlemleri", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, 30) };
            this.Controls.Add(lblTitle);

            // SOL PANEL (Form)
            pnlForm = OlusturPanel();
            pnlForm.Controls.Add(new Label { Text = "1. Hasta Seçimi", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(20, 30), AutoSize = true });
            cmbHasta = new ComboBox { Location = new Point(20, 55), Width = 350, Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            pnlForm.Controls.Add(cmbHasta);

            pnlForm.Controls.Add(new Label { Text = "2. Boş Oda Seçimi", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(20, 115), AutoSize = true });
            cmbOda = new ComboBox { Location = new Point(20, 140), Width = 350, Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            pnlForm.Controls.Add(cmbOda);

            btnKaydet = ButonOlustur("🛏 Hastaya Yatış Ver", BasariYesili, new Point(20, 205), 350);
            btnKaydet.Click += BtnKaydet_Click;
            pnlForm.Controls.Add(btnKaydet);
            this.Controls.Add(pnlForm);

            // SAĞ PANEL (Liste)
            pnlListe = OlusturPanel();
            pnlListe.Controls.Add(new Label { Text = "Hastanede Yatan Aktif Hastalar", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true });

            dgvYatanlar = new DataGridView
            {
                BackgroundColor = SafBeyaz,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                DefaultCellStyle = new DataGridViewCellStyle { SelectionBackColor = ColorTranslator.FromHtml("#E0F2FE"), SelectionForeColor = TextDark, BackColor = SafBeyaz, ForeColor = TextDark, Font = new Font("Segoe UI", 11) },
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = SafBeyaz, ForeColor = TextMuted, Font = new Font("Segoe UI", 11, FontStyle.Bold) },
                ColumnHeadersHeight = 35,
                RowHeadersVisible = false,
                RowTemplate = { Height = 40 },
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            pnlListe.Controls.Add(dgvYatanlar);
            this.Controls.Add(pnlListe);
        }

        private void VerileriYukle()
        {
            try
            {
                SqlHelper db = new SqlHelper();

                // --- HASTALAR İÇİN ---
                Dictionary<string, object> parametreler = new Dictionary<string, object>();
                parametreler.Add("@kullanici_id", Program.AktifKullaniciID);

                DataTable dtHastalar = db.GetTable("sp_DoktorunHastalariniGetir", parametreler);
                cmbHasta.SetDataSourceWithChooseOption(dtHastalar, "HastaAdi", "hasta_id");

                // --- ODALAR VE YATANLAR İÇİN (Parametre boş gidiyor) ---
                Dictionary<string, object> bosParametre = new Dictionary<string, object>();

                DataTable dtOdalar = db.GetTable("sp_BosOdalariGetir", bosParametre);
                cmbOda.SetDataSourceWithChooseOption(dtOdalar, "OdaAdi", "oda_id");

                DataTable dtYatanlar = db.GetTable("sp_AktifYatislariGetir", bosParametre);
                dgvYatanlar.DataSource = dtYatanlar;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenemedi: " + ex.Message);
            }
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (cmbHasta.SelectedValue == null || cmbOda.SelectedValue == null)
            {
                MessageBox.Show("Lütfen hasta ve müsait bir oda seçin!", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlHelper db = new SqlHelper();
                db.GetTable("sp_HastaYatisVer", new Dictionary<string, object> {
                    { "@hasta_id", cmbHasta.SelectedValue },
                    { "@oda_id", cmbOda.SelectedValue }
                });

                MessageBox.Show("Hasta yatışı başarıyla onaylandı ve oda meşgul durumuna alındı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                VerileriYukle(); // Tabloları ve combobox'ları anlık günceller
            }
            catch (Exception ex) { MessageBox.Show("Yatış sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private Panel OlusturPanel()
        {
            Panel p = new Panel { BackColor = SafBeyaz };
            p.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalPath(p.Width, p.Height, 20))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                { p.Region = new Region(path); e.Graphics.DrawPath(pen, path); }
            };
            return p;
        }

        private Button ButonOlustur(string text, Color bg, Point loc, int w)
        {
            Button b = new Button { Text = text, BackColor = bg, ForeColor = SafBeyaz, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = loc, Size = new Size(w, 45), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0;
            b.Paint += (s, e) => {
                using (GraphicsPath path = OvalPath(b.Width, b.Height, 10)) { b.Region = new Region(path); }
            };
            return b;
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