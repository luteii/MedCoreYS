using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HastaneYonetim.Moduller_Admin
{
    public partial class PersonelListesiEkrani : UserControl
    {
        DataGridView dgvPersonel;
        Color AnaZemin = ColorTranslator.FromHtml("#F4F7FE");
        Color SafBeyaz = ColorTranslator.FromHtml("#FFFFFF");
        Color TextDark = ColorTranslator.FromHtml("#1E293B");
        Color TextMuted = ColorTranslator.FromHtml("#64748B");
        Color AcikMavi = ColorTranslator.FromHtml("#E0F2FE");
        Color KoyuMavi = ColorTranslator.FromHtml("#0284C7");
        TextBox txtArama;

        public PersonelListesiEkrani()
        {
            this.Size = new Size(1000, 700);
            this.BackColor = AnaZemin;
            this.Dock = DockStyle.Fill;
            this.DoubleBuffered = true;
            EkraniKur();
        }

        private void EkraniKur()
        {
            Label lblTitle = new Label
            {
                Text = "Personel Listesi",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(40, 30)
            };
            this.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Veritabanı bağlantısı kurulduğunda, sistemdeki tüm doktor, sekreter ve diğer personeller burada listelenecektir.",
                Font = new Font("Segoe UI", 10),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(40, 80)
            };
            this.Controls.Add(lblSubtitle);

            // Arama Kutusu Alanı
            txtArama = new TextBox
            {
                Location = new Point(40, 110),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 11),
                Text = "Personel Ara (TC, Ad veya Bölüm)...",
                ForeColor = Color.Gray
            };
            txtArama.GotFocus += (s, e) => { if (txtArama.Text == "Personel Ara (TC, Ad veya Bölüm)...") { txtArama.Text = ""; txtArama.ForeColor = TextDark; } };
            txtArama.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtArama.Text)) { txtArama.Text = "Personel Ara (TC, Ad veya Bölüm)..."; txtArama.ForeColor = Color.Gray; } };
            this.Controls.Add(txtArama);

            Button btnAra = new Button
            {
                Text = "🔍 Ara",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(100, 30),
                Location = new Point(350, 110),
                BackColor = KoyuMavi,
                ForeColor = SafBeyaz,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAra.FlatAppearance.BorderSize = 0;
            btnAra.Click += (s, e) => PersonelListesiniGetir(txtArama.Text);
            this.Controls.Add(btnAra);

            // Grid Kapsayıcı Panel
            Panel pnlGridKapsayici = new Panel
            {
                Location = new Point(40, 160),
                Size = new Size(840, 510),
                BackColor = SafBeyaz,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right
            };
            pnlGridKapsayici.Paint += (s, e) => 
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = OvalKose(pnlGridKapsayici.Width - 1, pnlGridKapsayici.Height - 1, 15))
                using (Pen pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1))
                    e.Graphics.DrawPath(pen, path);
            };
            pnlGridKapsayici.Resize += (s, e) => 
            {
                if (pnlGridKapsayici.Width > 0 && pnlGridKapsayici.Height > 0)
                {
                    using (GraphicsPath path = OvalKose(pnlGridKapsayici.Width, pnlGridKapsayici.Height, 15))
                        pnlGridKapsayici.Region = new Region(path);
                }
            };
            this.Controls.Add(pnlGridKapsayici);

            // DataGridView Kurulumu (Placeholder)
            dgvPersonel = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(810, 480),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                BackgroundColor = SafBeyaz,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                ReadOnly = false, // Sadece checkbox düzenlenecek
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ScrollBars = ScrollBars.Both
            };

            dgvPersonel.ColumnHeadersDefaultCellStyle.BackColor = AcikMavi;
            dgvPersonel.ColumnHeadersDefaultCellStyle.ForeColor = KoyuMavi;
            dgvPersonel.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPersonel.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvPersonel.ColumnHeadersHeight = 45;

            dgvPersonel.DefaultCellStyle.BackColor = SafBeyaz;
            dgvPersonel.DefaultCellStyle.ForeColor = TextDark;
            dgvPersonel.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvPersonel.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F1F5F9");
            dgvPersonel.DefaultCellStyle.SelectionForeColor = KoyuMavi;
            dgvPersonel.RowTemplate.Height = 40;
            dgvPersonel.GridColor = ColorTranslator.FromHtml("#E2E8F0");

            dgvPersonel.DataBindingComplete += DgvPersonel_DataBindingComplete;
            dgvPersonel.CellFormatting += DgvPersonel_CellFormatting;
            
            // Checkbox anında tetiklenme ve veritabanı güncelleme
            dgvPersonel.CurrentCellDirtyStateChanged += (s, e) => 
            {
                if (dgvPersonel.IsCurrentCellDirty && dgvPersonel.CurrentCell.OwningColumn.Name == "hesap_aktif_mi")
                {
                    dgvPersonel.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };

            dgvPersonel.CellValueChanged += (s, e) => 
            {
                if (e.RowIndex >= 0 && dgvPersonel.Columns[e.ColumnIndex].Name == "hesap_aktif_mi")
                {
                    try
                    {
                        int kullaniciId = Convert.ToInt32(dgvPersonel.Rows[e.RowIndex].Cells["kullanici_id"].Value);
                        bool yeniDurum = Convert.ToBoolean(dgvPersonel.Rows[e.RowIndex].Cells["hesap_aktif_mi"].Value);

                        SqlHelper db = new SqlHelper();
                        string query = $"UPDATE Kullanicilar SET hesap_aktif_mi = {(yeniDurum ? 1 : 0)} WHERE kullanici_id = {kullaniciId}";
                        db.ExecuteNonQuery(query);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Durum güncellenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };

            pnlGridKapsayici.Controls.Add(dgvPersonel);

            if (!this.DesignMode)
            {
                PersonelListesiniGetir();
            }
        }

        private void DgvPersonel_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvPersonel.Columns.Count > 0)
            {
                if (dgvPersonel.Columns.Contains("kullanici_id"))
                    dgvPersonel.Columns["kullanici_id"].Width = 80;
                
                if (dgvPersonel.Columns.Contains("tc_no"))
                    dgvPersonel.Columns["tc_no"].Width = 150;

                if (dgvPersonel.Columns.Contains("Telefon"))
                    dgvPersonel.Columns["Telefon"].Width = 150;

                if (dgvPersonel.Columns.Contains("rol_ID"))
                    dgvPersonel.Columns["rol_ID"].Width = 120;

                // Ad Soyad fill the rest
                if (dgvPersonel.Columns.Contains("ad_soyad"))
                    dgvPersonel.Columns["ad_soyad"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            dgvPersonel.ClearSelection();
        }

        private void DgvPersonel_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPersonel.Columns[e.ColumnIndex].Name == "rol_ID" && e.Value != null)
            {
                if (int.TryParse(e.Value.ToString(), out int rolId))
                {
                    if (rolId == 1) e.Value = "1 / Admin";
                    else if (rolId == 2) e.Value = "2 / Doktor";
                    else if (rolId == 3) e.Value = "3 / Sekreter";
                    else if (rolId == 4) e.Value = "4 / Hemşire";
                    else if (rolId == 5) e.Value = "5 / Laborant";
                    else if (rolId == 6) e.Value = "6 / Veznedar";
                    else if (rolId == 7) e.Value = "7 / Hasta Bakıcı";
                    else if (rolId == 8) e.Value = "8 / Başhekim";
                    else if (rolId == 10) e.Value = "10 / Hasta";
                    else e.Value = $"{rolId} / Belirsiz";
                    e.FormattingApplied = true;
                }
            }
        }

        private void PersonelListesiniGetir(string aramaMetni = "")
        {
            try
            {
                SqlHelper db = new SqlHelper();
                DataTable dt = db.GetTable("sp_KullanicilariGetir");
                
                if (!string.IsNullOrWhiteSpace(aramaMetni) && aramaMetni != "Personel Ara (TC, Ad veya Bölüm)...")
                {
                    DataView dv = dt.DefaultView;
                    dv.RowFilter = string.Format("Convert(tc_no, 'System.String') LIKE '%{0}%' OR ad_soyad LIKE '%{0}%'", aramaMetni.Replace("'", "''"));
                    dgvPersonel.DataSource = dv;
                }
                else
                {
                    dgvPersonel.DataSource = dt;
                }
                
                if (dgvPersonel.Columns.Count > 0)
                {
                    if (dgvPersonel.Columns.Contains("kullanici_id")) dgvPersonel.Columns["kullanici_id"].HeaderText = "ID";
                    if (dgvPersonel.Columns.Contains("tc_no")) dgvPersonel.Columns["tc_no"].HeaderText = "TC Kimlik No";
                    if (dgvPersonel.Columns.Contains("ad_soyad")) dgvPersonel.Columns["ad_soyad"].HeaderText = "Ad Soyad";
                    if (dgvPersonel.Columns.Contains("Sifre")) dgvPersonel.Columns["Sifre"].Visible = false;
                    if (dgvPersonel.Columns.Contains("sifre")) dgvPersonel.Columns["sifre"].Visible = false;
                    if (dgvPersonel.Columns.Contains("rol_ID")) dgvPersonel.Columns["rol_ID"].HeaderText = "Rol";
                    if (dgvPersonel.Columns.Contains("son_giris_tarihi")) dgvPersonel.Columns["son_giris_tarihi"].HeaderText = "Son Giriş Tarihi";
                    if (dgvPersonel.Columns.Contains("hesap_aktif_mi")) dgvPersonel.Columns["hesap_aktif_mi"].HeaderText = "Aktiflik Durumu";
                    
                    // Sadece hesap_aktif_mi kolonu düzenlenebilir olsun
                    foreach(DataGridViewColumn col in dgvPersonel.Columns)
                    {
                        if (col.Name != "hesap_aktif_mi")
                        {
                            col.ReadOnly = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Personel yükleme hatası: " + ex.Message);
            }
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

