using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cargo_tracker.arayuz
{
    public partial class pSifre : Form
    {
        public pSifre()
        {
            InitializeComponent();
        }

        private void btnKodGonder_Click(object sender, EventArgs e)
        {
            try
            {
                string kod = new Random().Next(100000, 999999).ToString();
                if (KargoData.KodKaydet1(txtEmail.Text, kod))
                {
                    if (EmailServisi.EmailGonder1(txtEmail.Text, kod))
                    {
                        MessageBox.Show("Doğrulama kodu e-posta adresine gönderildi!");
                        txtYeniSifre.Visible = true;
                        pictureBox2.Visible = true;
                        panel7.Visible = true;
                        panel8.Visible = true;
                        label1.Visible = true;
                        txtKod.Visible = true;
                        label3.Visible = true;
                        btndegistir.Visible = true;
                        btnKodGonder.Visible = false;

                    }
                    else
                    {
                        MessageBox.Show("E-posta gönderilemedi! SMTP ayarlarını kontrol edin.");
                    }
                }
                else
                {
                    MessageBox.Show("E-posta bulunamadı veya kod kaydedilemedi!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sistemsel Hata: " + ex.Message);
            }
        }

        private void btndegistir_Click(object sender, EventArgs e)
        {
            string mail = txtEmail.Text.Trim();
            string yeniSifre = txtYeniSifre.Text.Trim();
            string girilenKod = txtKod.Text.Trim();

            if (string.IsNullOrEmpty(mail) || string.IsNullOrEmpty(yeniSifre) || string.IsNullOrEmpty(girilenKod))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!");
                return;
            }

            if (yeniSifre.Length < 8)
            {
                MessageBox.Show("Yeni şifre en az 8 karakter uzunluğunda olmalıdır!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string eskiSifre = KargoData.MevcutSifreGetir1(mail);

            if (yeniSifre == eskiSifre)
            {
                MessageBox.Show("Yeni şifreniz eski şifrenizle aynı olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (KargoData.KodDogrula1(mail, girilenKod))
            {
                if (KargoData.SifreGuncelle1(mail, yeniSifre))
                {
                    MessageBox.Show("Şifreniz başarıyla güncellendi! Giriş yapabilirsiniz.");
                    UserLogin loginFormu = new UserLogin();
                    loginFormu.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Şifre güncellenirken bir hata oluştu.");
                }
            }
            else
            {
                MessageBox.Show("Girdiğiniz onay kodu hatalı. Lütfen kontrol edin.");
            }
        }

        private void btngeri_Click(object sender, EventArgs e)
        {
            Personel login = new Personel();
            login.Show();
            this.Close();
        }

        private void pSifre_Load(object sender, EventArgs e)
        {
            txtYeniSifre.Visible = false;
            label1.Visible = false;
            txtKod.Visible = false;
            label3.Visible = false;
            btndegistir.Visible = false;
        }
    }
}