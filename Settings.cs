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
    public partial class Settings : Form
    {
        ControlPanel main_panel;
        AllSettings module_settings;

        public Settings(AllSettings module_settings)
        {
            this.module_settings = module_settings;
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.BringToFront();
            this.Padding = new Padding(0, 10, 0, 0);
            this.BackColor = Color.Black;
            control_panel_add();
            exit_panel_fill();
            settings_panel_fill();
            all_button_style();
        }

        void control_panel_add()
        {
            main_panel = new ControlPanel(
                using_name: "main_panel",
                dock_style: DockStyle.Fill,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    ));
            main_panel.add(new ControlPanel(
                using_name: "Settings Panel",
                dock_style: DockStyle.Left,
                using_width: 350,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black,
                    using_padding: new Padding(0, 10, 10, 0)
                    )));
            main_panel.add(new ControlPanel(
                using_name: "Exit",
                dock_style: DockStyle.Top,
                using_height: 55,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Black
                    )));
            this.Controls.Add(main_panel);
        }

        void exit_panel_fill()
        {
            main_panel.search_panel_control("Exit")[0].add(new ControlButton(
                using_name: "Exit",
                using_text: "Назад",
                dock_style: DockStyle.Left,
                using_width: 340,
                using_delegate: new MouseEventHandler(close_form)
                ));
        }

        void settings_panel_fill()
        {
            for(int a=0;a< module_settings.all_modules_addres.Count; a++)
            {
                main_panel.search_panel_control("Settings Panel")[0].add(new ControlButton(
                using_name: module_settings.all_modules_addres[a].name,
                using_text: $"Адрес {module_settings.all_modules_addres[a].name}: ",
                start_text: module_settings.all_modules_addres[a].Addres.ToString(),
                using_delegate: new MouseEventHandler(change_addres)
                ));
            }
        }

        void all_button_style()
        {
            foreach (ControlButton cb in main_panel.search_button_control())
            {
                cb.BackColor = Color.Gray;
                cb.start_color = Color.Gray;
                cb.Padding = new Padding(40, 0, 0, 0);
            }
        }

        void close_form(object sender, MouseEventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        void change_addres(object sender, MouseEventArgs e)
        {
            DataAddress address = module_settings.all_modules_addres.Find(x => x.name == (((ControlButton)sender).Name));

            EnterField ef = new EnterField("  Введи новый адрес", address.Addres.ToString());
            ef.Show();            
            ef.enter_handler += (text) =>
            {
                try
                {
                    address.Addres = byte.Parse(text);
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        ((ControlButton)sender).Set_Text = text;
                    }));
                }
                catch (Exception) { }
                ef.Dispose();
            };
        }
    }
}
