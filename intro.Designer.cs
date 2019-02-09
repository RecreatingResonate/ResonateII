namespace ResonateII
{
    partial class intro
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tickerpanel = new System.Windows.Forms.Panel();
            this.msg = new System.Windows.Forms.Label();
            this.logo = new System.Windows.Forms.PictureBox();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.tick = new System.Windows.Forms.Timer(this.components);
            this.tickerpanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // tickerpanel
            // 
            this.tickerpanel.Controls.Add(this.msg);
            this.tickerpanel.Location = new System.Drawing.Point(13, 213);
            this.tickerpanel.Name = "tickerpanel";
            this.tickerpanel.Size = new System.Drawing.Size(200, 45);
            this.tickerpanel.TabIndex = 2;
            // 
            // msg
            // 
            this.msg.AutoSize = true;
            this.msg.Font = new System.Drawing.Font("Lucida Console", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msg.ForeColor = System.Drawing.Color.White;
            this.msg.Location = new System.Drawing.Point(18, 16);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(336, 29);
            this.msg.TabIndex = 2;
            this.msg.Text = "the ticker is empty";
            // 
            // logo
            // 
            //this.logo.Image = global::ResonateII.Properties.Resources.logo;
            this.logo.Location = new System.Drawing.Point(13, 13);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(578, 168);
            this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logo.TabIndex = 1;
            this.logo.TabStop = false;
            // 
            // canvas
            // 
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(932, 270);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            // 
            // tick
            // 
            this.tick.Interval = 20;
            this.tick.Tick += new System.EventHandler(this.tick_Tick);
            // 
            // intro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 270);
            this.ControlBox = false;
            this.Controls.Add(this.tickerpanel);
            this.Controls.Add(this.logo);
            this.Controls.Add(this.canvas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "intro";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "intro";
            this.TopMost = true;
            this.tickerpanel.ResumeLayout(false);
            this.tickerpanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Panel tickerpanel;
        private System.Windows.Forms.Label msg;
        private System.Windows.Forms.Timer tick;
    }
}