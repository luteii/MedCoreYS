using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Doktor.Recete
{
    public partial class YeniReceteEkrani : UserControl
    {
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color MedikalMavi = ColorTranslator.FromHtml("#4318FF");
        Color BasariYesili = ColorTranslator.FromHtml("#05CD99");

        Panel pnlForm, pnlSepet;
        ComboBox cmbHasta, cmbIlac;
        TextBox txtTani, txtKullanim;
        DataGridView dgvSepet;
        Button btnEkle, btnKaydet;
        DataTable dtSepet;

        public YeniReceteEkrani()
        {
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;

            dtSepet = new DataTable();
            dtSepet.Columns.Add("IlacID", typeof(int));
            dtSepet.Columns.Add("İlaç Adı", typeof(string));
            dtSepet.Columns.Add("Kullanım Şekli", typeof(string));

            EkraniInsaEt();
            this.Load += (s, e) => DropdownlariDoldur();
        }

        // TAŞMA SORUNUNU ÇÖZEN DİNAMİK BOYUTLANDIRMA
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (pnlForm == null || pnlSepet == null) return;

            int margin = 30;
            int spacing = 20;
            int w = this.ClientSize.Width - (margin * 2);
            int h = this.ClientSize.Height - 80 - margin;

            int solGenislik = 400; // Sol taraf sabit
            int sagGenislik = w - solGenislik - spacing; // Sağ taraf esnek

            pnlForm.Bounds = new Rectangle(margin, 80, solGenislik, h);
            pnlSepet.Bounds = new Rectangle(margin + solGenislik + spacing, 80, sagGenislik, h);

            // Sağ panelin (Sepet) içindeki grid ve butonu esnet
            if (dgvSepet != null) dgvSepet.Bounds = new Rectangle(20, 60, sagGenislik - 40, h - 135);
            if (btnKaydet != null) btnKaydet.Bounds = new Rectangle(20, h - 65, sagGenislik - 40, 45);
        }

        private void EkraniInsaEt()
        {
            Label lblTitle = new Label { Text = "Yeni Reçete Oluştur", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, 30) };
            this.Controls.Add(lblTitle);

            // SOL PANEL (Form)
            pnlForm = OlusturPanel();
            pnlForm.Controls.Add(new Label { Text = "1. Hasta Seçimi", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(20, 20), AutoSize = true });
            cmbHasta = new ComboBox { Location = new Point(20, 45), Width = 350, Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            pnlForm.Controls.Add(cmbHasta);

            pnlForm.Controls.Add(new Label { Text = "2. Teşhis / Tanı", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(20, 95), AutoSize = true });
            txtTani = new TextBox { Location = new Point(20, 120), Width = 350, Font = new Font("Segoe UI", 11), Multiline = true, Height = 70, BorderStyle = BorderStyle.FixedSingle };
            pnlForm.Controls.Add(txtTani);

            pnlForm.Controls.Add(new Label { Text = "3. İlaç Ekle", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(20, 210), AutoSize = true });
            cmbIlac = new ComboBox { Location = new Point(20, 235), Width = 350, Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#F8FAFC") };
            pnlForm.Controls.Add(cmbIlac);

            pnlForm.Controls.Add(new Label { Text = "Kullanım Şekli (Örn: Günde 2 Tok)", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = TextMuted, Location = new Point(20, 285), AutoSize = true });
            txtKullanim = new TextBox { Location = new Point(20, 310), Width = 350, Font = new Font("Segoe UI", 12), BorderStyle = BorderStyle.FixedSingle };
            pnlForm.Controls.Add(txtKullanim);

            btnEkle = ButonOlustur("Reçeteye Ekle ➔", MedikalMavi, new Point(20, 365), 350);
            btnEkle.Click += BtnEkle_Click;
            pnlForm.Controls.Add(btnEkle);
            this.Controls.Add(pnlForm);

            // SAĞ PANEL (Sepet ve Kayıt)
            pnlSepet = OlusturPanel();
            pnlSepet.Controls.Add(new Label { Text = "Reçete Listesi", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = TextDark, Location = new Point(20, 20), AutoSize = true });

            dgvSepet = new DataGridView
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = dtSepet
            };
            pnlSepet.Controls.Add(dgvSepet);

            btnKaydet = ButonOlustur("💾 Reçeteyi Onayla ve Sisteme Kaydet", BasariYesili, new Point(20, 100), 100);
            btnKaydet.Click += BtnKaydet_Click;
            pnlSepet.Controls.Add(btnKaydet);
            this.Controls.Add(pnlSepet);
        }

        private void DropdownlariDoldur()
        {
            try
            {
                SqlHelper db = new SqlHelper();

                // 1. Hasta Listesini Çek ve Kutuya Bağla
                DataTable dtHastalar = db.GetTable("sp_DoktorunHastalariniGetir", new Dictionary<string, object> { { "@kullanici_id", Program.AktifKullaniciID } });
                cmbHasta.SetDataSourceWithChooseOption(dtHastalar, "HastaAdi", "hasta_id");

                // 2. İlaç Listesini Çek ve Kutuya Bağla
                DataTable dtIlaclar = db.GetTable("sp_IlaclariGetir", new Dictionary<string, object>());
                cmbIlac.SetDataSourceWithChooseOption(dtIlaclar, "İlaç Adı", "ilac_ID");

                if (dgvSepet.Columns.Count > 0) dgvSepet.Columns["IlacID"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Veriler yüklenemedi: " + ex.Message); }
        }
        private void BtnEkle_Click(object sender, EventArgs e)
        {
            if (cmbIlac.SelectedValue == null || string.IsNullOrWhiteSpace(txtKullanim.Text))
            {
                MessageBox.Show("Lütfen bir ilaç seçin ve kullanım şeklini yazın.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            dtSepet.Rows.Add(cmbIlac.SelectedValue, cmbIlac.Text, txtKullanim.Text);
            txtKullanim.Clear();
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            if (cmbHasta.SelectedValue == null || string.IsNullOrWhiteSpace(txtTani.Text) || dtSepet.Rows.Count == 0)
            {
                MessageBox.Show("Hasta seçimi, Tanı ve en az 1 ilaç zorunludur!", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SqlHelper db = new SqlHelper();
                Dictionary<string, object> prmBaslik = new Dictionary<string, object> {
                    { "@hasta_id", cmbHasta.SelectedValue },
                    { "@kullanici_id", Program.AktifKullaniciID },
                    { "@tani", txtTani.Text }
                };
                DataTable dtSonuc = db.GetTable("sp_ReceteBaslikEkle", prmBaslik);
                int yeniReceteID = Convert.ToInt32(dtSonuc.Rows[0]["YeniReceteID"]);

                foreach (DataRow row in dtSepet.Rows)
                {
                    db.GetTable("sp_ReceteDetayEkle", new Dictionary<string, object> {
                        { "@recete_id", yeniReceteID },
                        { "@ilac_id", row["IlacID"] },
                        { "@kullanim_sekli", row["Kullanım Şekli"] }
                    });
                }

                MessageBox.Show("Reçete başarıyla sisteme kaydedildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtTani.Clear();
                dtSepet.Rows.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Kayıt sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ==========================================
        // TASARIM YARDIMCILARI (Modern Görünüm)
        // ==========================================
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
            // Butonlara da oval tasarım veriyoruz
            b.Paint += (s, e) => {
                using (GraphicsPath path = OvalPath(b.Width, b.Height, 10))
                { b.Region = new Region(path); }
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