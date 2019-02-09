using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResonateII
{
    public partial class danb : Form
    {
        public static string keyBuff = "";

        public danb()
        {
            InitializeComponent();
            int mX = label1.Width;
            int mY = label1.Height;
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;
            label1.Location = new Point((Width / 2) - (mX / 2), (Height / 2) - (mY / 2));
        }

        private void danb_KeyPress(object sender, KeyPressEventArgs e)
        {
            keyBuff = keyBuff + e.KeyChar;
            if (keyBuff.Contains("Happy Birthday Dan")) Close();
        }
    }
}
