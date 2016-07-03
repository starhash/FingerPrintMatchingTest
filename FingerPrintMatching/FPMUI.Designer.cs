namespace FingerPrintMatching
{
    partial class FPMUI
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
            this.image = new System.Windows.Forms.PictureBox();
            this.load = new System.Windows.Forms.Button();
            this.path = new System.Windows.Forms.TextBox();
            this.gmiBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
            this.SuspendLayout();
            // 
            // image
            // 
            this.image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.image.Location = new System.Drawing.Point(0, 0);
            this.image.Name = "image";
            this.image.Size = new System.Drawing.Size(757, 441);
            this.image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.image.TabIndex = 0;
            this.image.TabStop = false;
            // 
            // load
            // 
            this.load.Location = new System.Drawing.Point(12, 12);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(75, 23);
            this.load.TabIndex = 1;
            this.load.Text = "Load";
            this.load.UseVisualStyleBackColor = true;
            this.load.Click += new System.EventHandler(this.load_Click);
            // 
            // path
            // 
            this.path.Location = new System.Drawing.Point(93, 12);
            this.path.Name = "path";
            this.path.Size = new System.Drawing.Size(302, 20);
            this.path.TabIndex = 2;
            // 
            // gmiBtn
            // 
            this.gmiBtn.Location = new System.Drawing.Point(12, 41);
            this.gmiBtn.Name = "gmiBtn";
            this.gmiBtn.Size = new System.Drawing.Size(106, 23);
            this.gmiBtn.TabIndex = 14;
            this.gmiBtn.Text = "Get Modifier Image";
            this.gmiBtn.UseVisualStyleBackColor = true;
            this.gmiBtn.Click += new System.EventHandler(this.gmiBtn_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Analyze";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FPMUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 441);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gmiBtn);
            this.Controls.Add(this.path);
            this.Controls.Add(this.load);
            this.Controls.Add(this.image);
            this.Name = "FPMUI";
            this.Text = "FingerPrintMatcher";
            ((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox image;
        private System.Windows.Forms.Button load;
        private System.Windows.Forms.TextBox path;
        private System.Windows.Forms.Button gmiBtn;
        private System.Windows.Forms.Button button1;
    }
}

