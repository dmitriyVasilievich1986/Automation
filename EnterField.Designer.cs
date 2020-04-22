namespace Automation
{
    partial class EnterField
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
            this.enter_text_field = new System.Windows.Forms.TextBox();
            this.input_cb = new System.Windows.Forms.ComboBox();
            this.input_tb = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // enter_text_field
            // 
            this.enter_text_field.Dock = System.Windows.Forms.DockStyle.Fill;
            this.enter_text_field.Enabled = false;
            this.enter_text_field.Location = new System.Drawing.Point(0, 0);
            this.enter_text_field.Multiline = true;
            this.enter_text_field.Name = "enter_text_field";
            this.enter_text_field.ReadOnly = true;
            this.enter_text_field.Size = new System.Drawing.Size(344, 210);
            this.enter_text_field.TabIndex = 0;
            // 
            // input_cb
            // 
            this.input_cb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.input_cb.FormattingEnabled = true;
            this.input_cb.Location = new System.Drawing.Point(0, 189);
            this.input_cb.Name = "input_cb";
            this.input_cb.Size = new System.Drawing.Size(344, 21);
            this.input_cb.Sorted = true;
            this.input_cb.TabIndex = 1;
            // 
            // input_tb
            // 
            this.input_tb.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.input_tb.Location = new System.Drawing.Point(0, 169);
            this.input_tb.Name = "input_tb";
            this.input_tb.Size = new System.Drawing.Size(344, 20);
            this.input_tb.TabIndex = 2;
            // 
            // EnterField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 210);
            this.Controls.Add(this.input_tb);
            this.Controls.Add(this.input_cb);
            this.Controls.Add(this.enter_text_field);
            this.KeyPreview = true;
            this.Name = "EnterField";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox enter_text_field;
        public System.Windows.Forms.ComboBox input_cb;
        public System.Windows.Forms.TextBox input_tb;
    }
}