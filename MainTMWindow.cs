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
        Delegate mouse_event_handler;

        public MainTMWindow(AllSettings module_settings, Delegate mouse_event_handler)
        {
            this.mouse_event_handler = mouse_event_handler;
            this.module_settings = module_settings;
            InitializeComponent();
            main_panel_add();
            fill_left_panel();
            fill_right_panle();
            all_button_style();
            this.FormClosing += (s, e) => { this.Dispose(); };
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
                using_auto_scroll: true,
                using_width: 350,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(10, 0, 10, 0)
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
                using_delegate: mouse_event_handler,
                send_data: new DataSending(
                    using_addres: module_settings.dout_din16,
                    using_data: new byte[] { 0x01, 0x06, 0x00, (byte)(0x60-a), 0x00, 0x00 }
                    ),
                value_data_sending: module_settings.AllAddres.din16[a],
                color_data_sending: new CheckButtonClass(
                    module: module_settings.dout_din16,
                    address: new byte[2] { 0x00, (byte)(16-a) },
                    port: "Control Port")
                ));
            }            
        }

        void fill_right_panle()
        {
            main_panel.search_panel_control("Right Panel")[0].add(new ControlPanel(
                using_name: "KF Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 55)
                    )));

            for(int a = 0; a < 3; a++)
            {
                main_panel.search_panel_control("KF Panel")[0].add(new ControlButton(
                    using_name: "kf",
                    using_text: $"KF {(char)('C' - a)}: ",
                    using_delegate: mouse_event_handler,
                    color_data_sending: new CheckButtonClass(
                        module: module_settings.dout_control,
                        address: new byte[2] { 0x00, (byte)(15 - a) },
                        port: "Control Port"),
                    send_data: new DataSending(
                        using_addres: module_settings.dout_control,
                        using_data: new byte[] { 0x01, 0x06, 0x00, (byte)(0x5f - a), 0x00, 0x00 }
                    )));
            }

            main_panel.search_panel_control("Right Panel")[0].add(new ControlPanel(
                using_name: "TC Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 55)
                    )));

            for(int a = 0; a < 3; a++)
            {
                main_panel.search_panel_control("TC Panel")[0].add(new ControlButton(
                    using_name: "tc",
                    using_text: $"TC {(char)('C' - a)}: ",
                    using_delegate: mouse_event_handler,
                    color_data_sending: new CheckButtonClass(
                        module: module_settings.dout_control,
                        address: new byte[2] { 0x00, (byte)(7 - a) },
                        port: "Control Port"),
                    send_data: new DataSending(
                        using_addres: module_settings.dout_control,
                        using_data: new byte[] { 0x01, 0x06, 0x00, (byte)(0x57 - a), 0x00, 0x00 }
                    )));
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
