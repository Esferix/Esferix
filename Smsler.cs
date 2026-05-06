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
    public partial class Smsler : Form
    {
        public string GelenKod { get; set; }
        public string GelenTakipNo { get; set; }
        public Smsler()
        {
            InitializeComponent();
        }
        private void smsler_Load(object sender, EventArgs e)
        {
            textBox2.Text = GelenKod;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string girilenKod = textBox1.Text.Trim();
            string beklenenKod = textBox2.Text.Trim();

            if (girilenKod == beklenenKod && !string.IsNullOrEmpty(girilenKod))
            {
                string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string nullQuery = "UPDATE kargo_dogrulama SET dogrulama_kodu = NULL WHERE dogrulama_kodu = @kod";
                        using (MySqlCommand cmd = new MySqlCommand(nullQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@kod", beklenenKod);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch {}
                }

                MessageBox.Show("Kod Doğrulandı! Saat seçim sayfasına yönlendiriliyorsunuz.");

                SaatSecimFormu saatForm = new SaatSecimFormu();
                saatForm.AktarilanTakipNo = this.GelenTakipNo;
                saatForm.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Hatalı kod girdiniz, lütfen tekrar deneyin.");
            }
        }
    }
}
