namespace Launcher.PointBlank
{
    partial class Init
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
            this.INIT_TEXT = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // INIT_TEXT
            // 
            this.INIT_TEXT.Location = new System.Drawing.Point(-4, 9);
            this.INIT_TEXT.Name = "INIT_TEXT";
            this.INIT_TEXT.Size = new System.Drawing.Size(274, 23);
            this.INIT_TEXT.TabIndex = 0;
            this.INIT_TEXT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Init
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 41);
            this.ControlBox = false;
            this.Controls.Add(this.INIT_TEXT);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Init";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label INIT_TEXT;
    }
}