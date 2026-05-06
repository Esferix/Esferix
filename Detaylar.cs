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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace cargo_tracker.arayuz
{
    public partial class Detaylar : Form
    {
        public Detaylar()
        {
            InitializeComponent();
        }
        public string SeciliTakipNo { get; set; }

        private void Detaylar_Load_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SeciliTakipNo))
            {
                textBox2.Text = SeciliTakipNo;
            }
        }
        public void HareketleriYukle(string takipNo)
        {
            flowLayoutPanel1.Controls.Clear();
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM KargoHareketleri WHERE TakipNo = @no ORDER BY islem_tarihi DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@no", takipNo);

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            int sayac = 0; 
                            while (dr.Read())
                            {
                                KargoAdim adim = new KargoAdim();
                                adim.DurumBasligi = dr["durum_basligi"].ToString();
                                adim.Tarih = dr["islem_tarihi"].ToString();

                                string il = dr["konum_il"].ToString();
                                string ilce = dr["konum_ilce"].ToString();
                                adim.Konum = il + ", " + ilce;

                                if (sayac == 0)
                                {
                                    adim.AktifYap(true);
                                }
                                else
                                {
                                    adim.AktifYap(false);
                                }

                                flowLayoutPanel1.Controls.Add(adim);
                                sayac++;
                            }
                        }
                    }
                }
                if (flowLayoutPanel1.Controls.Count > 0)
                {
                    var sonAdim = (KargoAdim)flowLayoutPanel1.Controls[flowLayoutPanel1.Controls.Count - 1];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Liste yüklenirken hata: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string takipNo = textBox2.Text;
            string yeniDurum = comboBox2.Text;
            string konum = comboBox3.Text;
            string ilce = textBox1.Text;
            string tarih = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (string.IsNullOrEmpty(yeniDurum) || string.IsNullOrEmpty(konum))
            {
                MessageBox.Show("Lütfen ilgili alanları doldurun!");
                return;
            }

            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO KargoHareketleri (TakipNo, durum_basligi, islem_tarihi, konum_il, konum_ilce) " +
                                   "VALUES (@no, @durum, @tarih, @il, @ilce)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@no", textBox2.Text);
                        cmd.Parameters.AddWithValue("@durum", yeniDurum);
                        cmd.Parameters.AddWithValue("@tarih", tarih);
                        cmd.Parameters.AddWithValue("@il", comboBox3.Text);
                        cmd.Parameters.AddWithValue("@ilce", textBox1.Text);

                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Kargo durumu başarıyla güncellendi!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

