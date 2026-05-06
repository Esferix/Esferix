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
    public partial class personelKayit : Form
    {
        string secilenDosyaYolu = "";
        public string eskiFotoAdi;
        public personelKayit()
        {
            InitializeComponent();
        }
        public bool guncellemeModu = false;
        public string eskiKullaniciAdi;
        private void personelKayit_Load(object sender, EventArgs e)
        {
            if (guncellemeModu)
            {
                txtMail.ReadOnly = true;
                txtSifre.ReadOnly = true;
                chkSifreGoster.Enabled = false;
                button1.Text = "Güncelle";
                this.Text = "Personel Güncelleme";
            }
            else
            {
                txtMail.ReadOnly = false;
                txtSifre.ReadOnly = false;
                button1.Text = "Kaydet";
            }
            if (guncellemeModu && !string.IsNullOrEmpty(eskiFotoAdi))
            {
                string anaKlasor = @"C:\EsferixKargo\PersonelFotograflari\";
                string tamYol = Path.Combine(anaKlasor, eskiFotoAdi);

                if (File.Exists(tamYol))
                {
                    using (var stream = new FileStream(tamYol, FileMode.Open, FileAccess.Read))
                    {
                        pictureBox2.Image = Image.FromStream(stream); 
                    }
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool hataVar = false;

            if (string.IsNullOrWhiteSpace(txtAd.Text))
            {
                errorProvider1.SetError(txtAd, "Ad alanı boş bırakılamaz!");
                hataVar = true;
            }

            if (string.IsNullOrWhiteSpace(txtSoyad.Text))
            {
                errorProvider1.SetError(txtSoyad, "Soyad alanı boş bırakılamaz!");
                hataVar = true;
            }

            if (string.IsNullOrWhiteSpace(txtKullaniciAdi.Text))
            {
                errorProvider1.SetError(txtKullaniciAdi, "Lütfen bir kullanıcı adı belirleyin!");
                hataVar = true;
            }

            if (string.IsNullOrWhiteSpace(txtKimlik.Text))
            {
                errorProvider1.SetError(txtKimlik, "Lütfen bir kimlik numarası girin!");
                hataVar = true;
            }

            string eposta = txtMail.Text.Trim();

            if (string.IsNullOrWhiteSpace(eposta) || !eposta.Contains("@") || !eposta.Contains("."))
            {
                errorProvider1.SetError(txtMail, "Geçerli bir e-posta adresi giriniz! (örnek@mail.com)!");
                hataVar = true;
            }

            string safNumaraKayit = msktxtTelefon.Text.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("_", "");

            if (!string.IsNullOrWhiteSpace(safNumaraKayit) && safNumaraKayit.Length < 10)
            {
                errorProvider1.SetError(msktxtTelefon, "Lütfen geçerli bir telefon numarası giriniz!");
                hataVar = true;
            }

            if (!guncellemeModu && txtSifre.Text.Length < 8)
            {
                errorProvider1.SetError(txtSifre, "Şifre en az 8 karakter olmalıdır!");
                hataVar = true;
            }

            if (hataVar) return;

            try
            {
                string ad = txtAd.Text;
                string soyad = txtSoyad.Text;
                string kullaniciAdi = txtKullaniciAdi.Text;
                string mail = txtMail.Text;
                string telefon = msktxtTelefon.Text;
                string kimlikNo = txtKimlik.Text;
                string adres = txtAdres.Text;
                string sifre = txtSifre.Text;

                string hedefKlasor = @"C:\EsferixKargo\PersonelFotograflari";
                string dosyaAdi = guncellemeModu ? eskiFotoAdi : "";

                if (!string.IsNullOrEmpty(secilenDosyaYolu))
                {
                    if (!Directory.Exists(hedefKlasor))
                        Directory.CreateDirectory(hedefKlasor);

                    dosyaAdi = kimlikNo + Path.GetExtension(secilenDosyaYolu);
                    string hedefYol = Path.Combine(hedefKlasor, dosyaAdi);

                    File.Copy(secilenDosyaYolu, hedefYol, true);
                }
                string sorgu = "";

                if (guncellemeModu)
                {
                    sorgu = "UPDATE Personeller SET ad=@ad, soyad=@soyad, kullanici_adi=@kadi, eposta=@mail, telefon=@tel, kimlik_no=@tc, adres=@adres, sifre=@sifre, fotograf_yolu=@foto WHERE kullanici_adi=@eskiKadi";
                }
                else
                {
                    sorgu = "INSERT INTO Personeller (ad, soyad, kullanici_adi, eposta, telefon, kimlik_no, adres, sifre, fotograf_yolu) VALUES (@ad, @soyad, @kadi, @mail, @tel, @tc, @adres, @sifre, @foto)";
                }

                string baglantiYolu = "Server=localhost;Database=kargo_takip;Uid=root;Pwd=;";
                using (MySqlConnection baglanti = new MySqlConnection(baglantiYolu))
                {
                    baglanti.Open();
                    using (MySqlCommand komut = new MySqlCommand(sorgu, baglanti))
                    {
                        komut.Parameters.AddWithValue("@ad", ad);
                        komut.Parameters.AddWithValue("@soyad", soyad);
                        komut.Parameters.AddWithValue("@kadi", kullaniciAdi);
                        komut.Parameters.AddWithValue("@mail", mail);
                        komut.Parameters.AddWithValue("@tel", telefon);
                        komut.Parameters.AddWithValue("@tc", kimlikNo);
                        komut.Parameters.AddWithValue("@adres", adres);
                        komut.Parameters.AddWithValue("@sifre", sifre);
                        komut.Parameters.AddWithValue("@foto", dosyaAdi);

                        if (guncellemeModu)
                        {
                            komut.Parameters.AddWithValue("@eskiKadi", eskiKullaniciAdi);
                        }

                        komut.ExecuteNonQuery();
                    }
                }
                string mesaj = guncellemeModu ? "Personel başarıyla güncellendi." : $"{ad} {soyad} başarıyla sisteme kaydedildi.";

                MessageBox.Show(mesaj, "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
                Temizle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Temizle()
        {
            txtAd.Clear();
            txtAdres.Clear();
            txtSoyad.Clear();
            txtKullaniciAdi.Clear();
            txtMail.Clear();
            msktxtTelefon.Clear();
            txtKimlik.Clear();
            txtSifre.Clear();
            pictureBox2.Image = null;
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                secilenDosyaYolu = ofd.FileName; 

                using (var stream = new FileStream(secilenDosyaYolu, FileMode.Open, FileAccess.Read))
                {
                    pictureBox2.Image = Image.FromStream(stream);
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox2.Image == null)
            {
                using (Font font = new Font("Arial", 40, FontStyle.Bold))
                {
                    string text = "+";

                    Size textSize = TextRenderer.MeasureText(text, font);

                    int x = ((pictureBox2.Width - textSize.Width) / 2)+7;
                    int y = (pictureBox2.Height - textSize.Height) / 2;

                    e.Graphics.DrawString(text, font, Brushes.Gray, x, y);
                }
            }
        }

        private void txtAd_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAd.Text))
            {
                errorProvider1.SetError(txtAd, "");
            }
        }

        private void txtSoyad_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSoyad.Text))
            {
                errorProvider1.SetError(txtSoyad, "");
            }
        }

        private void txtMail_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMail.Text) && txtMail.Text.Contains("@") && txtMail.Text.Contains("."))
            {
                errorProvider1.SetError(txtMail, "");
            }
        }

        private void txtKimlik_TextChanged(object sender, EventArgs e)
        {
            if (txtKimlik.Text.Length >= 11)
            {
                errorProvider1.SetError(txtKimlik, "");
            }
        }

        private void txtKullaniciAdi_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtKullaniciAdi.Text))
            {
                errorProvider1.SetError(txtKullaniciAdi, "");
            }
        }

        private void txtSifre_TextChanged(object sender, EventArgs e)
        {
            if (txtSifre.Text.Length >= 8)
            {
                errorProvider1.SetError(txtSifre, "");
            }
        }

        private void msktxtTelefon_TextChanged(object sender, EventArgs e)
        {
            string safNumara = msktxtTelefon.Text.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("_", "");

            if (safNumara.Length >= 10)
            {
                errorProvider1.SetError(msktxtTelefon, "");
            }
        }

        private void txtKimlik_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }
    }
}
