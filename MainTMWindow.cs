using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automation
{
    public partial class MainTMWindow : Form
    {
        public ControlPanel main_panel;
        AllSettings module_settings;
        Delegate mouse_event_handler;
        Modbus port;
        MainForm main_form;

        public MainTMWindow(AllSettings module_settings, Delegate mouse_event_handler, Modbus port, MainForm main_form)
        {
            this.main_form = main_form;
            this.port = port;
            this.mouse_event_handler = mouse_event_handler;
            this.module_settings = module_settings;
            InitializeComponent();
            main_panel_add();
            fill_left_panel();
            fill_right_panle();
            fill_third_panel();
            fill_forth_panel();
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
                using_name: "Indikation Panel",
                dock_style: DockStyle.Left,
                using_auto_scroll: true,
                using_width: 350,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 10, 0)
                    )));

            main_panel.add(new ControlPanel(
                using_name: "Third Panel",
                dock_style: DockStyle.Left,
                using_auto_scroll: true,
                using_width: 350,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 10, 0)
                    )));

            main_panel.add(new ControlPanel(
                using_name: "Right Panel",
                dock_style: DockStyle.Left,
                using_auto_scroll: true,
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
            // **** i out **** //

            main_panel.search_panel_control("Left Panel")[0].add(new ControlPanel(
                using_name: "i out panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 55)
                    )));

            for (int a = 0; a < 2; a++)
            {
                main_panel.search_panel_control("i out panel")[0].add(new ControlButton(
                    using_name: "i_out",
                    using_text: $"Ток выхода, канал{2 - a}: ",
                    value_data_sending: module_settings.AllAddres.i_out_value[a],
                    color_data_sending: module_settings.AllAddres.i_out_color[a]
                    ));
            }


            // **** U out **** //

            main_panel.search_panel_control("Left Panel")[0].add(new ControlPanel(
                using_name: "U out panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 55)
                    )));

            for (int a = 0; a < 2; a++)
            {
                main_panel.search_panel_control("U out panel")[0].add(new ControlButton(
                    using_name: "u_out",
                    using_text: $"Напряжение выхода {2 - a}: ",
                    value_data_sending: module_settings.AllAddres.u_out_value[a],
                    color_data_sending: module_settings.AllAddres.u_out_color[a]
                    ));
            }

            // **** DIN **** //

            main_panel.search_panel_control("Left Panel")[0].add(new ControlPanel(
                using_name: "Din Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 35)
                    )));

            for (int a = 0; a < 16; a++)
            {
                main_panel.search_panel_control("Din Panel")[0].add(new ControlButton(
                using_name: "din16",
                using_text: $"Din {16 - a}: ",
                using_delegate: mouse_event_handler,
                using_description: "Din модуля. Подача потенциала и показания с модуля.",
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
                        using_delegate: new MouseEventHandler(all_button_on)),
                    new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_text: "Сменить min max значения ",
                        using_name: "din",
                        using_height: 55,
                        using_delegate: new MouseEventHandler(change_min_max))
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
            
            // **** Dout **** //

            main_panel.search_panel_control("Left Panel")[0].add(new ControlPanel(
                using_name: "Dout Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    )));

            for (int a = 0; a < 16; a++)
            {
                main_panel.search_panel_control("Dout Panel")[0].add(new ControlButton(
                using_name: "Dout",
                using_text: $"Dout {16 - a}: ",
                using_delegate: mouse_event_handler,
                using_description: "Din модуля. Подача потенциала и показания с модуля.",
                using_menu: new ButtonConstructor[] {new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_height: 55,
                        using_text: "Выключить все Dout",
                        using_delegate: new MouseEventHandler(all_button_off),
                        using_name: "Dout"),
                    new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_text: "Включить все Dout",
                        using_name: "Dout",
                        using_height: 55,
                        using_delegate: new MouseEventHandler(all_button_on))
                },
                send_data: new DataSending(
                    using_addres: module_settings.module,
                    using_data: new byte[] { 0x01, 0x06, 0x00, (byte)(0x60-a), 0x00, 0x00 },
                    port_name: "Module Port"
                    ),
                value_data_sending: module_settings.AllAddres.dout_value[a],
                color_data_sending: module_settings.AllAddres.dout_color[a]
                ));
            }            
        }

        void fill_right_panle()
        {
            // **** Temperature **** //

            main_panel.search_panel_control("Right Panel")[0].add(new ControlPanel(
                using_name: "Temperature panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 55)
                    )));
            main_panel.search_panel_control("Temperature panel")[0].add(new ControlButton(
                using_name: "Temperature",
                using_text: $"Температура: ",
                value_data_sending: module_settings.AllAddres.temperature_value[0]
                ));
            
            // **** akk **** //

            main_panel.search_panel_control("Right Panel")[0].add(new ControlPanel(
                using_name: "akk panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 55)
                    )));
            string[] akk = new string[] { "Напряжение аккумулятора: ", "Ток заряда аккумулятора: " };
            for(int a=0; a<akk.Length; a++)
            {
                main_panel.search_panel_control("akk panel")[0].add(new ControlButton(
                using_name: "akk",
                using_text: akk[a],
                value_data_sending: module_settings.AllAddres.akk_value[a]
                ));
            }
            
            // **** current 0 **** //

            main_panel.search_panel_control("Right Panel")[0].add(new ControlPanel(
                using_name: "current 0 panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 55)
                    )));
            main_panel.search_panel_control("current 0 panel")[0].add(new ControlButton(
            using_name: "current 0",
            using_text: "Current 0: ",
            using_delegate: mouse_event_handler,
            color_data_sending: new CheckButtonClass(
                    module: module_settings.mtu5,
                    address: new byte[2] { 0x00, 0x0f },
                    port: "Control Port"),
            send_data: new DataSending(
                        using_addres: module_settings.mtu5,
                        using_data: new byte[] { 0x01, 0x06, 0x00, 0x63, 0x00, 0x00 },
                        port_name: "Control Port"),
            value_data_sending: module_settings.AllAddres.curr0_value
            ));
            

            // **** TU **** //

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
                    using_delegate: mouse_event_handler,
                    color_data_sending: module_settings.AllAddres.tu_color[a],
                    send_data: new DataSending(
                        using_addres: module_settings.module,
                        using_data: new byte[] { 0x01, 0x06, 0x00, (byte)(0x63 - a), 0x00, 0x00 },
                        port_name: "Module Port"
                    )));
            }
            main_panel.search_panel_control("tu Panel")[0].add(new ControlButton(
                    using_name: "EnTU",
                    using_text: "EnTU",
                    value_data_sending: module_settings.AllAddres.tu[3],
                    color_data_sending: new CheckButtonClass(
                        module: module_settings.module,
                        address: new byte[2] { 0x00, 0x09 },
                        port: "Module Port")
                    ));

            // **** KF **** //

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
                    using_description: "КФ модуля. Подача потенциала и показания с модуля.",
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
                        using_delegate: new MouseEventHandler(all_button_on)),
                    new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_text: "Сменить min max значения ",
                        using_name: "kf",
                        using_height: 55,
                        using_delegate: new MouseEventHandler(change_min_max))
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

            // **** TC **** //

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
                    using_description: "TC модуля. Подача потенциала и показания с модуля.",
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
                        using_delegate: new MouseEventHandler(all_button_on)),
                    new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_text: "Сменить min max значения ",
                        using_name: "tc",
                        using_height: 55,
                        using_delegate: new MouseEventHandler(change_min_max))
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
            for (int a = 0; a < 4; a++)
            {
                main_panel.search_panel_control("Third Panel")[0].add(new ControlButton(
                    using_name: $"Module Power",
                    using_text: $"Питание {4-a} канал: ",
                    value_data_sending: module_settings.AllAddres.power[a],
                    using_delegate: mouse_event_handler,
                    using_description: "Измерение питания модуля. Показания самого модуля.",
                    using_menu: new ButtonConstructor[] {
                    new ButtonConstructor(
                        using_button_constructor: new ControlConstructor(
                        using_color: Color.FromArgb(113, 125, 137),
                        using_padding: new Padding(40, 0, 0, 0)),
                        using_text: "Сменить min max значения ",
                        using_name: "module power",
                        using_height: 55,
                        using_delegate: new MouseEventHandler(change_min_max))
                    }
                    ));
            }
        }

        void fill_forth_panel()
        {
            main_panel.search_panel_control("Indikation Panel")[0].add(new ControlPanel(
                using_name: "kf indikation Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_padding: new Padding(0, 0, 0, 55),
                    using_color: Color.Black
                    )));

            main_panel.search_panel_control("Indikation Panel")[0].add(new ControlPanel(
                using_name: "Din indikation Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_padding: new Padding(0, 0, 0, 55),
                    using_color: Color.Black
                    )));
            
            main_panel.search_panel_control("Indikation Panel")[0].add(new ControlPanel(
                using_name: "TU indikation Panel",
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_padding: new Padding(0, 0, 0, 55),
                    using_color: Color.Black
                    )));

            for (int a = 0; a < 16; a++)
            {
                main_panel.search_panel_control("Din indikation Panel")[0].add(new ControlButton(
                    using_name: $"Din indikation",
                    using_text: $"Din {16-a}",
                    value_data_sending: module_settings.AllAddres.din16_ind_value[a],
                    color_data_sending: module_settings.AllAddres.din16_ind[a]
                    ));
            }
            string[] tu = new string[] { "Авария RF", "Авария OFF", "Авария ON" };
            for (int a = 0; a < 3; a++)
            {
                main_panel.search_panel_control("kf indikation Panel")[0].add(new ControlButton(
                    using_name: $"kf indikation",
                    using_text: $"KF {(char)('C'-a)}",
                    value_data_sending: module_settings.AllAddres.kf_ind_value[a],
                    color_data_sending: module_settings.AllAddres.kf_ind[a]
                    ));
                main_panel.search_panel_control("TU indikation Panel")[0].add(new ControlButton(
                    using_name: $"TU indikation",
                    using_text: $"{tu[a]}",
                    value_data_sending: module_settings.AllAddres.tu_ind_value[a],
                    color_data_sending: module_settings.AllAddres.tu_ind[a]
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

        void change_min_max(object sender, MouseEventArgs e)
        {
            MinMaxNone min_max = module_settings.all_min_max.Find(x => x.name == ((ControlButton)sender).Name);

            EnterField ef = new EnterField($"  Введи новые значения min max и none для {min_max.name}", min_max.min_max_get());
            ef.Show();
            ef.enter_handler += (text) =>
            {
                min_max.min_max_set(text);
                BeginInvoke((MethodInvoker)(() =>
                {
                    ((ControlButton)sender).Set_Text = min_max.min_max_get();
                }));
                ef.Dispose();
            };
        }
    }
}
