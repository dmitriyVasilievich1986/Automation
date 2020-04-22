using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automation
{
    public partial class ComPortOptions : Form
    {
        Modbus port;
        ControlPanel main_panel;

        public ComPortOptions(Modbus using_port)
        {
            port = using_port;
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.TopLevel = false;
            this.BringToFront();
            control_panel_add();
        }

        void control_panel_add()
        {
            main_panel = new ControlPanel(
                using_name: "main_panel",
                dock_style: DockStyle.Fill,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    ));
            main_panel = new ControlPanel(
                using_name: "back_button_panel",
                dock_style: DockStyle.Top,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    ));
            main_panel.add(new ControlButton(
                dock_style: DockStyle.Left,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "test",
                using_text: "test",
                using_height: 55
                ));
            this.Controls.Add(main_panel);
        }
    }
}
