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

namespace Automation
{
    public partial class ComPortOptions : Form
    {
        Modbus port;
        ControlPanel main_panel;

        public event EventHandler some_event;
        public string event_text = "";

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
            port_exchange();
        }

        void port_exchange()
        {
            Task.Run(async()=> {
                while (!this.IsDisposed)
                {
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        main_panel.search_button_control("com_port_addres")[0].BackColor = port.exchange_counter < 5 && port.IsOpen ? Color.Green : Color.FromArgb(113, 125, 137);
                        main_panel.search_button_control("com_port_close")[0].Visible = port.IsOpen ? true : false;
                        main_panel.search_button_control("com_port_open")[0].Visible = port.IsOpen ? false : true;
                    }));
                    await Task.Delay(100);
                }
            });            
        }

        void control_panel_add()
        {
            this.main_panel = new ControlPanel(
                using_name: "main_panel",
                dock_style: DockStyle.Fill,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    ));
            
            // **** Кнопки управления com порта**** //
            this.main_panel.add(new ControlPanel(
                using_name: "com_port_buttons",
                dock_style: DockStyle.Left,
                using_width: 350,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 50, 10, 0)
                    )));
            this.main_panel.search_panel_control("com_port_buttons")[0].add(new ControlButton(
                addres: new byte[1],
                using_name: "com_port_open",
                using_delegate: new MouseEventHandler(open_port),
                using_text: "Открыть порт",
                hide_panel: port.IsOpen ? false : true,
                using_height: 55,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0))
                ));
            this.main_panel.search_panel_control("com_port_buttons")[0].add(new ControlButton(
                addres: new byte[1],
                using_name: "com_port_close",
                using_text: "Закрыть порт",
                using_delegate: new MouseEventHandler(close_port),
                hide_panel: port.IsOpen ? true : false,
                using_height: 55,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0))
                ));
            this.main_panel.search_panel_control("com_port_buttons")[0].add(new ControlButton(
                addres: new byte[1],
                using_name: "com_port_addres",
                using_text: port.PortName,
                using_delegate: new MouseEventHandler(choose_com_port_name),
                using_height: 55,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0))
                ));

            // **** кнопка выхода из меню **** //
            this.main_panel.add(new ControlPanel(
                using_name: "back_button_panel",
                dock_style: DockStyle.Top,
                using_height: 55,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    )));
            this.main_panel.search_panel_control("back_button_panel")[0].add(new ControlButton(
                addres: new byte[1],
                dock_style: DockStyle.Left,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_delegate: new MouseEventHandler(close_form),
                using_name: "exit",
                using_text: "Выход",
                using_width: 340
                ));
            this.Controls.Add(main_panel);
        }

        void close_form(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        void choose_com_port_name(object sender, MouseEventArgs e)
        {
            EnterField ef = new EnterField("  Введи имя com порта", SerialPort.GetPortNames(), port.PortName);
            ef.Show();
            ef.FormClosing += (se, ev) => {
                if (ef.input_cb.Text == "")
                {
                    ef.Dispose();
                    return;
                }
                try
                {
                    port.PortName = ef.input_cb.Text;
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        this.main_panel.search_button_control("com_port_addres")[0].Text = port.PortName;
                    }));
                }
                catch (Exception) { }
                ef.Dispose();
            };
        }

        void open_port(object sender, MouseEventArgs e)
        {
            try
            {
                port.Open();                
            }
            catch (Exception error)
            {
                event_text = error.Message;
                some_event.Invoke(this, e);
                event_text = "";
            }
            if (port.IsOpen)
            {
                event_text = $"  Порт {port.PortName} открыт";
                some_event.Invoke(this, e);
                port.exchange_counter = 10;
            }
        }

        void close_port(object sender, MouseEventArgs e)
        {
            port.Close();
            event_text = $"  Порт {port.PortName} закрыт";
            some_event.Invoke(this, e);
        }
    }
}
