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
    public partial class EnterField : Form
    {
        public EnterField(string text, String input)
        {
            InitializeComponent();
            enter_text_field.Text = text;
            input_tb.Text = input;
            input_cb.Visible = false;
            input_tb.MouseDown += (s, e) => { if (input_tb.Text == input) input_tb.Text = ""; };
            this.KeyDown += (s, e) => {
                if(e.KeyCode == Keys.Enter)
                {
                    if (input_tb.Text == input)
                    {
                        input_tb.Text = "";
                        Dispose();
                    }
                }
                else if(e.KeyCode == Keys.Escape)
                {
                    input_tb.Text = "";
                    Dispose();
                }
            };
        }

        public EnterField(string text, String[] input, string input_text)
        {
            InitializeComponent();
            input_cb.Items.AddRange(input);
            enter_text_field.Text = text;
            input_cb.Text = input_text;
            input_tb.Visible = false;
            this.KeyDown += (s, e) => {
                if(e.KeyCode == Keys.Enter)
                {
                    this.Close();
                }
                else if(e.KeyCode == Keys.Escape)
                {
                    input_cb.Text = "";
                    this.Close();
                }
            };
        }
    }
}
