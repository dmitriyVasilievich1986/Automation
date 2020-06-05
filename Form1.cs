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
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace Automation
{
    public partial class MainForm: Form
    {
        string program_version = "Версия программы: v. 2.0.1";
        int time_from_start = 0;

        Point LastPoint;
        Settings settings_form;
        bool cntr_key_pressed = false;
        public bool continue_cycle = true;
        List<string> admin_text = new List<string>();
        List<ModuleParameters> all_addres;
        string this_directory = Directory.GetCurrentDirectory();

        public ControlPanel main_panel;
        ControlPanel control_panel;
        public ControlPanel child_form_panel;

        public TextBox condition_tb;
        TransmitionArray port1;
        TransmitionArray port2;
        TransmitionArray port3;
        TransmitionArray port4;

        public Modbus port_control = new Modbus(using_port_name: Properties.Settings.Default.port1, using_name: "Control Port", index:"1");
        public Modbus port_chanel_a = new Modbus(using_port_name: Properties.Settings.Default.port2, using_name: "Module Port", index: "2");
        public Modbus port_chanel_b = new Modbus(using_port_name: Properties.Settings.Default.port3, using_name: "Module Port", index: "3");
        public Modbus port_chanel_c = new Modbus(using_port_name: Properties.Settings.Default.port4, using_name: "Module Port", index: "4");

        public AllSettings module_settings = new AllSettings();

        Dictionary<string, DataSending> send_control_port;

        public List<Modbus> all_ports
        {
            get
            {
                return new List<Modbus>() { port_control, port_chanel_a, port_chanel_b, port_chanel_c };
            }
        }

        public MainForm()
        {
            System.Windows.Forms.Timer tim = new System.Windows.Forms.Timer();
            tim.Interval = 100;
            tim.Tick += (s, e) =>
            {
                time_from_start++;
                if (time_from_start % 36000 == 0)
                    time_from_start_show();
            };
            tim.Start();
            this.FormClosing += (s, e) => { tim.Dispose(); };
            InitializeComponent();
            list_of_all_modules();
            initialize_form_comtrol();
            control_init();
            load_condition_panel();
            condition_tb.Text += "    Форма загружена успешно" + Environment.NewLine + program_version + Environment.NewLine;
            port_initialization();
            this.KeyDown += form_key_down;
            this.KeyUp += form_key_up;
            condition_tb.TextChanged += (s, e) =>
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    condition_tb.SelectionStart = condition_tb.Text.Length;
                    condition_tb.ScrollToCaret();
                }));
                try
                {
                    System.IO.Directory.CreateDirectory(this_directory + @"\log");
                    using (StreamWriter sw = new StreamWriter(this_directory + @"\log\" + "condition.txt", false, Encoding.UTF8))
                        sw.Write(condition_tb.Text);
                }
                catch (Exception) { }
            };
            open_module_form();
            new_task_run();
        }

        string convert_time(int time)
        {
            return time < 10 ? "0" + time.ToString() : time.ToString();
        }

        void time_from_start_show()
        {
            int hours = time_from_start / 36000;
            int minutes = (time_from_start - (hours * 36000)) / 600;
            int sec = ((time_from_start - (hours * 36000)) - (minutes * 600)) / 10;
            try
            {
                condition_tb.Text += $"Программа запущена: {convert_time(hours)}:{convert_time(minutes)}:{convert_time(sec)}" + Environment.NewLine;
            }
            catch (Exception) { }
        }

        #region initialization

        void port_initialization()
        {
            port_control.receive_handler += port_control_receive;
            port_chanel_a.receive_handler += port_control_receive;
            port_chanel_b.receive_handler += port_control_receive;
            port_chanel_c.receive_handler += port_control_receive;
            port1 = new TransmitionArray(port_control);
            port2 = new TransmitionArray(port_chanel_a);
            port3 = new TransmitionArray(port_chanel_b);
            port4 = new TransmitionArray(port_chanel_c);
        }

        void initialize_form_comtrol()
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
                System.IO.Directory.CreateDirectory(this_directory + @"\JSon");
                using (StreamWriter sw = new StreamWriter(this_directory + @"\JSon\" + "all_addres.txt", false, Encoding.UTF8))
                    sw.Write(JsonConvert.SerializeObject(all_addres));
                using (StreamWriter sw = new StreamWriter(this_directory + @"\JSon\" + "module_settings.txt", false, Encoding.UTF8))
                    sw.Write(JsonConvert.SerializeObject(module_settings));
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

        void control_init()
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

            // **** tests **** //
            control_panel.add(new ControlPanel(
                dock_style: DockStyle.Top,
                using_float_height: true,
                hide_panel: false,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(28, 40, 52),
                    using_padding: new Padding(20, 0, 0, 0)),
                using_name: "tests"
                ));
            control_panel.search_panel_control("tests")[0].add(new ControlButton(
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(search_addres),
                using_name: "search_addres",
                using_text: "Найти адрес модуля"
                )); 
            foreach ((string, string) s in (new (string, string)[] { ("проверка Ping", "ping"), ("проверка Din", "din"), ("проверка kf", "kf"), ("проверка tc", "tc"), ("проверка tu", "tu"), ("Проверка питания","power"), ("Полная проверка блока","full test") }))
            {
                control_panel.search_panel_control("tests")[0].add(new ControlButton(
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(tc_test),
                using_name: s.Item2,
                using_text: s.Item1
                ));
            }
            control_panel.add(new ControlButton(
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(44, 62, 80),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "tests",
                using_text: "Проверка модуля",
                using_button_text_constructor: new ControlConstructor(using_color: Color.LightGray),
                click_mouse_handler: hide_control_panel,
                using_height: 55
                ));

            // **** settings **** //

            control_panel.add(new ControlPanel(
                dock_style: DockStyle.Top,
                using_float_height: true,
                hide_panel: false,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(28, 40, 52),
                    using_padding: new Padding(20, 0, 0, 0)),
                using_name: "settings"
                ));
            control_panel.search_panel_control("settings")[0].add(new ControlButton(
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(open_settings_form),
                using_name: "module_settings",
                using_text: "Настройки адресов модулей"
                ));
            control_panel.search_panel_control("settings")[0].add(new ControlButton(
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(open_module_settings_form),
                using_name: "module_settings",
                using_text: "Настройки модуля"
                ));
            control_panel.search_panel_control("settings")[0].add(new ControlButton(
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(power_on_off),
                using_name: "Power 220",
                using_text: "Подача 220"
                ));
            control_panel.add(new ControlButton(
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(44, 62, 80),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "settings",
                using_text: "Настройки модуля",
                using_button_text_constructor: new ControlConstructor(using_color: Color.LightGray),
                click_mouse_handler: hide_control_panel,
                using_height: 55
                ));
            
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
            add_module_button();
            control_panel.add(new ControlButton(
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(44, 62, 80),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "module",
                using_text: "Выбор модуля",
                using_button_text_constructor: new ControlConstructor(using_color: Color.LightGray),
                click_mouse_handler: hide_control_panel,
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
                menuing: new ButtonConstructor[] {
                    new ButtonConstructor(text: "Закрыть все порты", click_mouse_handler: close_all_ports),
                    new ButtonConstructor(text: "Открыть все порты", click_mouse_handler: open_all_ports),
                    new ButtonConstructor(text: "Закрыть порт", click_mouse_handler: close_port, name: port_chanel_c.index),
                    new ButtonConstructor(text: "Открыть порт", click_mouse_handler: open_port, name: port_chanel_c.index)
                },
                using_delegate: new MouseEventHandler(open_com_port_form),
                using_name: port_chanel_c.index,
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
                menuing: new ButtonConstructor[] {
                    new ButtonConstructor(text: "Закрыть все порты", click_mouse_handler: close_all_ports ),
                    new ButtonConstructor(text: "Открыть все порты", click_mouse_handler: open_all_ports),
                    new ButtonConstructor(text: "Закрыть порт", click_mouse_handler: close_port, name: port_chanel_b.index),
                    new ButtonConstructor(text: "Открыть порт", click_mouse_handler: open_port, name: port_chanel_b.index)
                },
                using_description: "Один из портов обмена проверяемого модуля. Канал B.",
                using_name: port_chanel_b.index,
                using_text: "Порт модуля канал B",
                using_height: 55
                ));
            control_panel.search_panel_control("com_ports")[0].add(new ControlButton(
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                menuing: new ButtonConstructor[] {
                new ButtonConstructor(text: "Закрыть все порты", click_mouse_handler: close_all_ports),
                new ButtonConstructor(text: "Открыть все порты",click_mouse_handler: open_all_ports),
                new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Закрыть порт",
                    using_delegate: new MouseEventHandler(close_port),
                    using_name: port_chanel_a.index,
                    using_height: 55),
                new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Открыть порт",
                    using_name: port_chanel_a.index,
                    using_delegate: new MouseEventHandler(open_port),
                    using_height: 55)
                },
                using_description: "Один из портов обмена проверяемого модуля. Канал A.",
                using_name: port_chanel_a.index,
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
                using_name: port_control.index,
                using_text: "Порт управления",
                menuing: new ButtonConstructor[] {
                new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Закрыть все порты",
                    using_delegate: new MouseEventHandler(close_all_ports),
                    using_height: 55),
                new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Открыть все порты",
                    using_delegate: new MouseEventHandler(open_all_ports),
                    using_height: 55),
                new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Закрыть порт",
                    using_delegate: new MouseEventHandler(close_port),
                    using_name: port_control.index,
                    using_height: 55),
                new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Открыть порт",
                    using_name: port_control.index,
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
                click_mouse_handler: hide_control_panel,
                using_text: "Доступные порты",
                using_height: 55
                ));

            // *** Панель HW ***
            control_panel.add(new ControlPanel(
                dock_style: DockStyle.Top,
                using_height: 100,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Gray,
                    using_padding: new Padding(50, 0, 0, 0)),
                using_name: "HW Panel"
                ));
            PictureBox HW = new PictureBox();
            HW.Image = Properties.Resources.HW3;
            HW.Size = new Size(100, 100);
            HW.Anchor = AnchorStyles.Left;
            HW.Left = 100;
            control_panel.search_panel_control("HW Panel")[0].Controls.Add(HW);
            this.Controls.Add(main_panel);
            this.Controls.Add(control_panel);
            foreach (ControlPanel cp in control_panel.search_panel_control()) cp.Visible = false;

        }

        void add_module_button()
        {
            control_panel.search_panel_control("module")[0].Controls.Clear();
            foreach (ModuleParameters mp in all_addres.OrderByDescending(x=>x.module_name))
            {
                control_panel.search_panel_control("module")[0].add(new ControlButton(
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(change_module_or_module_setup),
                using_menu: new ButtonConstructor[] {new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Сменить название модуля",
                    using_name: mp.module_name,
                    using_delegate: new MouseEventHandler(change_module_name),
                    using_height: 55),
                new ButtonConstructor(
                    text: "Новый модуль",
                    using_name: mp.module_name,
                    us_delegate: add_new_module,
                    using_height: 55),
                new ButtonConstructor(
                    using_dock_style: DockStyle.Top,
                    using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                    using_text: "Удалить модуль",
                    using_name: mp.module_name,
                    using_delegate: new MouseEventHandler(delete_module),
                    using_height: 55)
                },
                using_name: mp.module_name,
                using_text: mp.module_name
                ));
            }
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
                using_float_height: true,
                dock_style: DockStyle.Left,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 0, 10, 0))
                ));
            main_panel.search_panel_control("mtu_condition_panel")[0].add(new ControlButton(
                using_name: "psc_current",
                    using_text: "Ток потребления модуля: ",
                    value_data_sending: new CheckButtonClass(
                        module: module_settings.psc,
                        address: new byte[2] { 0x01, 0x0A },
                        port: "Control Port"),
                    using_button_constructor: new ControlConstructor(
                        using_color: Color.Gray,
                        using_padding: new Padding(40, 0, 0, 0)
                        ),
                    using_height: 55,
                    dock_style: DockStyle.Top
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
                using_name: "mtu_TC_12v",
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
                using_name: "mtu_TC_12v",
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
                        new byte[] { 0x01, 0x06, 0x00, (byte)(0x59 + a), 0x00, 0x01 }
                        ),
                    using_height: 55,
                    using_delegate: new MouseEventHandler(sending_data_button),
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
            Task.Run(async ()=> {
                while (true)
                {
                    if (port_chanel_a.IsOpen && module_settings.AllAddres.module_name != "Test" && module_settings.AllAddres.data_to_send.Count != 0)
                    {
                        foreach (DataSending ds in module_settings.AllAddres.data_to_send)
                        {
                            try
                            {
                                port_chanel_a.Transmit(ds.send_data());
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
            Task.Run(async ()=> {
                while (true)
                {
                    if (port_chanel_b.IsOpen && module_settings.AllAddres.module_name != "Test" && module_settings.AllAddres.data_to_send.Count != 0)
                    {
                        foreach (DataSending ds in module_settings.AllAddres.data_to_send)
                        {
                            try
                            {
                                port_chanel_b.Transmit(ds.send_data());
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
            Task.Run(async ()=> {
                while (true)
                {
                    if (port_chanel_c.IsOpen && module_settings.AllAddres.module_name != "Test" && module_settings.AllAddres.data_to_send.Count != 0)
                    {
                        foreach (DataSending ds in module_settings.AllAddres.data_to_send)
                        {
                            try
                            {
                                port_chanel_c.Transmit(ds.send_data());
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
                case Keys.Escape:
                    continue_cycle = false;
                    if (this.child_form_panel.Controls[0].GetType() != typeof(MainTMWindow))
                        this.open_module_form();
                    break;
                case Keys.A:
                    if (cntr_key_pressed)
                        admin_text.Add("a");
                    break;
                case Keys.D:
                    if (cntr_key_pressed)
                        admin_text.Add("d");
                    break;
                case Keys.E:
                    if (cntr_key_pressed)
                        admin_text.Add("e");
                    break;
                case Keys.I:
                    if (cntr_key_pressed)
                        admin_text.Add("i");
                    break;
                case Keys.M:
                    if (cntr_key_pressed)
                        admin_text.Add("m");
                    break;
                case Keys.N:
                    if (cntr_key_pressed)
                        admin_text.Add("n");
                    if (string.Join("", admin_text) == "admin")
                    {
                        close_all_ports(null, null);
                        cntr_key_pressed = false;
                        admin_text.Clear();
                        AllButtonTable all_button_table = new AllButtonTable(main_panel.search_button_control());
                        AllDataSendings all_data = new AllDataSendings(module_settings.AllAddres, module_settings);
                        all_button_table.Show();
                        all_data.Show();                        
                    }
                    break;
                case Keys.W:
                    if (cntr_key_pressed)
                        admin_text.Add("w");
                    if (string.Join("", admin_text) == "new")
                    {
                        cntr_key_pressed = false;
                        admin_text.Clear();
                        add_new_module(null, null);
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
                        port2.Show();
                    }
                    break;
                case Keys.C:
                    if(cntr_key_pressed)
                    {
                        port3.Show();
                    }
                    break;
                case Keys.V:
                    if(cntr_key_pressed)
                    {
                        port4.Show();
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
            using (StreamReader sw = new StreamReader(this_directory + @"\JSon\" + "all_addres.txt"))
                all_addres = JsonConvert.DeserializeObject<List<ModuleParameters>>(sw.ReadToEnd());
            using (StreamReader sw = new StreamReader(this_directory + @"\JSon\" + "module_settings.txt"))
                module_settings = JsonConvert.DeserializeObject<AllSettings>(sw.ReadToEnd());
            module_settings.AllAddres = all_addres[0];
            foreach(ModuleParameters mp in all_addres)
            {
                for(int a = 0; a < 3; a++)
                {
                    mp.kf_ind_value[a].module = module_settings.module;
                    mp.tu_ind_value[a].module = module_settings.module;
                    mp.kf_ind[a].module = module_settings.module;
                    mp.tu_ind[a].module = module_settings.module;
                }
                for(int a = 0; a < 4; a++)
                {
                    mp.power[a].module = module_settings.module;
                }
                for(int a = 0; a < 2; a++)
                {
                    mp.u_out_value[a].module = module_settings.module;
                    mp.u_out_color[a].module = module_settings.module;
                    mp.i_out_color[a].module = module_settings.module;
                    mp.i_out_value[a].module = module_settings.module;
                }
                for(int a = 0; a < 16; a++)
                {
                    mp.din16_ind_value[a].module = module_settings.module;
                    mp.din16_ind[a].module = module_settings.module;
                    mp.dout_value[a].module = module_settings.module;
                    mp.dout_color[a].module = module_settings.module;
                }
                mp.curr0_value.module = module_settings.module;
                mp.temperature_value[0].module = module_settings.module;
            }
            //foreach (ModuleParameters mp in all_addres)
            //{
            //    mp.power_min_max.name = "current";
            //    mp.power_min_max.describe = "Ток потребления: ";
            //    mp.din_min_max.name = "din";
            //    mp.din_min_max.describe = "Din min/max: ";
            //    mp.kf_min_max.name = "kf";
            //    mp.kf_min_max.describe = "kf min/max: ";
            //    mp.tc_min_max.name = "tc";
            //    mp.tc_min_max.describe = "tc min/max: ";
            //    mp.module_power_min_max.name = "module power";
            //    mp.module_power_min_max.describe = "Питание min/max: ";
            //    mp.power_port_count.name = "power ports";
            //    mp.power_port_count.describe = "Количество портов питания: ";
            //    mp.rs485_port_count.name = "rs485 ports";
            //    mp.rs485_port_count.describe = "Количество портов обмена: ";
            //}
            //foreach(ModuleParameters mp in all_addres)
            //{
            //    mp.tu.Add(new CheckButtonClass(
            //    module: new DataAddress(1),
            //        address: new byte[2] { 0, 0 },
            //        port: "Module Port"
            //        ));
            //}
            //all_addres.Add(new ModuleParameters("MTU3"));
            //all_addres.Find(x => x.module_name.Contains("MTU")).data_to_send.Clear();
            //all_addres.Find(x => x.module_name == "MTU3").data_to_send.Add(new DataSending(module_settings.module, new byte[] { 0x01, 0x02, 0x00, 0x01, 0x00, 0x12 }));
            //all_addres.Find(x => x.module_name == "MTU3").data_to_send.Add(new DataSending(module_settings.module, new byte[] { 0x01, 0x04, 0x01, 0x00, 0x00, 0x16 }));
        }

        #endregion

        public void power_on_off(object sender, MouseEventArgs e)
        {
            ControlPanel panel = control_panel.search_panel_control("HW Panel")[0];
            byte set = panel.Visible ? (byte)0x00 : (byte)0x01;
            power_set(set);
        }

        public async Task<bool> power_set(byte set)
        {
            while (port_control.interrupt_delay != 0)
            {
                await Task.Delay(100);
                if (port_control.exchange_counter >= 10)
                    return false;
            }
            port_control.set_interrupt(new byte[] { module_settings.dout_control.Addres, 0x10, 0x00, 0x51, 0x00, 0x02, 0x04, 0x00, set, 0x00, set });
            return true;
        }

        public void sending_data_button(object sender, MouseEventArgs e)
        {
            if (e != null && e.Button == MouseButtons.Right)
                open_form_with_dialog(sender, e);
            else
            {
                if (((ControlButton)sender).send_data == null) return;
                List<Modbus> ports = all_ports.FindAll(x => x.name == ((ControlButton)sender).send_data.port_name);
                byte[] send = ((ControlButton)sender).send_data.send_data();
                send[send.Length - 1] = ((ControlButton)sender).button_on_off();
                foreach (Modbus p in ports)
                    p.set_interrupt(send);
            }
        }

        public void sending_data_button_on_off(ControlButton button, byte set)
        {
            if (button.send_data == null)
                return;
            List<Modbus> ports = all_ports.FindAll(x => x.name == button.send_data.port_name);
            byte[] send = button.send_data.send_data();
            send[send.Length - 1] = set;
            foreach (Modbus p in ports)
                p.set_interrupt(send);
        }

        void hide_control_panel(ControlButton button)
        {
            foreach(ControlPanel cp in control_panel.search_panel_control())
            {
                if(cp.Name!="HW Panel")
                {
                    if (button.Name == cp.Name)
                        cp.Visible = cp.Visible ? false : true;
                    else
                        cp.Visible = false;
                }                
            }
        }

        #region open forms

        void open_form_with_dialog(object sender, MouseEventArgs e)
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

        void add_new_module(object a)
        {
            all_addres.Add(new ModuleParameters(module_name: "New module"));
            add_module_button();
            control_panel.search_panel_control("module")[0].Visible = false;
            hide_control_panel(control_panel.search_button_control("module")[0]);
        }
        
        void delete_module(ControlButton button)
        {
            all_addres.Remove(all_addres.Find(x => x.module_name == button.Name));
            add_module_button();
            control_panel.search_panel_control("module")[0].Visible = false;
            hide_control_panel(control_panel.search_button_control("module")[0]);
        }

        void change_module_or_module_setup(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && cntr_key_pressed)
            {
                cntr_key_pressed = false;
                open_form_with_dialog(sender, e);
                return;
            }
            else if (e.Button == MouseButtons.Right)
            {
                return;
            }
            else
            {
                change_module(sender, null);
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
                Modbus port = all_ports.Find(x=>x.index==((ControlButton)sender).Name);
                ComPortOptions cpo = new ComPortOptions(port, this);
                child_form_panel.Controls.Add(cpo);
                open_new_child_form(cpo);
                cpo.FormClosing += (se, ev) => { cpo.Dispose(); open_module_form(); };
            }            
        }

        public void open_module_form()
        {
            time_from_start_show();
            MainTMWindow main_tm_window = new MainTMWindow(
                module_settings: module_settings,
                mouse_event_handler: new MouseEventHandler(sending_data_button),
                port: port_control,
                main_form: this
                );
            open_new_child_form(main_tm_window);
        }

        #endregion

        #region кнопки основное управление

        void open_all_ports(object sender)
        {
            open_port(new ControlButton(using_name: "1"), null);
            open_port(new ControlButton(using_name: "2"), null);
            open_port(new ControlButton(using_name: "3"), null);
            open_port(new ControlButton(using_name: "4"), null);
        }

        void close_all_ports(object sender)
        {
            close_port(new ControlButton(using_name: "1"), null);
            close_port(new ControlButton(using_name: "2"), null);
            close_port(new ControlButton(using_name: "3"), null);
            close_port(new ControlButton(using_name: "4"), null);
        }

        public void open_port(object sender)
        {
            Modbus port = all_ports.Find(x => ((ControlButton)sender).Name.Contains(x.index));
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

        public void close_port(object sender)
        {
            Modbus port = all_ports.Find(x => ((ControlButton)sender).Name.Contains(x.index));
            port.Close();
            condition_tb.Text += $"  Порт {port.PortName} закрыт" + Environment.NewLine;
        }        

        public void port_control_receive(Modbus using_port)
        {
            if (using_port.data_receive[1] != 0x02 && using_port.data_receive[1] != 0x04) return;
            byte[] checkout = new byte[3] { using_port.data_transmit[0], using_port.data_transmit[2], using_port.data_transmit[3] };
            List<ControlButton> all_button;

            if (using_port.name == "Control Port" &&
                using_port.data_receive[0] == module_settings.dout_control.Addres &&
                (using_port.data_receive[3] & 3) != 0)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    control_panel.search_panel_control("HW Panel")[0].Visible = true;
                }));
            }                
            else if (using_port.name == "Control Port" &&
                    using_port.data_receive[0] == module_settings.dout_control.Addres &&
                    (using_port.data_receive[3] & 3) == 0)
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    control_panel.search_panel_control("HW Panel")[0].Visible = false;
                }));
            }                

            if (using_port.data_receive[1] == 0x04) 
            {
                all_button = main_panel.search_button_control().FindAll(x => x.value_data_sending != null);
                for (int a = 0; a < using_port.data_receive[2] / 2; a += 2) 
                {
                    for (int item = 0; item < all_button.Count; item++)
                    {
                        all_button[item].check_value_result(checkout, using_port.result[a / 2], using_port.name);
                    }
                    checkout[2] += 2;
                }
            }
            else if(using_port.data_receive[1] == 0x02)
            {
                UInt32 full_result = 0;
                for (int a = 0; a < using_port.data_receive[2]; a++)
                    full_result |= (UInt32)(using_port.data_receive[3 + a] << (a * 8));

                all_button = main_panel.search_button_control().FindAll(x => x.color_data_sending != null);
                for (int a = 0; a < using_port.data_receive[2] * 8; a++) 
                {
                    for (int item = 0; item < all_button.Count; item++)
                    {
                        all_button[item].check_color_result(checkout, (full_result & (1 << a)) != 0 ? 1 : 0, using_port.name);
                    }
                    checkout[2]++;
                }
            }
        }

        public void color_button(ControlButton button, int result)
        {
            button.BackColor = result == 1 ? Color.Red : button.start_color;
        }

        void change_module(object sender, MouseEventArgs e)
        {
            module_settings.AllAddres = all_addres.Find(x => x.module_name == ((ControlButton)sender).Name);
            open_module_form();
        }

        void open_settings_form(object sender, MouseEventArgs e)
        {
            Settings settings = new Settings(module_settings);
            open_new_child_form(settings);
            settings.FormClosing += (a, b) => { open_module_form(); settings.Dispose(); };
        }
        
        void open_module_settings_form(object sender, MouseEventArgs e)
        {
            ModuleSettings settings = new ModuleSettings(module_settings);
            open_new_child_form(settings);
            settings.FormClosing += (a, b) => { open_module_form(); settings.Dispose(); };
        }

        void tc_test(object sender, MouseEventArgs e)
        {
            string test_name = ((ControlButton)sender).Name;
            AllTests test = new AllTests(this);
            test.partial_test(test_name);
        }

        void change_module_name(object sender, MouseEventArgs e)
        {
            ModuleParameters module = all_addres.Find(x => x.module_name == ((ControlButton)sender).Name);
            ControlButton button = control_panel.search_button_control().Find(x => x.Name == module.module_name);

            EnterField ef = new EnterField($"  Введи новое название модуля", module.module_name);
            ef.Show();
            ef.enter_handler += (text) =>
            {
                button.Name = text;
                button.Text = text;
                module.module_name = text;
                ef.Dispose();
            };
        }

        async void search_addres(object sender, MouseEventArgs e)
        {
            continue_cycle = true;
            for (module_settings.module.Addres = 0x01; module_settings.module.Addres < 0xfa; module_settings.module.Addres++) 
            {
                condition_tb.Text += $"Устанавливаем аддрес модуля: {module_settings.module.Addres}" + Environment.NewLine;
                if (!continue_cycle)
                {
                    condition_tb.Text += $"Прерывание..." + Environment.NewLine;
                    break;
                }
                await Task.Delay(200);
                if (port_chanel_a.exchange_counter < 10 || port_chanel_b.exchange_counter < 10 || port_chanel_c.exchange_counter < 10)
                    break;
            }
        }

        #endregion
    }
}
