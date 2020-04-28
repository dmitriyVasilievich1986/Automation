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
    public partial class MainTMWindow : Form
    {
        public ControlPanel main_panel;
        AllSettings module_settings;

        public MainTMWindow(AllSettings module_settings)
        {
            this.module_settings = module_settings;
            InitializeComponent();
            main_panel_add();
            fill_left_panel();
            all_button_style();
        }

        void main_panel_add()
        {
            main_panel = new ControlPanel(
                dock_style: DockStyle.Fill,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    ));
            this.Controls.Add(main_panel);

            main_panel.add(new ControlPanel(
                using_name: "Right Panel",
                dock_style: DockStyle.Left,
                using_width: 350,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 10, 0)
                    )));

            main_panel.add(new ControlPanel(
                using_name: "Left Panel",
                dock_style: DockStyle.Left,
                using_auto_scroll: true,
                using_width: 350,
                using_panel_constructor: new ControlConstructor(
                    using_padding: new Padding(0, 0, 10, 0),
                    using_color: Color.Black
                    )
                ));
        }

        void fill_left_panel()
        {
            main_panel.search_panel_control("Left Panel")[0].add(new ControlPanel(
                using_name: "Din Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    )));

            for (int a = 0; a < 16; a++)
            {
                main_panel.search_panel_control("Din Panel")[0].add(new ControlButton(
                using_name: "din16",
                using_text: $"Din {16 - a}: ",
                color_data_sending: new CheckButtonClass(
                    module: module_settings.dout_din16,
                    address: new byte[2] { 0x00, (byte)(16-a) },
                    port: "Control Port")
                ));
            }            
        }

        void all_button_style()
        {
            foreach(ControlButton cb in main_panel.search_button_control())
            {
                cb.BackColor = Color.Gray;
                cb.start_color = Color.Gray;
                cb.Padding = new Padding(40, 0, 0, 0);
            }
        }
    }
}
