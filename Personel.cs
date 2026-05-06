using MySql.Data.MySqlClient;
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
    public partial class Personel : Form
    {
        public Personel()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string eposta = txtMail.Text; 
            string sifre = txtSifre.Text;

            try
            {
                using (MySqlConnection baglanti = new MySqlConnection("Server=localhost;Database=kargo_takip;Uid=root;Pwd=;"))
                {
                    baglanti.Open();
                    string sorgu = "SELECT * FROM Personeller WHERE eposta = @mail AND sifre = @sifre";

                    MySqlCommand komut = new MySqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@mail", eposta);
                    komut.Parameters.AddWithValue("@sifre", sifre);

                    MySqlDataReader oku = komut.ExecuteReader();

                    if (oku.Read()) 
                    {
                        PersonelForm panel = new PersonelForm();
                        panel.girisYapanKullanici = oku["kullanici_adi"].ToString();
                        this.Hide();
                        panel.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("E-posta veya şifre hatalı!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AnaForm ana = new AnaForm();
            ana.Show();
            this.Close();
        }

        private void btnunuttum_Click(object sender, EventArgs e)
        {
            pSifre sif = new pSifre();
            sif.Show();
            this.Close();
        }
    }
}
