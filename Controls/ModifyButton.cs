using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automation
{
    public class ControlButton : Button
    {
        public string button_description = "";
        public string result_text = "";
        public float result = 0;
        public ButtonConstructor[] menuing;
        public CheckButtonClass color_data_sending;
        public CheckButtonClass value_data_sending;
        public Color start_color;
        public DataSending send_data;
        public Delegate mouse_click;
        public delegate void cmh(ControlButton button);
        cmh click_mouse_handler;

        public ControlButton(
            DockStyle dock_style = DockStyle.Top,
            ControlConstructor using_button_constructor = null,
            ControlConstructor using_button_text_constructor = null,
            int using_width = 350,
            int using_height = 55,
            string using_text = "",
            string using_name = "",
            string using_description = "",
            Delegate using_delegate = null,
            ToolTip using_tooltip = null,
            ButtonConstructor[] menuing = null,
            bool hide_panel = true,
            CheckButtonClass color_data_sending = null,
            CheckButtonClass value_data_sending = null,
            DataSending send_data = null,
            string start_text = "",
            cmh click_mouse_handler = null
        )
        {
            this.TabStop = false;
            this.result_text = using_text;
            //this.MouseDown += (MouseEventHandler)using_delegate;
            //mouse_click = using_delegate;
            this.click_mouse_handler = click_mouse_handler;
            this.MouseDown += this.click_mouse_event;
            this.Width = using_width;
            this.Height = using_height;
            this.Dock = dock_style;
            this.Name = using_name;
            this.Text = using_text;
            this.button_description = using_description;
            this.Visible = hide_panel;
            this.FlatStyle = FlatStyle.Flat;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.FlatAppearance.BorderSize = 0;
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 204);
            if (using_button_constructor != null)
            {
                if (using_button_constructor.control_color != null)
                {
                    this.BackColor = using_button_constructor.control_color;
                    this.start_color = using_button_constructor.control_color;
                }
                if (using_button_constructor.control_padding != null)
                    this.Padding = using_button_constructor.control_padding;
            }
            if (using_button_text_constructor != null)
            {
                if (using_button_text_constructor.control_color != null)
                    this.ForeColor = using_button_text_constructor.control_color;
            }
            if (menuing != null)
                this.menuing = menuing;
            if (color_data_sending != null)
                this.color_data_sending = color_data_sending;
            if (send_data != null)
                this.send_data = send_data;
            if (start_text != "")
                Set_Text = start_text;
            if (value_data_sending != null)
            {
                this.value_data_sending = value_data_sending;
                if (value_data_sending.button_text != "")
                    this.Text = value_data_sending.button_text;
                if (value_data_sending.address[0] == 0x0 && value_data_sending.address[1] == 0x0)
                {
                    this.Visible = false;
                }
            }
        }

        public ControlButton(ButtonConstructor constructor)
        {
            this.TabStop = false;
            this.FlatStyle = FlatStyle.Flat;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 204);
            this.FlatAppearance.BorderSize = 0;
            this.Dock = DockStyle.Top;
            this.BackColor = Color.FromArgb(113, 125, 137);
            this.Padding = new Padding(40, 0, 0, 0);
            this.Name = constructor.name;
            this.Text = constructor.text;
            this.Width = 100;
            this.Height = 35;
            this.button_description = constructor.description;
            this.click_mouse_handler = constructor.click_mouse_handler;
        }

        public float Result
        {
            get { return this.result; }
            set
            {
                this.result = value;
                try
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        this.Text = this.result_text + Math.Round(value, 3).ToString();
                    }));
                }
                catch { }

            }
        }

        public void check_value_result(byte[] using_addres, float new_value, string port_name)
        {
            if (value_data_sending == null || value_data_sending.module.Addres != using_addres[0] || value_data_sending.address[0] != using_addres[1] || value_data_sending.address[1] != using_addres[2] || value_data_sending.port != port_name) return;
            Result = new_value;
        }

        public void check_color_result(byte[] using_addres, int change_color, string port_name)
        {
            if (color_data_sending == null || color_data_sending.module.Addres != using_addres[0] || color_data_sending.address[0] != using_addres[1] || color_data_sending.address[1] != using_addres[2] || color_data_sending.port != port_name) return;
            this.BackColor = change_color != 0 ? Color.Red : this.start_color;
        }

        public string Set_Text
        {
            set { this.Text = result_text + value; }
        }


        public byte button_on_off()
        {
            return this.BackColor == Color.Red ? (byte)0x00 : (byte)0x01;
        }

        void click_mouse_event(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this.menuing != null)
                open_form_with_dialog();
            else if (e.Button == MouseButtons.Left)
                click_mouse_handler(this);
        }

        void open_form_with_dialog()
        {
            windows_variant win = new windows_variant(
                Cursor.Position.X - 50,
                Cursor.Position.Y - 50,
                this.button_description,
                this.menuing);
            win.StartPosition = FormStartPosition.Manual;
            win.Show();
        }
    }
}
