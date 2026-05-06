using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace cargo_tracker
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            if (txtKullanici.Text == "admin" && txtSifre.Text == "1234")
            {
                if (Program.AdminAcik)
                {
                    MessageBox.Show("Admin paneli zaten açık!");
                    return;
                }

                Program.AdminAcik = true;

                AdminForm admin = new AdminForm();
                admin.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Hatalı giriş");
            }
        }

        

        private void btnGeriDon_Click(object sender, EventArgs e)
        {
            AnaForm frmAna = new AnaForm();
            frmAna.Show();
            this.Close();
        }
    }
}
