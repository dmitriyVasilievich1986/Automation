namespace Automation
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btn_close_application = new System.Windows.Forms.PictureBox();
            this.btn_minimize_application = new System.Windows.Forms.PictureBox();
            this.btn_maximized_application = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.btn_close_application)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_minimize_application)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_maximized_application)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_close_application
            // 
            this.btn_close_application.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_close_application.Image = ((System.Drawing.Image)(resources.GetObject("btn_close_application.Image")));
            this.btn_close_application.Location = new System.Drawing.Point(1344, 15);
            this.btn_close_application.Name = "btn_close_application";
            this.btn_close_application.Size = new System.Drawing.Size(30, 30);
            this.btn_close_application.TabIndex = 1;
            this.btn_close_application.TabStop = false;
            // 
            // btn_minimize_application
            // 
            this.btn_minimize_application.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_minimize_application.Image = ((System.Drawing.Image)(resources.GetObject("btn_minimize_application.Image")));
            this.btn_minimize_application.Location = new System.Drawing.Point(1220, 15);
            this.btn_minimize_application.Name = "btn_minimize_application";
            this.btn_minimize_application.Size = new System.Drawing.Size(30, 30);
            this.btn_minimize_application.TabIndex = 2;
            this.btn_minimize_application.TabStop = false;
            // 
            // btn_maximized_application
            // 
            this.btn_maximized_application.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_maximized_application.Image = ((System.Drawing.Image)(resources.GetObject("btn_maximized_application.Image")));
            this.btn_maximized_application.Location = new System.Drawing.Point(1282, 15);
            this.btn_maximized_application.Name = "btn_maximized_application";
            this.btn_maximized_application.Size = new System.Drawing.Size(30, 30);
            this.btn_maximized_application.TabIndex = 3;
            this.btn_maximized_application.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ClientSize = new System.Drawing.Size(1412, 781);
            this.Controls.Add(this.btn_maximized_application);
            this.Controls.Add(this.btn_minimize_application);
            this.Controls.Add(this.btn_close_application);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(1400, 750);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(10, 60, 10, 10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.btn_close_application)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_minimize_application)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_maximized_application)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox btn_close_application;
        private System.Windows.Forms.PictureBox btn_minimize_application;
        private System.Windows.Forms.PictureBox btn_maximized_application;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
    }
}

