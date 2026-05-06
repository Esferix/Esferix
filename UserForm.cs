using cargo_tracker;
using cargo_tracker.arayuz;
using cargo_tracker.destek;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static cargo_tracker.AdminForm;
using static cargo_tracker.UserLogin;

namespace cargo_tracker
{
    public partial class UserForm : Form
    {
        private string girisYapanKullaniciAd;
        public UserForm(string kullaniciAdi)
        {
            InitializeComponent();
            this.girisYapanKullaniciAd = kullaniciAdi;
        }
        public class KargoRepository
        {
            private string connString = "Server=localhost;Database=kargo_takip;Uid=root;Pwd=;";

            public List<string> KullaniciKargolariniGetir(string kullaniciMail)
            {
                List<string> numaralar = new List<string>();
                using (MySqlConnection baglanti = new MySqlConnection(connString))
                {
                    string sorgu = @"SELECT 
                    k.takipNo, 
                    k.gonderen, 
                    k.alici, 
                    k.adres, 
                    k.tarih, 
                    k.saat, 
                    k.islem, 
                    k.birim, 
                    k.secilenSaat,
                    p.ad, 
                    p.telefon
                FROM kargolar k
                INNER JOIN kullanicilar u ON k.alici = u.kullanici_adi
                LEFT JOIN personeller p ON k.personel_kadi = p.kullanici_adi
                WHERE u.mail = @mail";

                    MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@mail", kullaniciMail);

                    baglanti.Open();
                    using (MySqlDataReader dr = komut.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            numaralar.Add(dr["takipNo"].ToString());
                        }
                    }
                }
                return numaralar;
            }
            public DataTable TakipNoIleGetir(string takipNo)
            {
                using (MySqlConnection baglanti = new MySqlConnection(connString))
                {
                    string sorgu = @"SELECT 
                            k.TakipNo, 
                            k.gonderen, 
                            k.alici, 
                            k.adres, 
                            k.tarih, 
                            k.saat, 
                            k.islem, 
                            k.secilenSaat, 
                            p.ad, 
                            p.telefon 
                        FROM kargolar k 
                        LEFT JOIN personeller p ON k.personel_kadi = p.kullanici_adi 
                        WHERE k.TakipNo = @TakipNo";
                    using (MySqlDataAdapter da = new MySqlDataAdapter(sorgu, baglanti))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@TakipNo", takipNo);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt; 
                    }
                }
            }
        }
        public class KargoServisi
        {
            private KargoRepository _repo = new KargoRepository();

            public DataTable KargoSorgula(string takipNo, out string mesaj)
            {
              
                if (string.IsNullOrWhiteSpace(takipNo))
                {
                    mesaj = "Lütfen bir takip numarası giriniz.";
                    return null;
                }

                try
                {
                    DataTable sonuc = _repo.TakipNoIleGetir(takipNo);

                    if (sonuc.Rows.Count > 0)
                    {
                        mesaj = "Kargo bulundu.";
                        return sonuc;
                    }
                    else
                    {
                        mesaj = "Bu takip numarasına ait kargo bulunamadı.";
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    mesaj = "Sorgulama hatası: " + ex.Message;
                    return null;
                }
            }
        }

        private void btnSorgula_Click(object sender, EventArgs e)
        {
            KargoServisi servis = new KargoServisi();
            string mesaj;
            DataTable dt = servis.KargoSorgula(txtTakipNo.Text, out mesaj);

            if(dt != null && dt.Rows.Count > 0)
            {
                dgvKargolarUser.DataSource = dt;

                
                dgvKargolarUser.Columns["TakipNo"].HeaderText = "Takip No";
                dgvKargolarUser.Columns["gonderen"].HeaderText = "Gönderen";
                dgvKargolarUser.Columns["alici"].HeaderText = "Alıcı";
                dgvKargolarUser.Columns["adres"].HeaderText = "Adres";
                dgvKargolarUser.Columns["tarih"].HeaderText = "Tarih";
                dgvKargolarUser.Columns["saat"].HeaderText = "Saat";
                dgvKargolarUser.Columns["islem"].HeaderText = "İşlem Durumu";
                dgvKargolarUser.Columns["secilenSaat"].HeaderText = "Seçilen Saat";
                dgvKargolarUser.Columns["telefon"].HeaderText = "Kurye Numarası";
                dgvKargolarUser.Columns["ad"].HeaderText = "Kurye Adı";


                dgvKargolarUser.Columns["TakipNo"].Width = 90;
                dgvKargolarUser.Columns["gonderen"].Width = 90;
                dgvKargolarUser.Columns["alici"].Width = 90;
                dgvKargolarUser.Columns["adres"].Width = 90;
                dgvKargolarUser.Columns["tarih"].Width = 100;
                dgvKargolarUser.Columns["saat"].Width = 100;
                dgvKargolarUser.Columns["islem"].Width = 120;
                dgvKargolarUser.Columns["secilenSaat"].Width = 100;
                dgvKargolarUser.Columns["telefon"].Width = 120;
                dgvKargolarUser.Columns["ad"].Width = 100;
                btndetay.Visible = true;
                button1.Visible = true;
            }
            else
            {
                dgvKargolarUser.DataSource = null;
                btndetay.Visible = false;
                button1.Visible = false;
                MessageBox.Show("Hata: " + txtTakipNo.Text + " numaralı kargo bulunamadı!", "Sorgulama Sonucu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.ShowDialog();
        }
        private void txtTakipNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void UserForm_Load(object sender, EventArgs e)
        {
            KargoRepository repo = new KargoRepository();
            List<string> kargolar = repo.KullaniciKargolariniGetir(girisYapanKullaniciAd);

            listBox1.Items.Clear();
            foreach (string no in kargolar)
            {
                listBox1.Items.Add(no);
            }
            dgvKargolarUser.ColumnHeadersDefaultCellStyle.BackColor = Color.SlateGray;
            dgvKargolarUser.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKargolarUser.EnableHeadersVisualStyles = false;
        }
        private void btnGeriDon_Click(object sender, EventArgs e)
        {
            UserLogin frmGiris = new UserLogin();
            frmGiris.Show();
            this.Close();
        }

        private void dgvKargolarUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtTakipNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void kytdüzen_Click(object sender, EventArgs e)
        {
            KullaniciRepository repo = new KullaniciRepository();
            Kullanici mevcutKullanici = repo.KullaniciBilgileriniGetir(GlobalDegiskenler.GirisYapanMail);

            if (mevcutKullanici != null)
            {
                kayitOl frmKayit = new kayitOl();
                frmKayit.GuncellenecekKullanici = mevcutKullanici;
                frmKayit.Show();
                this.Hide();
            }
        }

        private void btndetay_Click(object sender, EventArgs e)
        {
            string takipNo = txtTakipNo.Text; 

            if (!string.IsNullOrEmpty(takipNo))
            {
                Detaylar detayForm = new Detaylar();
                detayForm.HareketleriYukle(takipNo);
                detayForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Lütfen bir takip numarası girin.");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";

            using (MySqlConnection baglanti = new MySqlConnection(connectionString))
            {
                try
                {
                    baglanti.Open();
                    string query = @"SELECT kd.dogrulama_kodu 
                             FROM kargo_dogrulama kd
                             INNER JOIN kullanicilar u ON kd.telefon = u.telefon
                             INNER JOIN kargolar k ON u.kullanici_adi = k.alici
                             WHERE k.TakipNo = @takip 
                             ORDER BY kd.id DESC LIMIT 1";

                    MySqlCommand cmd = new MySqlCommand(query, baglanti);
                    cmd.Parameters.AddWithValue("@takip", txtTakipNo.Text);

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string cekilenKod = result.ToString();

                        Smsler smsFormu = new Smsler();
                        smsFormu.GelenKod = cekilenKod;
                        smsFormu.GelenTakipNo = txtTakipNo.Text;
                        smsFormu.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Bu hesaba ait size gönderilmiş bir kod bulunamadı.", "Esferix");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı Hatası: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string takipNo = txtTakipNo.Text;

            string fotoAdi = VeritabanindanFotoIsmiAl(takipNo);

            if (!string.IsNullOrEmpty(fotoAdi))
            {
                string hedefKlasor = @"C:\EsferixKargo\TeslimatFotograflari";
                string tamYol = Path.Combine(hedefKlasor, fotoAdi);

                FotoGosterForm frm = new FotoGosterForm(tamYol);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Bu kargo için henüz bir teslimat fotoğrafı yüklenmemiş.");
            }
        }
        private string VeritabanindanFotoIsmiAl(string takipNo)
        {
            string fotoIsmi = "";
            string baglantiCumlesi = "Server=localhost;Database=kargo_takip;Uid=root;Pwd=;";

            using (MySqlConnection baglanti = new MySqlConnection(baglantiCumlesi))
            {
                string query = "SELECT teslimat_fotograf FROM kargolar WHERE TakipNo = @no";
                MySqlCommand cmd = new MySqlCommand(query, baglanti);
                cmd.Parameters.AddWithValue("@no", takipNo);

                baglanti.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    fotoIsmi = result.ToString();
                }
                baglanti.Close();
            }
            return fotoIsmi;
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            dgvKargolarUser.ClearSelection();
        }

        private void UserForm_Click(object sender, EventArgs e)
        {
            dgvKargolarUser.ClearSelection();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                txtTakipNo.Text = listBox1.SelectedItem.ToString();

                btnSorgula.PerformClick();
            }
        }
    }
}

