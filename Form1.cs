using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace Automation
{
    public partial class MainForm: Form
    {
        Point LastPoint;
        Settings settings_form;
        MainTMWindow main_tm_window;

        ControlPanel main_panel;
        ControlPanel control_panel;
        ControlPanel child_form_panel;

        TextBox condition_tb;
        TransmitionArray port1;

        Modbus port_control = new Modbus(using_port_name: Properties.Settings.Default.port1, using_name: "Control Port");
        Modbus port_chanel_a = new Modbus(using_port_name: Properties.Settings.Default.port2, using_name: "Module Port");
        Modbus port_chanel_b = new Modbus(using_port_name: Properties.Settings.Default.port3, using_name: "Module Port");
        Modbus port_chanel_c = new Modbus(using_port_name: Properties.Settings.Default.port4, using_name: "Module Port");
        AllSettings module_settings = new AllSettings();

        Dictionary<string, DataSending> send_control_port;

        public MainForm()
        {
            InitializeComponent();
            initialize_form_comtrol();
            control_init();
            load_condition_panel();
            all_form_load();
            condition_tb.Text += "    Форма загружена успешно" + Environment.NewLine;
            new_task_run();
            port_initialization();
        }

        #region initialization

        void port_initialization()
        {
            port_control.receive_handler += port_control_receive;
            port1 = new TransmitionArray(port_control);
            port1.Show();
        }

        private void initialize_form_comtrol()
        {
            this.MouseDown += (s, e) => { LastPoint = new System.Drawing.Point(e.X, e.Y); };
            this.MouseMove += (s, e) => {
                if (this.WindowState == FormWindowState.Maximized) return;
                if (e.Button == MouseButtons.Left && LastPoint.Y < 60)
                {
                    this.Left += e.X - LastPoint.X;
                    this.Top += e.Y - LastPoint.Y;
                }
                else if (e.Button == MouseButtons.Left)
                {
                    this.Width += (e.X - LastPoint.X) / 40;
                    this.Height += (e.Y - LastPoint.Y) / 40;
                }
            };
            btn_close_application.MouseClick += (s, e) =>
            {
                Properties.Settings.Default.port1 = port_control.PortName;
                Properties.Settings.Default.port2 = port_chanel_a.PortName;
                Properties.Settings.Default.port3 = port_chanel_b.PortName;
                Properties.Settings.Default.port4 = port_chanel_c.PortName;
                Properties.Settings.Default.Save();
                this.Dispose();
            };
            btn_minimize_application.MouseClick += (s, e) => { this.WindowState = FormWindowState.Minimized; };
            btn_maximized_application.MouseClick += (s, e) => {
                if (this.WindowState != FormWindowState.Maximized)
                    this.WindowState = FormWindowState.Maximized;
                else
                    this.WindowState = FormWindowState.Normal;
            };
        }

        private void control_init()
        {
            main_panel = new ControlPanel(
                    dock_style: DockStyle.Fill,
                    using_name: "main_panel",
                    using_width: 350,
                    using_panel_constructor: new ControlConstructor(
                        using_color: Color.Black,
                        using_padding: new Padding(10, 10, 10, 10))
                );
            control_panel = new ControlPanel(
                    dock_style: DockStyle.Left,
                    using_width: 350,
                    using_tooltip: new ToolTip(),
                    using_panel_constructor: new ControlConstructor(
                        using_color: Color.Black)
                );

            // **** module **** //

            control_panel.add(new ControlPanel(
                dock_style: DockStyle.Top,
                using_height: 100,
                using_float_height: true,
                hide_panel: false,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(28, 40, 52),
                    using_padding: new Padding(20, 0, 0, 0)),
                using_name: "module"
                ));
            control_panel.search_panel_control("module")[0].add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "test",
                using_text: "test",
                using_height: 55
                ));
            control_panel.search_panel_control("module")[0].add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "test",
                using_text: "test",
                using_height: 55
                ));
            control_panel.add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(44, 62, 80),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "module",
                using_text: "Выбор модуля",
                using_button_text_constructor: new ControlConstructor(using_color: Color.LightGray),
                using_delegate: new MouseEventHandler(hide_control_panel),
                using_height: 55
                ));

            // **** com ports **** //

            control_panel.add(new ControlPanel(
                dock_style: DockStyle.Top,
                using_height: 100,
                using_float_height: true,
                hide_panel: false,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(28, 40, 52),
                    using_padding: new Padding(20, 0, 0, 0)),
                using_name: "com_ports"
                ));
            control_panel.search_panel_control("com_ports")[0].add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_menu: new ButtonConstructor[] {new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Закрыть порт",
                    using_delegate: new MouseEventHandler(close_port),
                    using_name: "port_chanelC",
                    using_height: 55),
                    new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Открыть порт",
                    using_name: "port_chanelC",
                    using_delegate: new MouseEventHandler(open_port),
                    using_height: 55)
                },
                using_delegate: new MouseEventHandler(open_com_port_form),
                using_name: "port_chanelC",
                using_text: "Порт модуля канал C",
                using_description: "Один из портов обмена проверяемого модуля. Канал С.",
                using_height: 55
                ));
            control_panel.search_panel_control("com_ports")[0].add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(open_com_port_form),
                using_menu: new ButtonConstructor[] {new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Закрыть порт",
                    using_delegate: new MouseEventHandler(close_port),
                    using_name: "port_chanelB",
                    using_height: 55),
                    new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Открыть порт",
                    using_name: "port_chanelB",
                    using_delegate: new MouseEventHandler(open_port),
                    using_height: 55)
                },
                using_description: "Один из портов обмена проверяемого модуля. Канал B.",
                using_name: "port_chanelB",
                using_text: "Порт модуля канал B",
                using_height: 55
                ));
            control_panel.search_panel_control("com_ports")[0].add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_menu: new ButtonConstructor[] {new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Закрыть порт",
                    using_delegate: new MouseEventHandler(close_port),
                    using_name: "port_chanelA",
                    using_height: 55),
                    new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Открыть порт",
                    using_name: "port_chanelA",
                    using_delegate: new MouseEventHandler(open_port),
                    using_height: 55)
                },
                using_description: "Один из портов обмена проверяемого модуля. Канал A.",
                using_name: "port_chanelA",
                using_delegate: new MouseEventHandler(open_com_port_form),
                using_text: "Порт модуля канал А",
                using_height: 55
                ));
            control_panel.search_panel_control("com_ports")[0].add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(open_com_port_form),
                using_name: "control_port",
                using_text: "Порт управления",
                using_menu: new ButtonConstructor[] {new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Закрыть порт",
                    using_delegate: new MouseEventHandler(close_port),
                    using_name: "control_port",
                    using_height: 55),
                    new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Открыть порт",
                    using_name: "control_port",
                    using_delegate: new MouseEventHandler(open_port),
                    using_height: 55)
                },
                using_description: "Порт управления. Основной порт приема данных.",
                using_height: 55
                ));
            control_panel.add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(44, 62, 80),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_button_text_constructor: new ControlConstructor(using_color: Color.LightGray),
                using_name: "com_ports",
                using_delegate: new MouseEventHandler(hide_control_panel),
                using_text: "Доступные порты",
                using_height: 55
                ));
            this.Controls.Add(main_panel);
            this.Controls.Add(control_panel);
        }

        void all_form_load()
        {
            main_tm_window = new MainTMWindow();
            child_form_panel.Controls.Add(main_tm_window);
            main_tm_window.Show();
        }

        void load_condition_panel()
        {
            child_form_panel = new ControlPanel(
                dock_style: DockStyle.Fill,
                using_width: 350,
                using_tooltip: new ToolTip(),
                using_panel_constructor: new ControlConstructor(
                    using_padding: new Padding(0, 10, 0, 0),
                    using_color: Color.Black)
            );
            main_panel.add(child_form_panel);
            main_panel.add(new ControlPanel(
                using_name: "condition_panel",
                using_height: 220,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 0, 0))
                ));

            condition_tb = new TextBox();
            condition_tb.Enabled = true;
            condition_tb.Dock = DockStyle.Fill;
            condition_tb.Multiline = true;
            condition_tb.ScrollBars = ScrollBars.Vertical;
            condition_tb.BackColor = Color.LightGray;
            condition_tb.ForeColor = Color.Black;
            //condition_tb.TextChanged += (s, e) =>
            //{
            //    BeginInvoke((MethodInvoker)(() =>
            //    {
            //        //condition_tb.Lines = Data_Update.ToArray();
            //        condition_tb.SelectionStart = condition_tb.Text.Length;
            //        condition_tb.ScrollToCaret();
            //    }));
            //};
            condition_tb.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 204);
            main_panel.search_panel_control("condition_panel")[0].Controls.Add(condition_tb);

            // **** mtu condition panel **** //

            main_panel.search_panel_control("condition_panel")[0].add(new ControlPanel(
                using_name: "mtu_condition_panel",
                using_width: 350,
                dock_style: DockStyle.Left,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 10, 0))
                ));
            main_panel.search_panel_control("mtu_condition_panel")[0].add(new ControlButton(
                addres: new byte[1],
                using_name: "mtu_220v_tu",
                    using_text: "Напряжение ТУ: ",
                    using_button_constructor: new ControlConstructor(
                        using_color: Color.Gray,
                        using_padding: new Padding(40, 0, 0, 0)
                        ),
                    using_height: 55,
                    dock_style: DockStyle.Top
                ));
            main_panel.search_panel_control("mtu_condition_panel")[0].add(new ControlButton(
                addres: new byte[1],
                using_name: "mtu_tc_12v",
                using_text: "ТС 12В канал 2: ",
                using_button_constructor: new ControlConstructor(
                    using_color: Color.Gray,
                    using_padding: new Padding(40, 0, 0, 0)
                    ),
                using_height: 55,
                dock_style: DockStyle.Top
                ));
            main_panel.search_panel_control("mtu_condition_panel")[0].add(new ControlButton(
                addres: new byte[2] { 0x01, 0x08},
                module: module_settings.mtu5,
                using_name: "mtu_tc_12v",
                using_text: "ТС 12В канал 1: ",
                using_port_name: "Control Port",
                using_button_constructor: new ControlConstructor(
                    using_color: Color.Gray,
                    using_padding: new Padding(40, 0, 0, 0)
                    ),
                using_height: 55,
                dock_style: DockStyle.Top
                ));


            // **** power panel **** //

            main_panel.search_panel_control("condition_panel")[0].add(new ControlPanel(
                using_name: "power_panel",
                using_width: 350,
                dock_style: DockStyle.Left,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 10, 0))
                ));

            for (int a = 0; a < 4; a++)
            {
                main_panel.search_panel_control("power_panel")[0].add(new ControlButton(
                    addres: new byte[1],
                    using_name: "power_button",
                    using_text: $"Питание канал {4 - a}",
                    using_description: $"    Кнопка питания {4 - a} канала. Подает и выключает питание с соответсвующего канала питания стенда.",
                    using_button_constructor: new ControlConstructor(
                        using_color: Color.Gray,
                        using_padding: new Padding(40, 0, 0, 0)
                        ),
                    using_height: 55,
                    using_delegate: new MouseEventHandler(open_form_with_dialog),
                    dock_style: DockStyle.Top
                    ));
            }
        }

        void new_task_run()
        {
            send_control_port = new Dictionary<string, DataSending>()
            {
                //{ "DOUT16 din16", new DataSending(module_settings.dout_din16, new byte[]{ 0x00, 0x02, 0x00, 0x01, 0x00, 0x10 }) },
                //{ "DOUT16 din32", new DataSending(module_settings.dout_din32, new byte[]{ 0x00, 0x02, 0x00, 0x01, 0x00, 0x10 }) },
                //{ "DOUT16 control", new DataSending(module_settings.dout_control, new byte[]{ 0x00, 0x02, 0x00, 0x01, 0x00, 0x10 }) },
                //{ "psc", new DataSending(module_settings.psc, new byte[]{ 0x00, 0x04, 0x01, 0x0a, 0x00, 0x04 }) },
                { "rf mtu", new DataSending(module_settings.mtu5, new byte[]{ 0x00, 0x02, 0x00, 0x0f, 0x00, 0x01 }) },
                { "проверка ту и 12в mtu", new DataSending(module_settings.mtu5, new byte[]{ 0x00, 0x04, 0x01, 0x08, 0x00, 0x06 }) }
            };

            Task.Run(async ()=> {
                while (true)
                {
                    if (port_control.IsOpen)
                    {
                        foreach (DataSending ds in send_control_port.Values)
                        {
                            try
                            {
                                port_control.Transmit(ds.send_data());
                            }
                            catch (Exception error)
                            {
                                BeginInvoke((MethodInvoker)(() =>
                                {
                                    condition_tb.Text += error.Message + Environment.NewLine;
                                }));
                            }
                            await Task.Delay(1000);
                        }
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
            });
        }

        #endregion

        private void hide_control_panel(object sender, MouseEventArgs e)
        {
            foreach(ControlPanel cp in control_panel.search_panel_control())
            {
                if(((ControlButton)sender).Name == cp.Name)
                    cp.Visible = cp.Visible ? false : true;
                else
                    cp.Visible = false;
            }
        }

        private void open_form_with_dialog(object sender, MouseEventArgs e)
        {
            windows_variant win = new windows_variant(
                Cursor.Position.X - 50,
                Cursor.Position.Y - 50,
                ((ControlButton)sender).button_description,
                ((ControlButton)sender).menuing);
            win.StartPosition = FormStartPosition.Manual;
            win.Show();
        }

        void close_child_form()
        {
            main_tm_window.Visible = false;
            for(int a=1;a< child_form_panel.Controls.Count; a++)
            {
                child_form_panel.Controls[a].Dispose();
            }
        }

        void open_com_port_form(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                open_form_with_dialog(sender, e);
                return;
            }
            else
            {
                Modbus port = new Modbus();
                switch (((ControlButton)sender).Name)
                {
                    case "control_port":
                        port = port_control;
                        break;
                    case "port_chanelA":
                        port = port_chanel_a;
                        break;
                    case "port_chanelB":
                        port = port_chanel_b;
                        break;
                    case "port_chanelC":
                        port = port_chanel_c;
                        break;
                }
                close_child_form();
                ComPortOptions cpo = new ComPortOptions(port);
                child_form_panel.Controls.Add(cpo);
                cpo.Show();
                cpo.some_event += (a, b) => { condition_tb.Text += cpo.event_text + Environment.NewLine; };
                cpo.FormClosing += (se, ev) => { cpo.Dispose(); main_tm_window.Visible = true; };
            }            
        }

        void open_port(object sender, MouseEventArgs e)
        {
            Modbus port = new Modbus();
            switch (((ControlButton)sender).Name)
            {
                case "control_port":
                    port = port_control;
                    break;
                case "port_chanelA":
                    port = port_chanel_a;
                    break;
                case "port_chanelB":
                    port = port_chanel_b;
                    break;
                case "port_chanelC":
                    port = port_chanel_c;
                    break;
            }
            try
            {
                port.Open();
            }
            catch(Exception error) { condition_tb.Text += error.Message + Environment.NewLine; }
            if (port.IsOpen)
            {
                condition_tb.Text += $"  Порт {port.PortName} открыт" + Environment.NewLine;
                port.exchange_counter = 10;
            }
        }

        void close_port(object sender, MouseEventArgs e)
        {
            Modbus port = new Modbus();
            switch (((ControlButton)sender).Name)
            {
                case "control_port":
                    port = port_control;
                    break;
                case "port_chanelA":
                    port = port_chanel_a;
                    break;
                case "port_chanelB":
                    port = port_chanel_b;
                    break;
                case "port_chanelC":
                    port = port_chanel_c;
                    break;
            }
            port.Close();
            condition_tb.Text += $"  Порт {port.PortName} закрыт" + Environment.NewLine;
        }        

        public void port_control_receive(Modbus using_port)
        {
            if (using_port.data_receive[1] != 0x02 && using_port.data_receive[1] != 0x04) return;
            List<ControlButton> all_button = main_panel.search_button_control().FindAll(x => x.port_name == "Control Port");
            byte[] checkout = new byte[3] { using_port.data_transmit[0], using_port.data_transmit[2], using_port.data_transmit[3] };
            //BeginInvoke((MethodInvoker)(() =>
            //{
            //    condition_tb.Text += checkout[0].ToString("X2") + "/" + checkout[1].ToString("X2")  + Environment.NewLine;
            //    condition_tb.Text += using_port.data_transmit[0].ToString("X2") + "/" + using_port.data_transmit[1].ToString("X2") + "/" + using_port.data_transmit[2].ToString("X2") + "/" + using_port.data_transmit[3].ToString("X2") + Environment.NewLine;
            //}));

            if (using_port.data_receive[1] == 0x04) 
            {
                for (int a = 0; a < using_port.data_receive[2] / 2; a += 2) 
                {
                    for (int item = 0; item < all_button.Count; item++)
                    {
                        all_button[item].check_result(checkout, using_port.result[a / 2]);
                    }
                    checkout[1] += 2;
                }
            }
            
            //for (int a = 0; a < length; a++)
            //{

            //    for (int item = 0; item < all_button.Count; item++)
            //    {
            //        if (checkout[1] == 0x04)
            //            all_button[item].check_result(checkout, using_port.result[a]);
            //    }
            //}

        }
    }
}
