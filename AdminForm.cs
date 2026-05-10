using cargo_tracker.arayuz;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace cargo_tracker
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
        }
        public void AdminTabloyuDoldur()
        {
            using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
            {
                string sorgu = "SELECT TakipNo, Gonderen, Alici, Adres, islem, saat, tarih, personel_durum, Birim FROM kargolar";
                MySqlDataAdapter da = new MySqlDataAdapter(sorgu, baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvKargolar.DataSource = dt;
                dgvKargolar.Columns["takipNo"].HeaderText = "Takip No";
                dgvKargolar.Columns["gonderen"].HeaderText = "Gönderen";
                dgvKargolar.Columns["alici"].HeaderText = "Alıcı";
                dgvKargolar.Columns["adres"].HeaderText = "Adres";
                dgvKargolar.Columns["tarih"].HeaderText = "Tarih";
                dgvKargolar.Columns["saat"].HeaderText = "Saat";
                dgvKargolar.Columns["islem"].HeaderText = "İşlem Durumu";
                dgvKargolar.Columns["birim"].HeaderText = "Birim";
                dgvKargolar.Columns["personel_durum"].HeaderText = "Kurye Bildirimi";

                dgvKargolar.Columns["takipNo"].Width = 80;
                dgvKargolar.Columns["gonderen"].Width = 80;
                dgvKargolar.Columns["alici"].Width = 80;
                dgvKargolar.Columns["adres"].Width = 80;
                dgvKargolar.Columns["tarih"].Width = 80;
                dgvKargolar.Columns["saat"].Width = 80;
                dgvKargolar.Columns["islem"].Width = 100;
                dgvKargolar.Columns["birim"].Width = 80;
                dgvKargolar.Columns["personel_durum"].Width = 100;
            }
        }
        public class Kargo
        {
            public string TakipNo { get; set; }
            public string Gonderen { get; set; }
            public string Alici { get; set; }
            public string Adres { get; set; }
            public string Birim { get; set; }
            public string Islem { get; set; }
            public DateTime Tarih { get; set; }
            public string Saat { get; set; }
        }
        public class KargoRepository
        {
            public void Guncelle(Kargo kargo)
            {
                using (MySqlConnection baglanti = new MySqlConnection(connString))
                {

                    string sorgu = "UPDATE kargolar SET gonderen=@gonderen, alici=@alici, adres=@adres," + "birim=@birim, islem=@islem, tarih=@tarih, saat=@saat WHERE takipNo=@takipNo";

                    using (MySqlCommand komut = new MySqlCommand(sorgu, baglanti))
                    {
                        komut.Parameters.AddWithValue("@gonderen", kargo.Gonderen);
                        komut.Parameters.AddWithValue("@alici", kargo.Alici);
                        komut.Parameters.AddWithValue("@adres", kargo.Adres);
                        komut.Parameters.AddWithValue("@birim", kargo.Birim);
                        komut.Parameters.AddWithValue("@islem", kargo.Islem);
                        komut.Parameters.AddWithValue("@tarih", kargo.Tarih);
                        komut.Parameters.AddWithValue("@saat", kargo.Saat);
                        komut.Parameters.AddWithValue("@takipNo", kargo.TakipNo);

                        baglanti.Open();
                        komut.ExecuteNonQuery();
                    }
                }
            }

            private string connString = "Server=localhost;Database=kargo_takip;Uid=root;Pwd=;";

            public void Ekle(Kargo kargo)
            {
                using (MySqlConnection baglanti = new MySqlConnection(connString))
                {
                    string sorgu = "INSERT INTO kargolar (takipNo, gonderen, alici, adres, birim, tarih, saat, islem) " +
                                   "VALUES (@takipNo, @gonderen, @alici, @adres, @birim, @tarih, @saat, @islem)";

                    using (MySqlCommand komut = new MySqlCommand(sorgu, baglanti))
                    {
                        komut.Parameters.AddWithValue("@takipNo", kargo.TakipNo);
                        komut.Parameters.AddWithValue("@gonderen", kargo.Gonderen);
                        komut.Parameters.AddWithValue("@alici", kargo.Alici);
                        komut.Parameters.AddWithValue("@adres", kargo.Adres);
                        komut.Parameters.AddWithValue("@birim", kargo.Birim);
                        komut.Parameters.AddWithValue("@tarih", kargo.Tarih.ToString("yyyy-MM-dd"));
                        komut.Parameters.AddWithValue("@saat", kargo.Tarih.ToString("HH:mm:ss"));
                        komut.Parameters.AddWithValue("@islem", kargo.Islem);

                        baglanti.Open();
                        komut.ExecuteNonQuery();
                    }
                }
            }
        }
        public class KargoServisi
        {
            private KargoRepository _repo = new KargoRepository();

            public string YeniKargoKaydet(Kargo kargo)
            {

                kargo.TakipNo = KargoData.RastgeleTakipNoUret();
                kargo.Tarih = DateTime.Now;

                try
                {
                    _repo.Ekle(kargo);
                    return $"Kargo başarıyla kaydedildi! Takip No: {kargo.TakipNo}";
                }
                catch (Exception ex)
                {
                    throw new Exception("Kargo kaydedilirken bir hata oluştu: " + ex.Message);
                }
            }
        }
        private void btnEkle_Click(object sender, EventArgs e)
        {
            try
            {

                var yeniKargo = new Kargo
                {
                    Gonderen = txtGonderici.Text,
                    Alici = txtAlici.Text,
                    Adres = txtAdres.Text,
                    Birim = txtBirim.Text,
                    Islem = cmbIslem.Text
                };


                var servis = new KargoServisi();
                string sonucMesaji = servis.YeniKargoKaydet(yeniKargo);


                MessageBox.Show(sonucMesaji);
                AdminTabloyuDoldur();
                KutulariTemizle();
                dgvKargolar.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbIslem_Enter(object sender, EventArgs e)
        {
            if (cmbIslem.Text == "İşlem Seçiniz")
            {
                cmbIslem.Text = "";
                cmbIslem.ForeColor = Color.Black;
            }
        }

        private void cmbIslem_Leave(object sender, EventArgs e)
        {
            if (cmbIslem.Text == "")
            {
                cmbIslem.Text = "İşlem Seçiniz";
                cmbIslem.ForeColor = Color.Gray;
            }
        }

        private void txtAdres_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetterOrDigit(e.KeyChar) ||
                     char.IsWhiteSpace(e.KeyChar) ||
                         e.KeyChar == '/' ||
                         e.KeyChar == '-' ||
                         e.KeyChar == '.' ||
                         e.KeyChar == ',' ||
                         e.KeyChar == ':')
            {
                return;
            }

            if (char.IsControl(e.KeyChar))
                return;


            if (!char.IsLetter(e.KeyChar) &&
                !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void SadeceHarf_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (char.IsControl(e.KeyChar))
                return;


            if (!char.IsLetter(e.KeyChar) &&
                !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void AdminForm_Load(object sender, EventArgs e)
        {
            PersonelListele();
            AdminTabloyuDoldur();
            AliciOnerileriniYukle();
            TelefonOnerileriniYukle();
            KutulariTemizle();
            dgvKargolar.ColumnHeadersDefaultCellStyle.BackColor = Color.SlateGray;
            dgvKargolar.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKargolar.EnableHeadersVisualStyles = false;

            dgvKargolar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKargolar.MultiSelect = false;
            dgvKargolar.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvKargolar.ClearSelection();
            dgvPersonel.ClearSelection();
        }
        private void TelefonOnerileriniYukle()
        {
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";
            AutoCompleteStringCollection liste = new AutoCompleteStringCollection();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT DISTINCT telefon FROM kullanicilar WHERE telefon IS NOT NULL";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            liste.Add(dr["telefon"].ToString());
                        }
                    }

                    textBox1.AutoCompleteCustomSource = liste;
                }
                catch (Exception)
                { }
            }
        }
        private void KutulariTemizle()
        {
            txtGonderici.Clear();
            txtAlici.Clear();
            txtAdres.Clear();
            txtBirim.Clear();
            textBox1.Clear();
            cmbIslem.SelectedIndex = -1;
        }
        private void btnHareketEkle_Click(object sender, EventArgs e)
        {
            if (dgvKargolar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz kargoyu tablodan seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string takipNo = dgvKargolar.SelectedRows[0].Cells["TakipNo"].Value.ToString();
            string gonderen = txtGonderici.Text;
            string alici = txtAlici.Text;
            string adres = txtAdres.Text;
            string birim = txtBirim.Text;
            string durum = cmbIslem.Text;

            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sorgu = @"UPDATE kargolar SET 
                             gonderen = @gonderen, 
                             alici = @alici, 
                             adres = @adres, 
                             islem = @durum, 
                             birim = @birim,
                             tarih = @tarih,
                             saat = @saat
                             WHERE TakipNo = @no";

                    using (MySqlCommand cmd = new MySqlCommand(sorgu, conn))
                    {
                        cmd.Parameters.AddWithValue("@no", takipNo);
                        cmd.Parameters.AddWithValue("@gonderen", gonderen);
                        cmd.Parameters.AddWithValue("@alici", alici);
                        cmd.Parameters.AddWithValue("@adres", adres);
                        cmd.Parameters.AddWithValue("@durum", durum);
                        cmd.Parameters.AddWithValue("@birim", birim);
                        cmd.Parameters.AddWithValue("@tarih", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@saat", DateTime.Now.ToString("HH:mm:ss"));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Kargo bilgileri başarıyla güncellendi!", "Esferix", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    AdminTabloyuDoldur();
                    KutulariTemizle();
                    dgvKargolar.ClearSelection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Güncelleme hatası: " + ex.Message);
                }
            }
        }

        private void txtAdres_Enter(object sender, EventArgs e)
        {
            if (txtAdres.Text == "Adres")
            {
                txtAdres.Text = "";
                txtAdres.ForeColor = Color.Black;
            }
        }

        private void txtAdres_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAdres.Text))
            {
                txtAdres.Text = "Adres";
                txtAdres.ForeColor = Color.Gray;
            }
        }

        private void AdminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.AdminAcik = false;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Program.AdminAcik = false;
            LoginForm login = new LoginForm();
            login.Show();

            this.Close();
        }

        private void dgvKargolar_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKargolar.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvKargolar.SelectedRows[0];

                txtGonderici.Text = row.Cells["gonderen"].Value?.ToString();
                txtAlici.Text = row.Cells["alici"].Value?.ToString();
                txtAdres.Text = row.Cells["adres"].Value?.ToString();
                txtBirim.Text = row.Cells["birim"].Value?.ToString();
                cmbIslem.Text = row.Cells["islem"].Value?.ToString();

                txtGonderici.BackColor = Color.White;
                txtGonderici.ForeColor = Color.Black;
                txtAlici.BackColor = Color.White;
                txtAlici.ForeColor = Color.Black;
                txtAdres.BackColor = Color.White;
                txtAdres.ForeColor = Color.Black;
                txtBirim.BackColor = Color.White;
                txtBirim.ForeColor = Color.Black;
                cmbIslem.BackColor = Color.White;
                cmbIslem.ForeColor = Color.Black;
            }
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || textBox1.Text.Length < 10)
            {
                MessageBox.Show("Lütfen geçerli bir telefon numarası giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string telefonNo = textBox1.Text;
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string kontrolSorgusu = "SELECT COUNT(*) FROM kullanicilar WHERE telefon = @tel";
                    using (MySqlCommand kontrolCmd = new MySqlCommand(kontrolSorgusu, conn))
                    {
                        kontrolCmd.Parameters.AddWithValue("@tel", telefonNo);
                        int kayitSayisi = Convert.ToInt32(kontrolCmd.ExecuteScalar());

                        if (kayitSayisi == 0)
                        {
                            MessageBox.Show("Bu telefon numarasına ait bir kayıt bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    string deleteQuery = "DELETE FROM kargo_dogrulama WHERE telefon = @tel";
                    using (MySqlCommand delCmd = new MySqlCommand(deleteQuery, conn))
                    {
                        delCmd.Parameters.AddWithValue("@tel", telefonNo);
                        delCmd.ExecuteNonQuery();
                    }

                    string randomCode = new Random().Next(100000, 999999).ToString();

                    string upsertQuery = @"INSERT INTO kargo_dogrulama (telefon, dogrulama_kodu) 
                       VALUES (@tel, @kod) 
                       ON DUPLICATE KEY UPDATE dogrulama_kodu = @kod";

                    using (MySqlCommand upsertCmd = new MySqlCommand(upsertQuery, conn))
                    {
                        upsertCmd.Parameters.AddWithValue("@tel", telefonNo);
                        upsertCmd.Parameters.AddWithValue("@kod", randomCode);
                        upsertCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Numara doğrulandı ve kod gönderildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
            }
        }
        private void AliciOnerileriniYukle()
        {
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";
            AutoCompleteStringCollection liste = new AutoCompleteStringCollection();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT kullanici_adi FROM kullanicilar";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read()) { liste.Add(dr["kullanici_adi"].ToString()); }
                    }
                    txtAlici.AutoCompleteCustomSource = liste;
                }
                catch { }
            }
        }
        private void txtAlici_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAlici.Text)) return;

            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT telefon, adres FROM kullanicilar WHERE kullanici_adi = @ad";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ad", txtAlici.Text);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            textBox1.Text = dr["telefon"].ToString();
                            txtAdres.Text = dr["adres"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bilgiler getirilirken hata oluştu: " + ex.Message);
                }
            }
        }
        private void txtAlici_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtAlici_Leave(sender, EventArgs.Empty);
            }
        }

        private void panel7_Click(object sender, EventArgs e)
        {
            dgvKargolar.ClearSelection();
            KutulariTemizle();
        }

        private void AdminForm_Click(object sender, EventArgs e)
        {
            label12.Text = string.Empty;
            pictureBox3.Image = null;
            dgvKargolar.ClearSelection();
            dgvPersonel.ClearSelection();
            KutulariTemizle();
        }
        private void dgvKargolar_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKargolar.Rows[e.RowIndex];

                txtGonderici.Text = row.Cells["gonderen"].Value?.ToString();
                txtAlici.Text = row.Cells["alici"].Value?.ToString();
                txtAdres.Text = row.Cells["adres"].Value?.ToString();
                txtBirim.Text = row.Cells["birim"].Value?.ToString();
                cmbIslem.Text = row.Cells["islem"].Value?.ToString();

                txtGonderici.BackColor = Color.White;
                txtGonderici.ForeColor = Color.Black;
                txtAlici.BackColor = Color.White;
                txtAlici.ForeColor = Color.Black;
                txtAdres.BackColor = Color.White;
                txtAdres.ForeColor = Color.Black;
                txtBirim.BackColor = Color.White;
                txtBirim.ForeColor = Color.Black;
                cmbIslem.BackColor = Color.White;
                cmbIslem.ForeColor = Color.Black;
            }
        }
        private void txtKargoAra_TextChanged(object sender, EventArgs e)
        {
            string aranan = txtKargoAra.Text.Trim();

            using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
            {
                baglanti.Open();

                string sorgu = "SELECT TakipNo, Gonderen, Alici, Adres, Tarih, Saat, islem, Birim, secilenSaat FROM kargolar WHERE " +
                               "TakipNo LIKE @p1 OR Gonderen LIKE @p1 OR Alici LIKE @p1";

                MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@p1", "%" + aranan + "%"); 

                MySqlDataAdapter da = new MySqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvKargolar.DataSource = dt;
            }
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            if (dgvKargolar.SelectedRows.Count > 0)
            {
                DialogResult onay = MessageBox.Show("Seçili kargoyu silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (onay == DialogResult.Yes)
                {
                    try
                    {
                        int seciliId = Convert.ToInt32(dgvKargolar.SelectedRows[0].Cells["TakipNo"].Value);

                        using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
                        {
                            baglanti.Open();

                            string hareketSilSorgu = "DELETE FROM kargohareketleri WHERE TakipNo = @TakipNo";
                            MySqlCommand cmdHareket = new MySqlCommand(hareketSilSorgu, baglanti);
                            cmdHareket.Parameters.AddWithValue("@TakipNo", seciliId);
                            cmdHareket.ExecuteNonQuery();

                            string kargoSilSorgu = "DELETE FROM kargolar WHERE TakipNo = @TakipNo";
                            MySqlCommand cmdKargo = new MySqlCommand(kargoSilSorgu, baglanti);
                            cmdKargo.Parameters.AddWithValue("@TakipNo", seciliId);
                            cmdKargo.ExecuteNonQuery();

                            baglanti.Close();
                        }

                        MessageBox.Show("Kargo başarıyla silindi.");
                        AdminTabloyuDoldur();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz kargoyu listeden seçin!");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            personelKayit frm = new personelKayit();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                PersonelListele();
            }
        }
        public void PersonelListele()
        {
            using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
            {
                MySqlDataAdapter da = new MySqlDataAdapter("SELECT ad, soyad, kullanici_adi, eposta, kimlik_no, fotograf_yolu, sifre, telefon FROM personeller", baglanti);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvPersonel.DataSource = dt;
                dgvPersonel.Columns["ad"].HeaderText = "Ad";
                dgvPersonel.Columns["soyad"].HeaderText = "Soyad";
                dgvPersonel.Columns["kullanici_adi"].HeaderText = "Kullanıcı Adı";
                dgvPersonel.Columns["eposta"].HeaderText = "E-posta";
                dgvPersonel.Columns["telefon"].HeaderText = "Telefon";
                dgvPersonel.Columns["kimlik_no"].HeaderText = "Kimlik Numarası";
                dgvPersonel.Columns["fotograf_yolu"].Visible = false;
                dgvPersonel.Columns["sifre"].Visible = false;
            }
        }

        private void dgvPersonel_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPersonel.Rows[e.RowIndex];

                label12.Text = row.Cells["ad"].Value?.ToString() + " " + row.Cells["soyad"].Value?.ToString();

                try
                {
                    string anaKlasor = @"C:\EsferixKargo\PersonelFotograflari\";
                    string fotoYolu = row.Cells["fotograf_yolu"].Value?.ToString();
                    string tamYol = System.IO.Path.Combine(anaKlasor, fotoYolu);
                    if (!string.IsNullOrEmpty(fotoYolu) && System.IO.File.Exists(tamYol))
                    {
                        using (var stream = new System.IO.FileStream(tamYol, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            pictureBox3.Image = Image.FromStream(stream);
                        }
                    }
                    else
                    {
                        pictureBox3.Image = null; 
                    }
                }
                catch (Exception)
                {
                    pictureBox3.Image = null;
                }

                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            string aranan = txtAra.Text.Trim();

            using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
            {
                baglanti.Open();

                string sorgu = "SELECT ad, soyad, kullanici_adi, eposta, kimlik_no, fotograf_yolu, telefon FROM personeller WHERE " +
                               "kullanici_adi LIKE @p1 OR ad LIKE @p1 OR soyad LIKE @p1";

                MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@p1", "%" + aranan + "%");

                MySqlDataAdapter da = new MySqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvPersonel.DataSource = dt;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dgvPersonel.SelectedRows.Count > 0)
            {
                DialogResult onay = MessageBox.Show("Seçili personeli silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (onay == DialogResult.Yes)
                {
                    try
                    {
                        string seciliKullanici = dgvPersonel.SelectedRows[0].Cells["kullanici_adi"].Value.ToString();

                        using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
                        {
                            baglanti.Open();

                            string silSorgu = "DELETE FROM personeller WHERE kullanici_adi = @kadi";

                            MySqlCommand cmd = new MySqlCommand(silSorgu, baglanti);
                            cmd.Parameters.AddWithValue("@kadi", seciliKullanici);

                            cmd.ExecuteNonQuery();
                            baglanti.Close();
                        }

                        MessageBox.Show("Personel başarıyla silindi.");
                        PersonelListele();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz personeli listeden seçin!");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dgvPersonel.SelectedRows.Count > 0)
            {
                personelKayit frm = new personelKayit();
                frm.guncellemeModu = true;

                DataGridViewRow satir = dgvPersonel.SelectedRows[0];
                frm.txtAd.Text = satir.Cells["ad"].Value.ToString();
                frm.txtSoyad.Text = satir.Cells["soyad"].Value.ToString();
                frm.txtKullaniciAdi.Text = satir.Cells["kullanici_adi"].Value.ToString();
                frm.txtMail.Text = satir.Cells["eposta"].Value.ToString();
                frm.txtKimlik.Text = satir.Cells["kimlik_no"].Value.ToString();
                frm.msktxtTelefon.Text = satir.Cells["telefon"].Value.ToString();
                string fotoAdi = satir.Cells["fotograf_yolu"].Value?.ToString();
                frm.txtSifre.Text = satir.Cells["sifre"].Value?.ToString();
                frm.eskiFotoAdi = fotoAdi;

                frm.eskiKullaniciAdi = satir.Cells["kullanici_adi"].Value.ToString();

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    PersonelListele(); 
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz personeli seçin!");
            }
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            label12.Text = string.Empty;
            pictureBox3.Image = null;
            dgvPersonel.ClearSelection();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dgvKargolar.SelectedRows.Count > 0 && dgvPersonel.SelectedRows.Count > 0)
            {
                string seciliTakipNo = dgvKargolar.SelectedRows[0].Cells["TakipNo"].Value.ToString();
                string seciliPersonelKadi = dgvPersonel.SelectedRows[0].Cells["kullanici_adi"].Value.ToString();
                string personelAdSoyad = dgvPersonel.SelectedRows[0].Cells["ad"].Value.ToString() + " " +
                                         dgvPersonel.SelectedRows[0].Cells["soyad"].Value.ToString();

                try
                {
                    using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
                    {
                        baglanti.Open();

                        string kontrolSorgu = "SELECT COUNT(*) FROM kargolar WHERE TakipNo = @tNo AND personel_kadi = @pKadi";
                        MySqlCommand kontrolKomut = new MySqlCommand(kontrolSorgu, baglanti);
                        kontrolKomut.Parameters.AddWithValue("@tNo", seciliTakipNo);
                        kontrolKomut.Parameters.AddWithValue("@pKadi", seciliPersonelKadi);

                        int kayitSayisi = Convert.ToInt32(kontrolKomut.ExecuteScalar());

                        if (kayitSayisi > 0)
                        {
                            MessageBox.Show("Bu personel zaten bu kargoya atanmış durumda!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; 
                        }

                        DialogResult onay = MessageBox.Show($"{seciliTakipNo} numaralı kargoyu {personelAdSoyad} personeline atamak istediğinize emin misiniz?",
                            "Atama Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (onay == DialogResult.Yes)
                        {
                            string sorgu = "UPDATE kargolar SET personel_kadi = @pKadi, islem = @durum WHERE TakipNo = @tNo";
                            MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                            komut.Parameters.AddWithValue("@pKadi", seciliPersonelKadi);
                            komut.Parameters.AddWithValue("@durum", "Personel Teslim Aldı");
                            komut.Parameters.AddWithValue("@tNo", seciliTakipNo);

                            komut.ExecuteNonQuery();

                            MessageBox.Show("Kargo başarıyla personele atandı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            PersonelListele();
                            AdminTabloyuDoldur();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Atama sırasında hata oluştu: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Lütfen hem kargo listesinden hem de personel listesinden seçim yapınız!", "Eksik Seçim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void panel4_Click(object sender, EventArgs e)
        {
            label12.Text = string.Empty;
            pictureBox3.Image = null;
        }

        private void panel6_Click(object sender, EventArgs e)
        {
            dgvKargolar.ClearSelection();
            KutulariTemizle();
        }
    }
}