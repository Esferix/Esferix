using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace cargo_tracker.arayuz
{
    public partial class PersonelForm : Form
    {
        public string girisYapanKullanici;
        public PersonelForm()
        {
            InitializeComponent();
        }
        private void KargoListele(string kullaniciAdi)
        {
            try
            {
                using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
                {
                    baglanti.Open();
                    string sorgu = @"SELECT 
                    k.TakipNo, 
                    k.Gonderen, 
                    k.Alici, 
                    k.Adres, 
                    k.Tarih, 
                    k.Saat, 
                    k.personel_durum,
                    k.islem, 
                    k.Birim, 
                    k.SecilenSaat,
                    u.telefon 
                FROM kargolar k 
                INNER JOIN kullanicilar u ON k.Alici = u.kullanici_adi 
                WHERE k.personel_kadi = @p";
                    MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@p", kullaniciAdi);

                    MySqlDataAdapter da = new MySqlDataAdapter(komut);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvKargolar.DataSource = dt;

                    if (dgvKargolar.Columns.Contains("TakipNo")) dgvKargolar.Columns["TakipNo"].HeaderText = "Takip No";
                    if (dgvKargolar.Columns.Contains("Gonderen")) dgvKargolar.Columns["Gonderen"].HeaderText = "Gönderen";
                    if (dgvKargolar.Columns.Contains("Alici")) dgvKargolar.Columns["Alici"].HeaderText = "Alıcı";
                    if (dgvKargolar.Columns.Contains("Adres")) dgvKargolar.Columns["Adres"].HeaderText = "Adres";
                    if (dgvKargolar.Columns.Contains("personel_durum")) dgvKargolar.Columns["personel_durum"].HeaderText = "İşlem Durumu";
                    if (dgvKargolar.Columns.Contains("adres")) dgvKargolar.Columns["adres"].HeaderText = "Adres";
                    if (dgvKargolar.Columns.Contains("telefon")) dgvKargolar.Columns["telefon"].HeaderText = "Telefon";
                    if (dgvKargolar.Columns.Contains("Tarih")) dgvKargolar.Columns["Tarih"].HeaderText = "Tarih";
                    if (dgvKargolar.Columns.Contains("Saat")) dgvKargolar.Columns["Saat"].HeaderText = "Saat";
                    if (dgvKargolar.Columns.Contains("Birim")) dgvKargolar.Columns["Birim"].HeaderText = "Birim";
                    if (dgvKargolar.Columns.Contains("SecilenSaat")) dgvKargolar.Columns["SecilenSaat"].HeaderText = "Seçilen Saat";
                    dgvKargolar.Columns["islem"].Visible = false;

                    dgvKargolar.Columns["takipNo"].Width = 80;
                    dgvKargolar.Columns["gonderen"].Width = 80;
                    dgvKargolar.Columns["alici"].Width = 80;
                    dgvKargolar.Columns["adres"].Width = 80;
                    dgvKargolar.Columns["tarih"].Width = 80;
                    dgvKargolar.Columns["saat"].Width = 80;
                    dgvKargolar.Columns["personel_durum"].Width = 100;
                    dgvKargolar.Columns["birim"].Width = 80;
                    dgvKargolar.Columns["secilenSaat"].Width = 80;
                    dgvKargolar.Columns["telefon"].Width = 100;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kargolar listelenirken hata oluştu: " + ex.Message);
            }
        }
        private void PersonelForm_Load(object sender, EventArgs e)
        {
            KargoListele(girisYapanKullanici);
            dgvKargolar.ClearSelection();
            KutulariTemizle();
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            Personel per = new Personel();
            per.Show();
            this.Close();
        }

        private void KargoyuKaydet(string kaynakYol, string takipNo)
        {
            try
            {
                string hedefKlasor = @"C:\EsferixKargo\TeslimatFotograflari";
                if (!Directory.Exists(hedefKlasor)) Directory.CreateDirectory(hedefKlasor);

                string dosyaAdi = "kargo_" + takipNo + Path.GetExtension(kaynakYol);
                string hedefYol = Path.Combine(hedefKlasor, dosyaAdi);

                File.Copy(kaynakYol, hedefYol, true);

                VeritabaninaYolKaydet(dosyaAdi, takipNo);

                MessageBox.Show(takipNo + " numaralı kargoya fotoğraf eklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        private void VeritabaninaYolKaydet(string dosyaAdi, string takipNo)
        {
            string baglantiCumlesi = "Server=localhost;Database=kargo_takip;Uid=root;Pwd=;";

            using (MySqlConnection baglanti = new MySqlConnection(baglantiCumlesi))
            {
                string query = "UPDATE kargolar SET teslimat_fotograf = @foto WHERE TakipNo = @no";
                MySqlCommand cmd = new MySqlCommand(query, baglanti);
                cmd.Parameters.AddWithValue("@foto", dosyaAdi);
                cmd.Parameters.AddWithValue("@no", takipNo);

                baglanti.Open();
                cmd.ExecuteNonQuery();
                baglanti.Close();
            }
        }
        private string FotoYolunuGetir(string takipNo)
        {
            string yol = "";
            using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
            {
                baglanti.Open();
                string sorgu = "SELECT teslimat_fotograf FROM kargolar WHERE TakipNo = @p1";
                MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@p1", takipNo);

                object sonuc = komut.ExecuteScalar();
                if (sonuc != null) yol = sonuc.ToString();
            }
            return yol;
        }
        private void dgvKargolar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvKargolar.Rows[e.RowIndex];
                string takipNo = row.Cells["TakipNo"].Value.ToString();

                if (row.Cells["personel_durum"].Value != null)
                {
                    cmbIslem.Text = row.Cells["personel_durum"].Value.ToString();
                    cmbIslem.ForeColor = Color.Black; 
                }
                button2.Visible = true;
                pictureBox2.BackColor = Color.Transparent;

                string dosyaAdi = FotoYolunuGetir(takipNo);
                string anaKlasor = @"C:\EsferixKargo\TeslimatFotograflari\";
                string tamYol = Path.Combine(anaKlasor, dosyaAdi);

                if (!string.IsNullOrEmpty(dosyaAdi) && System.IO.File.Exists(tamYol))
                {
                    using (var stream = new System.IO.FileStream(tamYol, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        pictureBox2.Image = Image.FromStream(stream);
                        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
                else
                {
                    pictureBox2.Image = null; 
                }
            }
        }

        private void txtKargoAra_TextChanged(object sender, EventArgs e)
        {
            string aranan = txtKargoAra.Text.Trim();

            using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
            {
                baglanti.Open();

                string sorgu = @"SELECT k.TakipNo, k.Gonderen, k.Alici, k.Adres, k.Tarih, k.Saat, 
                k.personel_durum, k.islem, k.Birim, k.secilenSaat, u.telefon 
                FROM kargolar k 
                LEFT JOIN kullanicilar u ON k.Alici = u.kullanici_adi 
                WHERE k.TakipNo LIKE @p1 OR k.Gonderen LIKE @p1 OR k.Alici LIKE @p1";

                MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@p1", "%" + aranan + "%");

                MySqlDataAdapter da = new MySqlDataAdapter(komut);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvKargolar.DataSource = dt;
            }
        }

        private void panel7_Click(object sender, EventArgs e)
        {
            dgvKargolar.ClearSelection();
            KutulariTemizle();
        }
        private void KutulariTemizle()
        {
            pictureBox2.Image = null;
            button2.Visible = false;
            cmbIslem.SelectedIndex = -1;
            cmbIslem.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvKargolar.CurrentRow != null)
            {
                string takipNo = dgvKargolar.CurrentRow.Cells["TakipNo"].Value.ToString();
                string yeniDurum = cmbIslem.Text;
                string guncelTarih = DateTime.Now.ToString("yyyy-MM-dd"); 
                string guncelSaat = DateTime.Now.ToString("HH:mm:ss");

                if (string.IsNullOrEmpty(yeniDurum) || yeniDurum == "İşlem Seçiniz")
                {
                    MessageBox.Show("Lütfen geçerli bir işlem durumu seçin.");
                    return;
                }

                using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
                {
                    try
                    {
                        baglanti.Open();
                        string sorgu = "UPDATE kargolar SET Tarih=@tarih, Saat=@saat, personel_durum=@durum WHERE TakipNo=@no";
                        MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                        komut.Parameters.AddWithValue("@durum", cmbIslem.Text);
                        komut.Parameters.AddWithValue("@tarih", guncelTarih);
                        komut.Parameters.AddWithValue("@saat", guncelSaat);
                        komut.Parameters.AddWithValue("@no", dgvKargolar.CurrentRow.Cells["TakipNo"].Value.ToString());

                        int sonuc = komut.ExecuteNonQuery();

                        if (sonuc > 0)
                        {
                            MessageBox.Show(takipNo + " numaralı kargonun durumu '" + yeniDurum + "' olarak güncellendi.");

                            KargoListele(girisYapanKullanici);
                            KutulariTemizle();
                            dgvKargolar.ClearSelection();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Güncelleme sırasında hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen listeden güncellenecek kargoyu seçin.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string seciliTakipNo = dgvKargolar.CurrentRow.Cells["TakipNo"].Value.ToString();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    pictureBox2.Image = Image.FromStream(stream);
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                }
                KargoyuKaydet(openFileDialog.FileName, seciliTakipNo);
            }
        }

        private void cmbIslem_Leave(object sender, EventArgs e)
        {

        }

        private void cmbIslem_Enter(object sender, EventArgs e)
        {

        }

        private void PersonelForm_Click(object sender, EventArgs e)
        {
            dgvKargolar.ClearSelection();
            KutulariTemizle();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dgvKargolar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen önce listeden bir kargo seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string takipNo = dgvKargolar.SelectedRows[0].Cells["TakipNo"].Value?.ToString();

            Detaylar frm = new Detaylar();
            frm.SeciliTakipNo = takipNo;
            frm.Show();
        }
    }
}
