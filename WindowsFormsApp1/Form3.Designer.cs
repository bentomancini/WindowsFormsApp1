
namespace WindowsFormsApp1
{
    partial class Form3
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
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.btnMusicas = new Guna.UI2.WinForms.Guna2Button();
            this.btnArtistas = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.fundoofcc;
            this.guna2Panel1.Controls.Add(this.btnArtistas);
            this.guna2Panel1.Controls.Add(this.btnMusicas);
            this.guna2Panel1.Controls.Add(this.guna2CirclePictureBox1);
            this.guna2Panel1.Location = new System.Drawing.Point(12, 2);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(207, 450);
            this.guna2Panel1.TabIndex = 0;
            // 
            // guna2CirclePictureBox1
            // 
            this.guna2CirclePictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.guna2CirclePictureBox1.ErrorImage = null;
            this.guna2CirclePictureBox1.FillColor = System.Drawing.Color.Transparent;
            this.guna2CirclePictureBox1.Image = global::WindowsFormsApp1.Properties.Resources.tecfylogo_removebg_preview;
            this.guna2CirclePictureBox1.ImageRotate = 0F;
            this.guna2CirclePictureBox1.Location = new System.Drawing.Point(14, 0);
            this.guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            this.guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.guna2CirclePictureBox1.Size = new System.Drawing.Size(160, 111);
            this.guna2CirclePictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.guna2CirclePictureBox1.TabIndex = 13;
            this.guna2CirclePictureBox1.TabStop = false;
            // 
            // btnMusicas
            // 
            this.btnMusicas.BackColor = System.Drawing.Color.Transparent;
            this.btnMusicas.BorderColor = System.Drawing.Color.BlueViolet;
            this.btnMusicas.BorderRadius = 14;
            this.btnMusicas.BorderThickness = 1;
            this.btnMusicas.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnMusicas.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnMusicas.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnMusicas.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnMusicas.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(7)))), ((int)(((byte)(20)))));
            this.btnMusicas.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMusicas.ForeColor = System.Drawing.Color.White;
            this.btnMusicas.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(58)))), ((int)(((byte)(237)))));
            this.btnMusicas.Location = new System.Drawing.Point(46, 132);
            this.btnMusicas.Name = "btnMusicas";
            this.btnMusicas.Size = new System.Drawing.Size(105, 24);
            this.btnMusicas.TabIndex = 19;
            this.btnMusicas.Text = "Músicas";
            // 
            // btnArtistas
            // 
            this.btnArtistas.BackColor = System.Drawing.Color.Transparent;
            this.btnArtistas.BorderColor = System.Drawing.Color.BlueViolet;
            this.btnArtistas.BorderRadius = 14;
            this.btnArtistas.BorderThickness = 1;
            this.btnArtistas.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnArtistas.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnArtistas.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnArtistas.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnArtistas.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(7)))), ((int)(((byte)(20)))));
            this.btnArtistas.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnArtistas.ForeColor = System.Drawing.Color.White;
            this.btnArtistas.Location = new System.Drawing.Point(46, 175);
            this.btnArtistas.Name = "btnArtistas";
            this.btnArtistas.Size = new System.Drawing.Size(99, 24);
            this.btnArtistas.TabIndex = 22;
            this.btnArtistas.Text = "Artistas";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.fundoofcc;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.guna2Panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.guna2Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.guna2CirclePictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private Guna.UI2.WinForms.Guna2Button btnMusicas;
        private Guna.UI2.WinForms.Guna2Button btnArtistas;
    }
}