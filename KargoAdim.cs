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
    public partial class KargoAdim : UserControl
    {
        public KargoAdim()
        {
            InitializeComponent();
        }
        public string DurumBasligi { get => label1.Text; set => label1.Text = value; }
        public string Tarih { get => label3.Text; set => label3.Text = value; }
        public string Konum { get => label2.Text; set => label2.Text = value; }
        public void AktifYap(bool aktif = false)
        {
            if (aktif)
            {
                label1.ForeColor = Color.Green;
                label1.Font = new Font(label1.Font, FontStyle.Regular);
                pictureBox1.BackColor = Color.Transparent;
                pictureBox1.Image = Properties.Resources.yesil_tik;
            }
            else
            {
                label1.ForeColor = Color.Gray;
                label1.Font = new Font(label1.Font, FontStyle.Regular);
                pictureBox1.BackColor = Color.Transparent;
                pictureBox1.Image = Properties.Resources.siyah_tik;
            }
        }
    }
}
