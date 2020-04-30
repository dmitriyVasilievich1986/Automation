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
        Delegate port_module;
        Modbus port;

        public MainTMWindow(AllSettings module_settings, Delegate mouse_event_handler, Modbus port, Delegate port_module)
        {
            this.port_module = port_module;
            this.port = port;
            this.mouse_event_handler = mouse_event_handler;
            this.module_settings = module_settings;
            InitializeComponent();
            main_panel_add();
            fill_left_panel();
            fill_right_panle();
            fill_third_panel();
            all_button_style();
            this.FormClosing += (s, e) => { this.Dispose(); };
            this.Shown += (s, e) => { foreach (ControlPanel cp in main_panel.search_panel_control()) cp.set_visible(); };            
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
                using_name: "Third Panel",
                dock_style: DockStyle.Left,
                using_auto_scroll: true,
                using_width: 350,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(10, 0, 10, 0)
                    )));

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
                using_menu: new ButtonConstructor[] {new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_height: 55,
                        using_text: "Выключить все Din",
                        using_delegate: new MouseEventHandler(all_button_off),
                        using_name: "din16"),
                    new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_text: "Включить все Din",
                        using_name: "din16",
                        using_height: 55,
                        using_delegate: new MouseEventHandler(all_button_on))
                },
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
                using_name: "tu Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 55)
                    )));
                        
            string[] new_text = new string[] { "RF ", "OFF ", "ON " };
            for (int a = 0; a < 3; a++) 
            {
                main_panel.search_panel_control("tu Panel")[0].add(new ControlButton(
                    using_name: "tu",
                    using_text: new_text[a],
                    value_data_sending: module_settings.AllAddres.tu[a],
                    using_delegate: port_module,
                    color_data_sending: module_settings.AllAddres.tu_color[a],
                    send_data: new DataSending(
                        using_addres: module_settings.module,
                        using_data: new byte[] { 0x01, 0x06, 0x00, (byte)(0x63 - a), 0x00, 0x00 }
                    )));
            }
            main_panel.search_panel_control("tu Panel")[0].add(new ControlButton(
                    using_name: "entu",
                    using_text: "EnTU",
                    value_data_sending: module_settings.AllAddres.tu[3],
                    color_data_sending: new CheckButtonClass(
                        module: module_settings.module,
                        address: new byte[2] { 0x00, 0x09 },
                        port: "Module Port")
                    ));

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
                    value_data_sending: module_settings.AllAddres.kf[a],
                    using_delegate: mouse_event_handler,
                    using_menu: new ButtonConstructor[] {new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_height: 55,
                        using_text: "Выключить все KF",
                        using_delegate: new MouseEventHandler(all_button_off),
                        using_name: "kf"),
                    new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_text: "Включить все KF",
                        using_name: "kf",
                        using_height: 55,
                        using_delegate: new MouseEventHandler(all_button_on))
                    },
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
                    value_data_sending: module_settings.AllAddres.tc[a],
                    using_delegate: mouse_event_handler,
                    using_menu: new ButtonConstructor[] {new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_height: 55,
                        using_text: "Выключить все TC",
                        using_delegate: new MouseEventHandler(all_button_off),
                        using_name: "tc"),
                    new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_text: "Включить все TC",
                        using_name: "tc",
                        using_height: 55,
                        using_delegate: new MouseEventHandler(all_button_on))
                    },
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

        void fill_third_panel()
        {

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

        void all_button_on(object sender, MouseEventArgs e)
        {
            List<ControlButton> all_button = main_panel.search_button_control(((ControlButton)sender).Name).FindAll(x => x.Visible);
            if (all_button.Count == 0) return;
            List<byte> data = new List<byte>() { all_button[0].send_data.address.Addres, 0x10, 0x00, all_button[all_button.Count - 1].send_data.data[3], 0x00, (byte)all_button.Count, (byte)(all_button.Count * 2) };
            for (int a = 0; a < all_button.Count; a++)
            {
                data.Add(0x00);
                data.Add(0x01);
            }
            port.set_interrupt(data.ToArray());
        }

        void all_button_off(object sender, MouseEventArgs e)
        {
            List<ControlButton> all_button = main_panel.search_button_control(((ControlButton)sender).Name).FindAll(x => x.Visible);
            if (all_button.Count == 0) return;
            List<byte> data = new List<byte>() { all_button[0].send_data.address.Addres, 0x10, 0x00, all_button[all_button.Count - 1].send_data.data[3], 0x00, (byte)all_button.Count, (byte)(all_button.Count * 2) };
            for (int a = 0; a < all_button.Count; a++)
            {
                data.Add(0x00);
                data.Add(0x00);
            }
            port.set_interrupt(data.ToArray());
        }
    }
}
