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
    public partial class AllButtonTable : Form
    {
        public AllButtonTable(List<ControlButton> all_button)
        {
            InitializeComponent();
            DataTable buttons = new DataTable("All buttons");
            buttons.Columns.Add("Название кнопки", typeof(string));
            buttons.Columns.Add("Текст кнопки", typeof(string));
            buttons.Columns.Add("button text value", typeof(string));
            buttons.Columns.Add("port name value", typeof(string));
            buttons.Columns.Add("value modul:", typeof(byte));
            buttons.Columns.Add("value addres hi:", typeof(byte));
            buttons.Columns.Add("value addres lo:", typeof(byte));
            buttons.Columns.Add("port name color", typeof(string));
            buttons.Columns.Add("color modul:", typeof(byte));
            buttons.Columns.Add("color addres hi:", typeof(byte));
            buttons.Columns.Add("color addres lo:", typeof(byte));
            buttons.Columns.Add("data send:", typeof(string));

            
            foreach(ControlButton cb in all_button)
            {
                DataRow dr = buttons.NewRow();
                dr["Название кнопки"] = cb.Name;
                dr["Текст кнопки"] = cb.Text;
                string data = "";
                if (cb.send_data != null)
                {
                    foreach (byte b in cb.send_data.send_data())
                    {
                        data += b.ToString("X2") + " ";
                    }
                }                
                dr["data send:"] = data;
                if (cb.value_data_sending != null)
                {
                    dr["button text value"] = cb.value_data_sending.button_text;
                    dr["port name value"] = cb.value_data_sending.port;
                    dr["value modul:"] = cb.value_data_sending.module.Addres;
                    dr["value addres hi:"] = cb.value_data_sending.address[0];
                    dr["value addres lo:"] = cb.value_data_sending.address[1];
                }
                if (cb.color_data_sending != null)
                {
                    dr["port name color"] = cb.color_data_sending.port;
                    dr["color modul:"] = cb.color_data_sending.module.Addres;
                    dr["color addres hi:"] = cb.color_data_sending.address[0];
                    dr["color addres lo:"] = cb.color_data_sending.address[1];
                }
                buttons.Rows.Add(dr);
            }
            dataGridView1.DataSource = buttons;

            FormClosing += (s,e)=> {
                for(int a = 0; a < all_button.Count; a++)
                {
                    all_button[a].Name = buttons.Rows[a].Field<string>("Название кнопки");
                    all_button[a].Text = buttons.Rows[a].Field<string>("Текст кнопки");
                    if (all_button[a].value_data_sending != null)
                    {
                        all_button[a].value_data_sending.button_text = buttons.Rows[a].Field<string>("button text value");
                        all_button[a].value_data_sending.port = buttons.Rows[a].Field<string>("port name value");
                        all_button[a].value_data_sending.address[0] = buttons.Rows[a].Field<byte>("value addres hi:");
                        all_button[a].value_data_sending.address[1] = buttons.Rows[a].Field<byte>("value addres lo:");
                    }
                    if (all_button[a].color_data_sending != null)
                    {
                        all_button[a].color_data_sending.port = buttons.Rows[a].Field<string>("port name color");
                        all_button[a].color_data_sending.address[0] = buttons.Rows[a].Field<byte>("color addres hi:");
                        all_button[a].color_data_sending.address[1] = buttons.Rows[a].Field<byte>("color addres lo:");
                    }
                }
            };
        }
    }
}
