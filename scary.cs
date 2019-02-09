using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace ResonateII
{
    public partial class scary : Form
    {
        public scary()
        {
            InitializeComponent();
        }

        private void scary_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\sweetdreams.jpg");
            new SoundPlayer(Environment.GetEnvironmentVariable("windir") + "\\system\\d\\p.wav").PlayLooping();
        }
    }
}
