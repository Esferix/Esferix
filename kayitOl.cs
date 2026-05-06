using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static cargo_tracker.UserLogin;

namespace cargo_tracker.arayuz
{
    public partial class kayitOl : Form
    {
        public kayitOl()
        {
            InitializeComponent();
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }
        public Kullanici GuncellenecekKullanici { get; set; }

        private void kayitOl_Load(object sender, EventArgs e)
        {
            if (GuncellenecekKullanici != null)
            {
                this.Text = "Bilgileri Güncelle";
                button1.Text = "Güncelle";

                txtAd.Text = GuncellenecekKullanici.Ad;
                txtSoyad.Text = GuncellenecekKullanici.Soyad;
                txtKullaniciAdi.Text = GuncellenecekKullanici.KullaniciAdi;
                txtMail.Text = GuncellenecekKullanici.Mail;
                msktxtTelefon.Text = GuncellenecekKullanici.Telefon;
                txtSifre.Text = GuncellenecekKullanici.Sifre;
                txtIl.Text = GuncellenecekKullanici.Il;
                txtIlce.Text = GuncellenecekKullanici.Ilce;
                txtMahalle.Text = GuncellenecekKullanici.Mahalle;
                txtAdres.Text = GuncellenecekKullanici.Adres;

                txtMail.ReadOnly = true;
                txtSifre.ReadOnly = true;
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

            if (string.IsNullOrEmpty(txtSoyad.Text))
            {
                errorProvider1.SetError(txtSoyad, "Soyad alanı boş bırakılamaz!");
                hataVar = true;
            }

            if (string.IsNullOrWhiteSpace(txtKullaniciAdi.Text))
            {
                errorProvider1.SetError(txtKullaniciAdi, "Kullanıcı adı boş bırakılamaz!");
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

            if (txtSifre.Text.Length < 8)
            {
                errorProvider1.SetError(txtSifre, "Şifreniz en az 8 karakter olmalıdır!");
                hataVar = true;
            }

            if (hataVar) return;

            Kullanici kullaniciBilgileri = new Kullanici
            {
                Ad = txtAd.Text,
                Soyad = txtSoyad.Text,
                KullaniciAdi = txtKullaniciAdi.Text,
                Mail = txtMail.Text,
                Telefon = msktxtTelefon.Text,
                Sifre = txtSifre.Text,
                Il = txtIl.Text,
                Ilce = txtIlce.Text,
                Mahalle = txtMahalle.Text,
                Adres = txtAdres.Text
            };

            KullaniciServisi servis = new KullaniciServisi();

            if (GuncellenecekKullanici != null)
            {
                KullaniciRepository repo = new KullaniciRepository();
                bool guncellendiMi = repo.KullaniciGuncelle(
                    kullaniciBilgileri.Ad, kullaniciBilgileri.Soyad, kullaniciBilgileri.KullaniciAdi,
                    kullaniciBilgileri.Mail, kullaniciBilgileri.Telefon, kullaniciBilgileri.Sifre,
                    kullaniciBilgileri.Il, kullaniciBilgileri.Ilce, kullaniciBilgileri.Mahalle, kullaniciBilgileri.Adres
                );

                if (guncellendiMi)
                {
                    MessageBox.Show("Bilgileriniz başarıyla güncellendi.");
                    UserForm frmUser = new UserForm(txtMail.Text);
                    frmUser.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Güncelleme sırasında bir hata oluştu.");
                }
            }
            else
            {
                string sonuc = servis.KayitOl(kullaniciBilgileri);

                if (sonuc == "Başarılı")
                {
                    MessageBox.Show("Kaydınız başarıyla oluşturuldu.");
                    UserLogin frmGiris = new UserLogin();
                    frmGiris.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(sonuc);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (GuncellenecekKullanici != null)
            {
                UserForm frmUser = new UserForm(txtMail.Text);
                frmUser.Show();
                this.Close();
            }
            else
            {
                UserLogin frmGiris = new UserLogin();
                frmGiris.Show();
                this.Close();
            }
        }

        private void chkSifreGoster_CheckedChanged(object sender, EventArgs e)
        {
            txtSifre.UseSystemPasswordChar = !chkSifreGoster.Checked;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void kayitOl_Load_1(object sender, EventArgs e)
        {
            kayitOl_Load(sender, e);
        }

        private void txtMail_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMail.Text) && txtMail.Text.Contains("@") && txtMail.Text.Contains("."))
            {
                errorProvider1.SetError(txtMail, "");
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
    }
}
