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
    public partial class ModuleSettings : Form
    {
        ControlPanel main_panel;
        AllSettings module_settings;

        public ModuleSettings(AllSettings module_settings)
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
                using_name: "Min Max Panel",
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

        void settings_panel_fill()
        {
            for (int a = 0; a < module_settings.all_min_max.Count; a++)
            {
                main_panel.search_panel_control("Min Max Panel")[0].add(new ControlButton(
                using_name: "min_max" + module_settings.all_min_max[a].name,
                using_text: $"{module_settings.all_min_max[a].describe}",
                start_text: module_settings.all_min_max[a].min_max_get(),
                using_delegate: new MouseEventHandler(change_min_max)
                ));
            }
        }

        void change_min_max(object sender, MouseEventArgs e)
        {
            MinMaxNone min_max = module_settings.all_min_max.Find(x => ("min_max" + x.name) == (((ControlButton)sender).Name));

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
