﻿using System;
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
    public partial class main_form : Form
    {
        Point LastPoint;
        Settings settings_form;

        ControlPanel main_panel;
        ControlPanel control_panel;

        TextBox condition_tb;

        #region initialization

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
            btn_close_application.MouseClick += (s, e) => { Application.Exit(); };
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
                        using_color: Color.FromArgb(28, 40, 52))
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
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "test",
                using_text: "test",
                using_height: 55
                ));
            control_panel.search_panel_control("module")[0].add(new ControlButton(
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "test",
                using_text: "test",
                using_height: 55
                ));
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
                using_name: "test",
                using_text: "test",
                using_height: 55
                ));
            control_panel.search_panel_control("com_ports")[0].add(new ControlButton(
                dock_style: DockStyle.Top,
                using_button_constructor: new ControlConstructor(
                    using_color: Color.FromArgb(113, 125, 137),
                    using_padding: new Padding(40, 0, 0, 0)),
                using_name: "test",
                using_text: "test",
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
        }

        void all_form_load()
        {
            settings_form = new Settings();
            main_panel.Controls.Add(settings_form);
            settings_form.Show();            
        }

        void load_condition_panel()
        {
            main_panel.add(new ControlPanel(
                using_name: "condition_panel",
                using_height: 220,
                using_panel_constructor: new ControlConstructor(
                    using_padding: new Padding(0, 0, 0, 0))
                ));

            condition_tb = new TextBox();
            condition_tb.Enabled = false;
            condition_tb.Dock = DockStyle.Fill;
            condition_tb.Multiline = true;
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
                    using_padding: new Padding(0, 0, 10, 0))
                ));
            main_panel.search_panel_control("mtu_condition_panel")[0].add(new ControlButton(
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
                    using_height: 55,
                    using_delegate: new MouseEventHandler(open_form_with_dialog),
                    dock_style: DockStyle.Top
                    ));
            }
        }

        #endregion

        public main_form()
        {
            InitializeComponent();
            initialize_form_comtrol();
            control_init();
            all_form_load();
            load_condition_panel();
            condition_tb.Text += "    Форма зугружена успешно" + Environment.NewLine;
        }

        private void hide_control_panel(object sender, MouseEventArgs e)
        {
            foreach(ControlPanel cp in control_panel.search_panel_control())
            {
                if(((ControlButton)sender).Name == cp.Name)
                {
                    if(cp.Visible)
                        cp.Visible = false;
                    else
                        cp.Visible = true;
                }                    
                else
                    cp.Visible = false;
            }
        }

        private void open_form_with_dialog(object sender, MouseEventArgs e)
        {
            windows_variant win = new windows_variant(Cursor.Position.X - 50, Cursor.Position.Y - 50, ((ControlButton)sender).button_description);
            win.StartPosition = FormStartPosition.Manual;
            win.Show();
        }
    }
}