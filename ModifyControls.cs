using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automation
{
    class ModifyControls
    {

    }

    public class ButtonConstructor
    {
        public DockStyle dock_style;
        public Color back_color;
        public Padding button_padding;
        public Color text_color;
        public Padding text_padding;
        public int width;
        public int height;
        public string name;
        public string text;
        public string description;
        public Delegate new_delegate;

        public ButtonConstructor(
            DockStyle using_dock_style = DockStyle.Top,
            ControlConstructor using_button_constructor = null,
            ControlConstructor using_button_text_constructor = null,
            int using_width = 100,
            int using_height = 100,
            string using_text = "",
            string using_name = "",
            string using_description = "",
            Delegate using_delegate = null,
            ToolTip using_tooltip = null,
            ControlButton[] using_menu = null,
            bool hide_panel = true
        )
        {
            dock_style = using_dock_style;
            if (using_button_constructor != null) back_color = using_button_constructor.control_color;
            if (using_button_constructor != null) button_padding = using_button_constructor.control_padding;
            if (using_button_text_constructor != null) text_color = using_button_text_constructor.control_color;
            if (using_button_text_constructor != null) text_padding = using_button_text_constructor.control_padding;
            width = using_width;
            height = using_height;
            name = using_name;
            text = using_text;
            description = using_description;
            new_delegate = using_delegate;
        }
    }

    public class ControlConstructor
    {
        public Color control_color = Color.White;
        public Padding control_padding = new Padding(0, 0, 0, 0);

        public ControlConstructor(Color using_color)
        {
            control_color = using_color;
        }

        public ControlConstructor(Padding using_padding)
        {
            control_padding = using_padding;
        }

        public ControlConstructor(Color using_color, Padding using_padding)
        {
            control_padding = using_padding;
            control_color = using_color;
        }
    }

    public class ControlButton : Button
    {
        /*
         
         */

        ToolTip button_tooltip;
        public string button_description = "";
        public string result_text = "";
        public float result = 0;
        public ButtonConstructor[] menuing;
        public string port_name;
        public DataAddress module;
        public byte[] addres = new byte[2];
        public Color start_color;

        public ControlButton(
            byte[] addres,
            DockStyle dock_style = DockStyle.Top,
            ControlConstructor using_button_constructor = null,
            ControlConstructor using_button_text_constructor = null,
            DataAddress module = null,
            int using_width = 100,
            int using_height = 100,
            string using_text = "",
            string using_name = "",
            string using_port_name = "",
            string using_description = "",
            Delegate using_delegate = null,
            ToolTip using_tooltip = null,
            ButtonConstructor[] using_menu = null,
            bool hide_panel = true
        )
        {
            this.addres = addres;
            this.result_text = using_text;
            this.port_name = using_port_name;
            this.MouseDown += (MouseEventHandler)using_delegate;
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
            if (module != null) this.module = module;
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
            if (using_menu != null)
                menuing = using_menu;
            if (using_tooltip != null)
                button_tooltip = using_tooltip;
        }

        public ControlButton(ButtonConstructor constructor)
        {
            this.FlatStyle = FlatStyle.Flat;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 204);
            this.FlatAppearance.BorderSize = 0;

            Dock = constructor.dock_style;
            if (constructor.back_color != null) BackColor = constructor.back_color;
            if (constructor.button_padding != null) Padding = constructor.button_padding;
            if (constructor.text_color != null) ForeColor = constructor.text_color;
            Width = constructor.width;
            Height = constructor.height;
            Name = constructor.name;
            Text = constructor.text;
            button_description = constructor.description;
            this.MouseDown += (MouseEventHandler)constructor.new_delegate;
        }

        public float Result
        {
            get { return this.result; }
            set
            {
                this.result = value;
                BeginInvoke((MethodInvoker)(() =>
                {
                    this.Text = this.result_text + value.ToString();
                }));                
            }
        }

        public void check_result(byte[] using_addres, float new_value)
        {
            if (using_addres[0] != module.Addres || addres[0] != using_addres[1] || addres[1] != using_addres[2]) return;
            Result = new_value;
        }

        public void button_color(int change_color)
        {
            this.BackColor = change_color == 1 ? Color.Red : this.start_color;
        }
    }

    public class ControlPanel : Panel
    {
        /*
        ** Панель для упрощения работы, с собственными методами **
        */

        ToolTip panel_tooltip;
        bool float_height;

        public ControlPanel(
            DockStyle dock_style = DockStyle.Top,
            ControlConstructor using_panel_constructor = null,
            int using_width = 100,
            int using_height = 100,
            string using_name = "",
            ToolTip using_tooltip = null,
            bool using_float_height = false,
            bool hide_panel = true )
        {
            this.Width = using_width;
            this.float_height = using_float_height;
            this.Height = using_height;
            this.Dock = dock_style;
            this.Name = using_name;
            this.Visible = hide_panel;
            if (using_panel_constructor != null){
                if (using_panel_constructor.control_color != null)
                    this.BackColor = using_panel_constructor.control_color;
                if (using_panel_constructor.control_padding != null)
                    this.Padding = using_panel_constructor.control_padding;
            }
            if (using_tooltip != null)
                panel_tooltip = using_tooltip;
            if (using_float_height)
                set_height();
        }

        public void set_height()
        {
            if (!float_height) return;
            this.Height = this.Padding.Top;
            foreach(ControlButton cb in search_button_control())
            {
                this.Height += cb.Height;
            }
        }

        public List<ControlButton> search_button_control(string search_name = "")
        {
            List<ControlButton> output = new List<ControlButton>();
            foreach(var cb in this.Controls)
            {
                if(cb.GetType() == typeof(ControlButton))
                {
                    if (search_name != "" && ((ControlButton)cb).Name == search_name)
                        output.Add((ControlButton)cb);
                    else if (search_name == "")
                        output.Add((ControlButton)cb);
                }
                else if (cb.GetType() == typeof(ControlPanel))
                {
                    output.AddRange(((ControlPanel)cb).search_button_control(search_name).ToArray());
                }
            }
            return output;
        }

        public List<ControlPanel> search_panel_control(string search_name = "")
        {
            List<ControlPanel> output = new List<ControlPanel>();
            foreach(var cp in this.Controls)
            {
                if(cp.GetType() == typeof(ControlPanel))
                {
                    if (search_name != "" && ((ControlPanel)cp).Name == search_name) 
                        output.Add((ControlPanel)cp);
                    else if (search_name == "") 
                        output.Add((ControlPanel)cp);
                    output.AddRange(((ControlPanel)cp).search_panel_control(search_name).ToArray());
                }
            }
            return output;
        }

        public void add(ControlPanel add_panel)
        {
            this.Controls.Add(add_panel);
        }

        public void add(ControlButton add_button)
        {
            this.Controls.Add(add_button);
            if (float_height)
                set_height();
        }

        public void tooltip_activate()
        {
            if (panel_tooltip != null)
            {
                panel_tooltip.SetToolTip(this, "asd");
            }
        }
    }

    public class AllSettings
    {
        public DataAddress dout_din16 = new DataAddress(20);
        public DataAddress dout_din32 = new DataAddress(21);
        public DataAddress dout_control = new DataAddress(19);
        public DataAddress mtu5 = new DataAddress(16);
        public DataAddress psc = new DataAddress(17);
    }

    public class ButtonValue
    {
        public byte[] addres;
        public string port_name;
        public byte module;

        public ButtonValue(
            byte[] addres,
            string port_name,
            byte module
            )
        {
            this.module = module;
            this.addres = addres;
            this.port_name = port_name;
        }
    }
}
