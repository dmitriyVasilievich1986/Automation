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
    public partial class AllDataSendings : Form
    {
        public AllDataSendings(ModuleParameters ms, AllSettings allset)
        {
            InitializeComponent();
            DataTable buttons = new DataTable("All settings");
            int length = 0;
            for(int a=0; a < 6; a++)
            {
                buttons.Columns.Add($"Данные бит {a + 1}", typeof(byte));
            }

            foreach (DataSending ds in ms.data_to_send)
            {
                DataRow dr = buttons.NewRow();
                for(int a = 0; a < ds.data.Length; a++)
                {
                    dr[$"Данные бит {a + 1}"] = ds.data[a];
                }
                dr[$"Данные бит 1"] = ds.address.Addres;
                buttons.Rows.Add(dr);
            }
            dataGridView1.DataSource = buttons;
            
            
            DataTable tests = new DataTable("All tests");
            tests.Columns.Add("Название проверки", typeof(string));
            tests.Columns.Add("провести проверку", typeof(bool));

            foreach(TestMethod tm in ms.all_tests)
            {
                DataRow dr = tests.NewRow();
                dr["Название проверки"] = tm.name;
                dr["провести проверку"] = tm.test;
                tests.Rows.Add(dr);
            }
            dataGridView2.DataSource = tests;

            this.FormClosing += (s, e) =>
            {
                for(int a=0;a< ms.all_tests.Count; a++)
                {
                    ms.all_tests[a].name = tests.Rows[a].Field<string>("Название проверки");
                    ms.all_tests[a].test = tests.Rows[a].Field<bool>("провести проверку");
                }
                ms.data_to_send.Clear();
                for(int row = 0; row < buttons.Rows.Count; row++)
                {
                    ms.data_to_send.Add(new DataSending(allset.module, new byte[6]));
                    for (int a = 0; a < 6; a++)
                    {
                        ms.data_to_send[row].data[a] = buttons.Rows[row].Field<byte>($"Данные бит {a + 1}");
                    }
                }                    
                this.Dispose();
            };
        }
    }
}
