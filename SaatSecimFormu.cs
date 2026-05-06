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
    public partial class SaatSecimFormu : Form
    {
        public string AktarilanTakipNo { get; set; }
        public SaatSecimFormu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir saat dilimi seçiniz!");
                return;
            }

            string secilenSaat = comboBox1.SelectedItem.ToString();
            string connectionString = "server=localhost;database=kargo_takip;uid=root;pwd=;";

            using (MySqlConnection baglanti = new MySqlConnection(connectionString))
            {
                try
                {
                    baglanti.Open();
                    string query = "UPDATE kargolar SET secilenSaat = @saat WHERE TakipNo = @no";

                    using (MySqlCommand cmd = new MySqlCommand(query, baglanti))
                    {
                        cmd.Parameters.AddWithValue("@saat", secilenSaat);
                        cmd.Parameters.AddWithValue("@no", AktarilanTakipNo);

                        int sonuc = cmd.ExecuteNonQuery();

                        if (sonuc > 0)
                        {
                            MessageBox.Show($"Kargonuz için {secilenSaat} aralığı başarıyla kaydedildi!", "Esferix", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Kargo bulunamadı veya güncellenemedi.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }
        }
    }
}
