
using cargo_tracker.arayuz;
using cargo_tracker.destek;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cargo_tracker
{
    public partial class UserLogin : Form
    {
        public UserLogin()
        {
            InitializeComponent();
        }

        private void UserLogin_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            AnaForm frmAna = new AnaForm();
            frmAna.Show();
            this.Close();
        }
        public class Kullanici
        {
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string KullaniciAdi { get; set; }
            public string Mail { get; set; }
            public string Telefon { get; set; }
            public string Sifre { get; set; }
            public string Il { get; set; }
            public string Ilce { get; set; }
            public string Mahalle { get; set; }
            public string Adres { get; set; }
        }
        public class KullaniciGirisModel
        {
            public string Mail { get; set; }
            public string Sifre { get; set; }
        }

        public class KullaniciRepository
        {
            private string connectionString = "Server=localhost;Database=kargo_takip;Uid=root;Pwd=;";

            public bool KullaniciVarMi(string mail)
            {
                using (MySqlConnection baglanti = new MySqlConnection(connectionString))
                {
                    string sorgu = "SELECT COUNT(*) FROM kullanicilar WHERE mail=@mail";
                    MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@mail", mail);
                    baglanti.Open();
                    return Convert.ToInt32(komut.ExecuteScalar()) > 0;
                }
            }
            public bool GirisBilgileriDogruMu(string mail, string sifre)
            {
                
                using (MySqlConnection baglanti = new MySqlConnection(connectionString))
                {
                    
                    string sorgu = "SELECT COUNT(1) FROM kullanicilar WHERE mail=@mail AND Sifre=@sifre";
                    MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@mail", mail);
                    komut.Parameters.AddWithValue("@sifre", sifre);

                    baglanti.Open();
                    int sonuc = Convert.ToInt32(komut.ExecuteScalar());
                    return sonuc > 0; 
                }
            }
            public bool KullaniciGuncelle(string ad, string soyad, string kadi, string mail, string tel, string sifre, string il, string ilce, string mah, string adres)
            {
                using (MySqlConnection baglanti = new MySqlConnection(connectionString))
                {
                    string sorgu = "UPDATE kullanicilar SET Ad=@ad, Soyad=@soyad, Kullanici_Adi=@kadi, Telefon=@tel, Sifre=@sifre, IL=@il, Ilce=@ilce, Mahalle=@mah, Adres=@adres WHERE Mail=@mail";
                    MySqlCommand komut = new MySqlCommand(sorgu, baglanti);

                    komut.Parameters.AddWithValue("@ad", ad);
                    komut.Parameters.AddWithValue("@soyad", soyad);
                    komut.Parameters.AddWithValue("@kadi", kadi);
                    komut.Parameters.AddWithValue("@tel", tel);
                    komut.Parameters.AddWithValue("@sifre", sifre);
                    komut.Parameters.AddWithValue("@il", il);
                    komut.Parameters.AddWithValue("@ilce", ilce);
                    komut.Parameters.AddWithValue("@mah", mah);
                    komut.Parameters.AddWithValue("@adres", adres);
                    komut.Parameters.AddWithValue("@mail", mail);

                    baglanti.Open();
                    return komut.ExecuteNonQuery() > 0;
                }
            }
            public Kullanici KullaniciBilgileriniGetir(string mail)
            {
                using (MySqlConnection baglanti = new MySqlConnection(connectionString))
                {
                    string sorgu = "SELECT * FROM kullanicilar WHERE mail=@mail";
                    MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@mail", mail);
                    baglanti.Open();

                    using (MySqlDataReader reader = komut.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Kullanici
                            {
                                Ad = reader["ad"].ToString(),
                                Soyad = reader["soyad"].ToString(),
                                KullaniciAdi = reader["kullanici_adi"].ToString(),
                                Mail = reader["mail"].ToString(),
                                Telefon = reader["telefon"].ToString(),
                                Sifre = reader["sifre"].ToString(),
                                Il = reader["il"].ToString(),
                                Ilce = reader["ilce"].ToString(),
                                Mahalle = reader["mahalle"].ToString(),
                                Adres = reader["adres"].ToString()
                            };
                        }
                    }
                }
                return null;
            }


            public void Ekle(Kullanici kullanici)
            {
                using (MySqlConnection baglanti = new MySqlConnection(connectionString))
                {
                    string sorgu = "INSERT INTO kullanicilar (mail, Sifre) VALUES (@mail, @sifre)";
                    MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@mail", kullanici.Mail);
                    komut.Parameters.AddWithValue("@sifre", kullanici.Sifre);
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                }
            }
        }
        public class KullaniciServisi
        {
            private KullaniciRepository _repo = new KullaniciRepository();

            public string KayitOl(Kullanici yeniKullanici)
            {
                if (string.IsNullOrWhiteSpace(yeniKullanici.Ad) || string.IsNullOrWhiteSpace(yeniKullanici.Soyad))
                    return "Ad ve Soyad alanları boş bırakılamaz.";

                if (string.IsNullOrWhiteSpace(yeniKullanici.KullaniciAdi))
                    return "Lütfen bir kullanıcı adı belirleyiniz.";

                if (string.IsNullOrWhiteSpace(yeniKullanici.Mail) || !yeniKullanici.Mail.Contains("@"))
                    return "Geçerli bir mail adresi giriniz.";

                if (string.IsNullOrWhiteSpace(yeniKullanici.Sifre) || yeniKullanici.Sifre.Length < 8)
                    return "Şifreniz en az 8 karakterden oluşmalıdır.";

                if (_repo.KullaniciVarMi(yeniKullanici.Mail))
                    return "Bu E-mail zaten kayıtlı.";

                try
                {
                    string connStr = "Server=localhost;Database=kargo_takip;Uid=root;Pwd=;";
                    using (MySqlConnection conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        string sql = @"INSERT INTO kullanicilar 
                         (ad, soyad, kullanici_adi, mail, telefon, sifre, il, ilce, mahalle, adres) 
                         VALUES 
                         (@ad, @soyad, @kadi, @mail, @tel, @sifre, @il, @ilce, @mah, @adr)";

                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@ad", yeniKullanici.Ad);
                            cmd.Parameters.AddWithValue("@soyad", yeniKullanici.Soyad);
                            cmd.Parameters.AddWithValue("@kadi", yeniKullanici.KullaniciAdi);
                            cmd.Parameters.AddWithValue("@mail", yeniKullanici.Mail);
                            cmd.Parameters.AddWithValue("@tel", yeniKullanici.Telefon);
                            cmd.Parameters.AddWithValue("@sifre", yeniKullanici.Sifre);
                            cmd.Parameters.AddWithValue("@il", yeniKullanici.Il);
                            cmd.Parameters.AddWithValue("@ilce", yeniKullanici.Ilce);
                            cmd.Parameters.AddWithValue("@mah", yeniKullanici.Mahalle);
                            cmd.Parameters.AddWithValue("@adr", yeniKullanici.Adres);

                            cmd.ExecuteNonQuery();
                            return "Başarılı";
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                    {
                        return "Bu kullanıcı adı zaten alınmış. Lütfen farklı bir kullanıcı adı deneyin.";
                    }
                    return "Hata oluştu: " + ex.Message;
                }
            }

        }
        public class Servis
        { 
            private readonly KullaniciRepository _repo = new KullaniciRepository();

            public (bool Basarili, string Mesaj) GirisYap(KullaniciGirisModel model)
            {
               
                if (string.IsNullOrWhiteSpace(model.Mail) || string.IsNullOrWhiteSpace(model.Sifre))
                {
                    return (false, "E-posta veya şifre boş bırakılamaz.");
                }

                try
                {
                    bool basariliMi = _repo.GirisBilgileriDogruMu(model.Mail, model.Sifre);

                    if (basariliMi)
                        return (true, "Giriş Başarılı! Hoş geldiniz.");
                    else
                        return (false, "E-posta veya şifre hatalı!");
                }
                catch (Exception ex)
                {
                    return (false, $"Giriş sırasında teknik bir hata oluştu: {ex.Message}");
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            kayitOl frmKayit = new kayitOl();
            frmKayit.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            KullaniciRepository repo = new KullaniciRepository();

            if (repo.GirisBilgileriDogruMu(txtMail.Text, txtSifre.Text))
            {
                GlobalDegiskenler.GirisYapanMail = txtMail.Text;

            }
            var servis = new Servis();
            var girisVerisi = new KullaniciGirisModel
            {
                Mail = txtMail.Text,
                Sifre = txtSifre.Text
            };

            
            var sonuc = servis.GirisYap(girisVerisi);

            MessageBox.Show(sonuc.Mesaj);

            if (sonuc.Basarili)
            {
                UserForm kullaniciSayfasi = new UserForm(txtMail.Text);
                kullaniciSayfasi.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("E-posta veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnunuttum_Click(object sender, EventArgs e)
        {

            Sifre yenileForm = new Sifre();
            yenileForm.Show();
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == "AnaForm") 
                {
                    frm.Hide();
                }
            }
            this.Close();
        }
    }
    }


