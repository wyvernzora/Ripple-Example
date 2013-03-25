namespace RippleExample
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ripplePictureBox1 = new RippleExample.WinForms.Controls.RipplePictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ripplePictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ripplePictureBox1
            // 
            this.ripplePictureBox1.AnimationEnabled = true;
            this.ripplePictureBox1.ClickSplashRadius = 15;
            this.ripplePictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ripplePictureBox1.DragSplashRadius = 7;
            this.ripplePictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("ripplePictureBox1.Image")));
            this.ripplePictureBox1.Location = new System.Drawing.Point(0, 0);
            this.ripplePictureBox1.MinimumSize = new System.Drawing.Size(256, 256);
            this.ripplePictureBox1.Name = "ripplePictureBox1";
            this.ripplePictureBox1.Size = new System.Drawing.Size(500, 500);
            this.ripplePictureBox1.TabIndex = 0;
            this.ripplePictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Controls.Add(this.ripplePictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ripplePictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private WinForms.Controls.RipplePictureBox ripplePictureBox1;

    }
}

