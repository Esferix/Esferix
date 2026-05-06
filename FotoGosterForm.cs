using System;
using System.IO;
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
    public partial class FotoGosterForm : Form
    {
        public FotoGosterForm(string fotoYolu)
        {
            InitializeComponent();

            if (File.Exists(fotoYolu))
            {
                pictureBox1.Image = Image.FromFile(fotoYolu);
            }
            else
            {
                MessageBox.Show("Fotoğraf dosyası bulunamadı.");
            }
        }

        private void FotoGosterForm_Load(object sender, EventArgs e)
        {

        }
    }
}
