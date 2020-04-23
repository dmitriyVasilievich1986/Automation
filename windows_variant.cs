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
    public partial class windows_variant : Form
    {
        ControlPanel main_panel;
        TextBox description_textbox;

        private void initialzitaion_main_panel()
        {
            description_textbox = new TextBox();
            description_textbox = new TextBox();
            description_textbox.Dock = DockStyle.Top;
            description_textbox.Height = 150;
            description_textbox.Multiline = true;
            description_textbox.Enabled = false;
            description_textbox.ScrollBars = ScrollBars.Vertical;
            description_textbox.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 204);

            main_panel = new ControlPanel(
                dock_style: DockStyle.Fill,
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.White
                    ));
            main_panel.add(new ControlPanel(
                using_name: "main_panel",
                dock_style: DockStyle.Fill,
                using_float_height: true,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Red
                    )));
            main_panel.Controls.Add(description_textbox);
            this.Controls.Add(main_panel);
        }

        public windows_variant(int x, int y, string describe, ButtonConstructor[] menu)
        {
            InitializeComponent();
            initialzitaion_main_panel();
            description_textbox.Text = describe;
            this.Left = x;
            this.Top = y;
            this.MouseEnter += (s, e) => { this.Dispose(); };
            if (menu != null)
            {
                foreach(ButtonConstructor butcon in menu)
                {
                    main_panel.search_panel_control("main_panel")[0].add(new ControlButton(butcon));
                }
                this.Height = description_textbox.Height + Padding.Top + Padding.Bottom + main_panel.search_button_control().Count * 55;
            }
            else this.Height = description_textbox.Height + Padding.Top + Padding.Bottom;
        }
    }
}
