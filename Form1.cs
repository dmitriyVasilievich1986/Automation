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
        bool cntr_key_pressed = false;
        List<string> admin_text = new List<string>();
        List<ModuleParameters> all_addres;

        ControlPanel main_panel;
        ControlPanel control_panel;
        ControlPanel child_form_panel;

        TextBox condition_tb;
        TransmitionArray port1;
        TransmitionArray port2;

        Modbus port_control = new Modbus(using_port_name: Properties.Settings.Default.port1, using_name: "Control Port");
        Modbus port_chanel_a = new Modbus(using_port_name: Properties.Settings.Default.port2, using_name: "Module Port");
        Modbus port_chanel_b = new Modbus(using_port_name: Properties.Settings.Default.port3, using_name: "Module Port");
        Modbus port_chanel_c = new Modbus(using_port_name: Properties.Settings.Default.port4, using_name: "Module Port");
        AllSettings module_settings = new AllSettings();

        Dictionary<string, DataSending> send_control_port;

        public MainForm()
        {
            InitializeComponent();
            list_of_all_modules();
            initialize_form_comtrol();
            control_init();
            load_condition_panel();
            condition_tb.Text += "    Форма загружена успешно" + Environment.NewLine;
            new_task_run();
            port_initialization();
            this.KeyDown += form_key_down;
            this.KeyUp += form_key_up;
            condition_tb.TextChanged += (s, e) =>
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    //condition_tb.Lines = Data_Update.ToArray();
                    condition_tb.SelectionStart = condition_tb.Text.Length;
                    condition_tb.ScrollToCaret();
                }));
            };
            open_module_form();
        }

        #region initialization

        void port_initialization()
        {
            port_control.receive_handler += port_control_receive;
            port1 = new TransmitionArray(port_control);
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
            foreach(ModuleParameters mp in all_addres)
            {
                control_panel.search_panel_control("module")[0].add(new ControlButton(
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(change_module),
                using_name: mp.module_name,
                using_text: mp.module_name
                ));
            }
            control_panel.add(new ControlButton(
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
            foreach (ControlPanel cp in control_panel.search_panel_control()) cp.Visible = false;
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
            condition_tb.Enabled = false;
            condition_tb.ScrollBars = ScrollBars.Vertical;
            condition_tb.BackColor = Color.LightGray;
            condition_tb.ForeColor = Color.Black;            
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
                using_name: "mtu_220v_tu",
                    using_text: "Напряжение ТУ: ",
                    value_data_sending: new CheckButtonClass(
                        module: module_settings.mtu5,
                        address: new byte[2] { 0x01, 0x08 },
                        port: "Control Port"),
                    using_button_constructor: new ControlConstructor(
                        using_color: Color.Gray,
                        using_padding: new Padding(40, 0, 0, 0)
                        ),
                    using_height: 55,
                    dock_style: DockStyle.Top
                ));
            main_panel.search_panel_control("mtu_condition_panel")[0].add(new ControlButton(
                using_name: "mtu_tc_12v",
                using_text: "ТС 12В канал 2: ",
                value_data_sending: new CheckButtonClass(
                        module: module_settings.mtu5,
                        address: new byte[2] { 0x01, 0x0c },
                        port: "Control Port"),
                using_button_constructor: new ControlConstructor(
                    using_color: Color.Gray,
                    using_padding: new Padding(40, 0, 0, 0)
                    ),
                using_height: 55,
                dock_style: DockStyle.Top
                ));
            main_panel.search_panel_control("mtu_condition_panel")[0].add(new ControlButton(
                value_data_sending: new CheckButtonClass(
                    module: module_settings.mtu5,
                    address: new byte[2] { 0x01, 0x0a},
                    port: "Control Port"),     
                using_name: "mtu_tc_12v",
                using_text: "ТС 12В канал 1: ",
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
                    using_name: "power_button",
                    using_text: $"Питание канал {4 - a}",
                    using_description: $"    Кнопка питания {4 - a} канала. Подает и выключает питание с соответсвующего канала питания стенда.",
                    using_button_constructor: new ControlConstructor(
                        using_color: Color.Gray,
                        using_padding: new Padding(40, 0, 0, 0)
                        ),
                    color_data_sending: new CheckButtonClass(
                    module: module_settings.dout_control,
                    address: new byte[2] { 0x00, (byte)(0x09 + a) },                    
                    port: "Control Port"
                    ),
                    send_data: new DataSending(
                        using_addres: module_settings.dout_control,
                        new byte[] { 0x01, 0x06, 0x00, (byte)(0x59+a), 0x00, 0x01}
                        ),
                    using_height: 55,
                    using_delegate: new MouseEventHandler(port_control_sending_data_button),
                    dock_style: DockStyle.Top
                    ));
            }
        }

        void new_task_run()
        {
            send_control_port = new Dictionary<string, DataSending>()
            {
                { "DOUT16 din16", new DataSending(module_settings.dout_din16, new byte[]{ 0x00, 0x02, 0x00, 0x01, 0x00, 0x10 }) },
                { "DOUT16 din32", new DataSending(module_settings.dout_din32, new byte[]{ 0x00, 0x02, 0x00, 0x01, 0x00, 0x10 }) },
                { "DOUT16 control", new DataSending(module_settings.dout_control, new byte[]{ 0x00, 0x02, 0x00, 0x01, 0x00, 0x10 }) },
                { "psc", new DataSending(module_settings.psc, new byte[]{ 0x00, 0x04, 0x01, 0x0a, 0x00, 0x04 }) },
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
                            await Task.Delay(100);
                        }
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
            });
        }

        void form_key_down(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    cntr_key_pressed = true;
                    break;
                case Keys.A:
                    if (cntr_key_pressed)
                        admin_text.Add("a");
                    break;
                case Keys.D:
                    if (cntr_key_pressed)
                        admin_text.Add("d");
                    if (string.Join("", admin_text) == "ad")
                    {
                        cntr_key_pressed = false;
                        AllButtonTable all_button_table = new AllButtonTable(main_panel.search_button_control());
                        all_button_table.Show();
                    }
                    break;
                case Keys.Z:
                    if(cntr_key_pressed)
                    {
                        port1.Show();
                        //if (port1 == null)
                        //{
                        //    port1 = new TransmitionArray(port_control);
                        //    port1.FormClosing += (a, b) => { port1 = null; };
                        //    port1.Show();
                        //}
                    }
                    break;
                case Keys.X:
                    if(cntr_key_pressed)
                    {
                        if (port2 == null)
                        {
                            port2 = new TransmitionArray(port_chanel_a);
                            port2.FormClosing += (a, b) => { port2 = null; };
                            port2.Show();
                        }
                    }
                    break;
                default:
                    admin_text.Clear();
                    break;
            }
        }

        void form_key_up(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    cntr_key_pressed = false;
                    admin_text.Clear();
                    break;
            }
        }

        void list_of_all_modules()
        {
            all_addres = new List<ModuleParameters>();
            all_addres.Add(new ModuleParameters("Test"));
            all_addres.Add(new ModuleParameters("RTU5"));
            for(int a = 0; a < 16; a++)
            {
                all_addres.Find(x => x.module_name == "Test").din16[a].address = new byte[2] { 0, (byte)(a+1) };
                all_addres.Find(x => x.module_name == "RTU5").din16[a].address = new byte[2] { 0, (byte)(a+1) };
            }
            for (int a = 0; a < 3; a++)
            {
                all_addres.Find(x => x.module_name == "Test").kf[a].address = new byte[2] { 0, (byte)(a+1) };
                all_addres.Find(x => x.module_name == "RTU5").kf[a].address = new byte[2] { 0, (byte)(a+1) };
                all_addres.Find(x => x.module_name == "Test").tc[a].address = new byte[2] { 0, (byte)(a+1) };
                all_addres.Find(x => x.module_name == "RTU5").tc[a].address = new byte[2] { 0, (byte)(a+1) };
            }
            module_settings.AllAddres = all_addres[0];
        }

        #endregion

        void port_control_sending_data_button(object sender, MouseEventArgs e)
        {
            if (((ControlButton)sender).send_data == null) return;
            byte[] send = ((ControlButton)sender).send_data.send_data();
            send[send.Length - 1] = ((ControlButton)sender).button_on_off();
            port_control.set_interrupt(send);
        }

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

        #region open forms

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

        void open_new_child_form(Form new_child_form)
        {
            for (int a = 0; a < child_form_panel.Controls.Count; a++)
                child_form_panel.Controls[a].Dispose();
            child_form_panel.Controls.Add(new_child_form);
            new_child_form.Show();
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
                ComPortOptions cpo = new ComPortOptions(port);
                child_form_panel.Controls.Add(cpo);
                open_new_child_form(cpo);
                cpo.some_event += (a, b) => { condition_tb.Text += cpo.event_text + Environment.NewLine; };
                cpo.FormClosing += (se, ev) => { cpo.Dispose(); open_module_form(); };
            }            
        }

        void open_module_form()
        {
            MainTMWindow main_tm_window = new MainTMWindow(
                module_settings: module_settings,
                mouse_event_handler: new MouseEventHandler(port_control_sending_data_button)
                );
            open_new_child_form(main_tm_window);
        }

        #endregion

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
            byte[] checkout = new byte[3] { using_port.data_transmit[0], using_port.data_transmit[2], using_port.data_transmit[3] };
            List<ControlButton> all_button;

            if (using_port.data_receive[1] == 0x04) 
            {
                all_button = main_panel.search_button_control().FindAll(x => x.value_data_sending != null && x.value_data_sending.port == "Control Port");
                for (int a = 0; a < using_port.data_receive[2] / 2; a += 2) 
                {
                    for (int item = 0; item < all_button.Count; item++)
                    {
                        all_button[item].check_value_result(checkout, using_port.result[a / 2]);
                    }
                    checkout[2] += 2;
                }
            }
            else if(using_port.data_receive[1] == 0x02)
            {
                all_button = main_panel.search_button_control().FindAll(x => x.color_data_sending != null && x.color_data_sending.port == "Control Port");
                for (int loop1 = 0; loop1 < using_port.data_receive[2]; loop1++)
                {
                    for (int loop2 = 0; loop2 < 2; loop2++)
                    {
                        for(int loop3 = 0; loop3 < 4; loop3++)
                        {
                            for (int item = 0; item < all_button.Count; item++)
                            {
                                all_button[item].check_color_result(checkout, using_port.data_receive[loop1 + 3] & (1 << (loop3 + loop2 * 4)));
                            }
                            checkout[2] += 1;
                        }
                    }
                }
            }
        }

        public void color_button(ControlButton button, int result)
        {
            button.BackColor = result == 1 ? Color.Red : button.start_color;
        }

        void change_module(object sender, MouseEventArgs e)
        {
            module_settings.AllAddres = all_addres.Find(x => x.module_name == (((ControlButton)sender).Name));
            open_module_form();
        }
    }
}
