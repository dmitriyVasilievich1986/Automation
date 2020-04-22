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
        public MainTMWindow()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopLevel = false;
            this.BringToFront();
            this.Padding = new Padding(0, 10, 0, 0);
            this.Controls.Add(new ControlPanel(
                dock_style: DockStyle.Top,
                using_height: 100,
                using_panel_constructor: new ControlConstructor(
                    using_color: Color.Red
                    )));
        }
    }
}
