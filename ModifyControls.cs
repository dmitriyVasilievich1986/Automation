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

    public class ControlConstructor
    {
        public Color control_color;
        public Padding control_padding;

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
        public float result = 0;

        public ControlButton(
            DockStyle dock_style = DockStyle.Top,
            ControlConstructor using_button_constructor = null,
            ControlConstructor using_button_text_constructor = null,
            int using_width = 100,
            int using_height = 100,
            string using_text = "",
            string using_name = "",
            string using_description = "",
            Delegate using_delegate = null,
            ToolTip using_tooltip = null,
            bool hide_panel = true
        )
        {
            this.MouseClick += (MouseEventHandler)using_delegate;
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
                    this.BackColor = using_button_constructor.control_color;
                if (using_button_constructor.control_padding != null)
                    this.Padding = using_button_constructor.control_padding;
            }
            if (using_button_text_constructor != null)
            {
                if (using_button_text_constructor.control_color != null)
                    this.ForeColor = using_button_text_constructor.control_color;
            }
            if (using_tooltip != null)
                button_tooltip = using_tooltip;
        }

        public float Result
        {
            get { return this.result; }
            set { this.result = value; }
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
}
