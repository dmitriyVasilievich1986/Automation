using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automation
{
    public class ButtonConstructor
    {
        public string name;
        public string text;
        public string description;
        public Delegate new_delegate;
        public ControlButton.cmh click_mouse_handler;

        public ButtonConstructor(
            string text = "",
            string name = "",
            string description = "",
            ControlButton.cmh click_mouse_handler = null
        )
        {
            this.text = text;
            this.name = name;
            this.description = description;
            this.click_mouse_handler = click_mouse_handler;
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
            bool using_auto_scroll = false,
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
            this.AutoScroll = using_auto_scroll;
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
            this.Height = this.Padding.Top + this.Padding.Bottom;
            foreach(ControlButton cb in search_button_control())
            {
                this.Height += cb.Height;
            }
        }

        public void set_visible()
        {
            //if (!float_height) return;
            if (search_button_control().FindAll(x => x.Visible).Count == 0) this.Visible = false;
            else
            {
                this.Visible = true;
                this.Height = this.Padding.Top + this.Padding.Bottom;
                foreach (ControlButton cb in search_button_control())
                    if (cb.Visible)
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
                else if (cb.GetType() == typeof(MainTMWindow))
                {
                    output.AddRange(((MainTMWindow)cb).main_panel.search_button_control(search_name).ToArray());
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
                else if (cp.GetType() == typeof(MainTMWindow))
                {
                    output.AddRange(((MainTMWindow)cp).main_panel.search_panel_control(search_name).ToArray());
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
