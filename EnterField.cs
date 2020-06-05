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
        public delegate void EnterFormHandler(string text);
        public event EnterFormHandler enter_handler;
        public delegate void WithoutChangeHandler();
        public event WithoutChangeHandler without_change_handler;

        public EnterField(string text, String input)
        {
            InitializeComponent();
            enter_text_field.Text = text;
            input_tb.Text = input;
            input_cb.Visible = false;
            input_tb.Focus();
            input_tb.SelectAll();
            this.KeyDown += (s, e) => {
                if(e.KeyCode == Keys.Enter)
                {
                    if (input_tb.Text == "")
                    {
                        without_change_handler?.Invoke();
                        Dispose();
                    }
                    else
                        enter_handler.Invoke(input_tb.Text);
                }
                else if(e.KeyCode == Keys.Escape)
                {
                    input_tb.Text = "";
                    without_change_handler?.Invoke();
                    Close();
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
                    enter_handler?.Invoke(input_tb.Text);
                    this.Close();
                }
                else if(e.KeyCode == Keys.Escape)
                {
                    input_cb.Text = "";
                    without_change_handler?.Invoke();
                    this.Close();
                }
            };
        }
    }
}
