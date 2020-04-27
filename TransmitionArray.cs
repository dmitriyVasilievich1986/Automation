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
    public partial class TransmitionArray : Form
    {
        List<string> Data_Update = new List<string>();
        public TransmitionArray(Modbus port)
        {
            InitializeComponent();

            FormClosing += (s, e) => { Dispose(); };

            port.receive_handler += (p) =>
            {
                if (this.Visible)
                {
                    Data_Update.Add(port.receive_array);
                    while (Data_Update.Count > 250) Data_Update.RemoveAt(0);
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        condition_box.Lines = Data_Update.ToArray();
                        condition_box.SelectionStart = condition_box.Text.Length;
                        condition_box.ScrollToCaret();
                    }));
                }
            };
            port.transmit_handler += (p) =>
            {
                if (this.Visible)
                {
                    Data_Update.Add(port.transmit_array);
                    while (Data_Update.Count > 250) Data_Update.RemoveAt(0);
                    BeginInvoke((MethodInvoker)(() =>
                    {
                        condition_box.Lines = Data_Update.ToArray();
                        condition_box.SelectionStart = condition_box.Text.Length;
                        condition_box.ScrollToCaret();
                    }));
                }

            };
        }
    }
}
