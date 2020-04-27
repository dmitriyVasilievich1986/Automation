namespace Automation
{
    partial class TransmitionArray
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
            this.condition_box = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // condition_box
            // 
            this.condition_box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.condition_box.Location = new System.Drawing.Point(0, 0);
            this.condition_box.Multiline = true;
            this.condition_box.Name = "condition_box";
            this.condition_box.Size = new System.Drawing.Size(800, 450);
            this.condition_box.TabIndex = 0;
            // 
            // TransmitionArray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.condition_box);
            this.Name = "TransmitionArray";
            this.Text = "TransmitionArray";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox condition_box;
    }
}