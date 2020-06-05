using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace Automation
{
    public partial class AllTests : Form
    {
        MainForm main_form;
        int time_test = 0;
        int module_number = 1;
        bool number_is_get = false;

        public AllTests(MainForm main_form)
        {
            this.main_form = main_form;
            InitializeComponent();
            this.FormClosing += (s, e) => { this.Dispose(); };
            this.button1.Visible = false;
        }

        void save_result_in_db(List<Result> results)
        {
            if (!MysqlWork.try_get_connection(main_form.module_settings.AllAddres.module_name))
                return;
            if (MysqlWork.select_module(main_form.module_settings.AllAddres.module_name) == 0)
                    MysqlWork.insert_module(main_form.module_settings.AllAddres.module_name);
            MysqlWork.insert_module_result(main_form.module_settings.AllAddres.module_name, module_number, results);
            this.Close();
        }

        #region формирование таблицы

        void data_grid_draw(List<Result> list_of_all_result)
        {
            DataTable module = new DataTable("module");
            module.Columns.Add("Название модуля", typeof(string));
            module.Columns.Add("Номер модуля", typeof(int));
            module.Columns.Add("Время выполнения", typeof(string));
            DataRow dr = module.NewRow();
            dr["Название модуля"] = main_form.module_settings.AllAddres.module_name;
            dr["Номер модуля"] = module_number;
            int sec = time_test / 1000;
            dr["Время выполнения"] = $"{sec} сек. {time_test - sec*1000} м.сек";
            module.Rows.Add(dr);
            this.dataGridView2.DataSource = module;

            DataTable all_result = new DataTable("All result");
            all_result.Columns.Add("Название Теста", typeof(string));
            int length = 0;
            foreach (Result r in list_of_all_result)
                if (r.all_test_count > length) length = r.all_test_count;
            for(int a = 0; a < length; a++)
            {
                all_result.Columns.Add($"Данные {a + 1}", typeof(string));
            }
            foreach(Result result in list_of_all_result)
            {
                dr = all_result.NewRow();
                dr["Название Теста"] = result.test_name;
                all_result.Rows.Add(dr);
                foreach (Test test in result.all_test)
                {
                    dr = all_result.NewRow();
                    dr["Название Теста"] = test.test_name;
                    for (int a = 0; a < test.test.Count; a++)
                    {
                        dr[$"Данные {a + 1}"] = test.test[a].Item1 == 0 ? test.test[a].Item2 ? "Ok" : "Not Ok" : test.test[a].Item1.ToString();
                    }
                    all_result.Rows.Add(dr);
                }
            }
            
            this.dataGridView1.DataSource = all_result;
            this.Show();
            Colorized(list_of_all_result);
            this.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Escape)
                    this.Close();
                else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                    save_result_in_db(list_of_all_result);
            };
            this.button1.MouseDown += (s, e) => { save_result_in_db(list_of_all_result); };
        }

        async void Colorized(List<Result> list_of_all_result)
        {
            await Task.Delay(100);
            int row = 0;
            foreach(Result result in list_of_all_result)
            {
                while (this.dataGridView1.Rows[row].Cells[0].Value != result.test_name) row++;
                this.dataGridView1.Rows[row].Cells[0].Style.BackColor = result.is_test_norm ? Color.Green : Color.Red;
                foreach (Test test in result.all_test)
                {
                    row++;
                    for (int a = 0; a < test.test.Count; a++)
                    {
                        this.dataGridView1.Rows[row].Cells[a + 1].Style.BackColor = test.test[a].Item2 ? Color.Green : Color.Red;
                    }
                }
            }            
        }

        #endregion

        #region методы для обработки проверки

        async Task<bool> button_on_off(ControlButton button, byte set = 0x00)
        {
            if (button.send_data == null)
                return false;
            List<Modbus> ports = main_form.all_ports.FindAll(x => x.name == button.send_data.port_name);
            for(int a=0; a < 10; a++)
            {
                if (a >= 8)
                    return false;
                int output = 0;
                foreach (Modbus p in ports)
                    if (p.interrupt_delay > 0)
                        output++;
                if (output == 0)
                    break;
                await Task.Delay(100);
            }
            main_form.sending_data_button_on_off(button, set);
            //for (int full_cycle = 0; full_cycle < 5 && main_form.continue_cycle; full_cycle++)
            //{
            //    main_form.sending_data_button_on_off(button, set);
            //    for (int cycle = 0; cycle < 5 && button.BackColor != Color.Red && main_form.continue_cycle; cycle++)
            //        await Task.Delay(50);
            //    if (full_cycle >= 5)
            //        return false;
            //}
            return true;
        }

        async Task<bool> checkout(ControlButton button, float min, float max, Result result)
        {
            await Task.Delay(100);
            for(int cycle = 0; cycle <50 && (button.Result < min || button.Result > max); cycle++)
            {
                if (!main_form.continue_cycle) return false;
                await Task.Delay(50);
                if (cycle >= 25)
                {
                    if(result != null)
                        result.add(button.Result, false);
                    return false;
                }
            }
            if (result != null)
                result.add(button.Result, true);
            return true;
        }

        async Task<bool> buttons_result(List<ControlButton> all_button, int x, Result result, MinMaxNone min_max)
        {
            await checkout(all_button[x], min_max.min, min_max.max, null);
            for (int a = 0; a < all_button.Count; a++) 
            {
                if (!main_form.continue_cycle) return false;
                if (x == a)
                {
                    await checkout(all_button[x], min_max.min, min_max.max, result);
                }
                else
                {
                    await checkout(all_button[a], 0, min_max.none, result);
                }
            }
            return true;
        }

        #endregion

        public async void partial_test(string test_name)
        {
            if (main_form.child_form_panel.Controls[0].GetType() != typeof(MainTMWindow))
                main_form.open_module_form();

            main_form.continue_cycle = true;
            List<Result> all_result = new List<Result>();
            if (test_name == "power")
                all_result.Add(await power_test());
            else if (test_name == "module power")
                all_result.Add(await module_power_test());
            else if (test_name == "tu")
                all_result.Add(await tu_test());
            else if (test_name == "ping")
                all_result.Add(await ping_module());
            else if (test_name == "full test")
                all_result = await full_test(all_result);
            else
                all_result.Add(await tc_test(test_name));
            if (!main_form.continue_cycle)
            {
                main_form.condition_tb.Text += "***Прерывание проверки..." + Environment.NewLine;
                await main_form.power_set(0);
                return;
            }
            data_grid_draw(all_result);
        }

        async Task<List<Result>> full_test(List<Result> results)
        {
            await number_module_change();
            Timer t = new Timer();
            t.Interval = 100;
            t.Tick += (s, e) => { time_test += 100; };
            t.Start();
            main_form.condition_tb.Text += "Начало полной проверки..." + Environment.NewLine;
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.rs485_ports_test.test) results.Add(await rs485_port_test());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.power_test.test) results.Add(await power_test());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.module_power_test.test) results.Add(await module_power_test());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.din_test.test) results.Add(await tc_test("din"));
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.kf_test.test) results.Add(await tc_test("kf"));
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.tc_test.test) results.Add(await tc_test("tc"));
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.only_mtu_tu_test.test) results.Add(await only_mtu_tu_test());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.entu_test.test) results.Add(await entu_test());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.tu_test.test) results.Add(await tu_test());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.tc_12v_test.test) results.Add(await tc_12v());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.current0_test.test) results.Add(await current0_test());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.temperature_test.test) results.Add(await temperature_test());
            if (main_form.continue_cycle && main_form.module_settings.AllAddres.module_ping.test) results.Add(await ping_module());
            if (main_form.continue_cycle) main_form.condition_tb.Text += $"Завершение полной проверки блока №{module_number}..." + Environment.NewLine;
            t.Stop();
            this.button1.Visible = true;
            return results;
        }

        async Task<bool> number_module_change()
        {
            number_is_get = false;
            EnterField ef = new EnterField($"  Введи номер проверяемого модуля", module_number.ToString());
            ef.Show();
            ef.enter_handler += (text) =>
            {
                try
                {
                    module_number = int.Parse(text);
                }
                catch (Exception) { }
                number_is_get = true;
                ef.Dispose();
            };
            ef.without_change_handler += () => { this.Dispose(); };
            while (!number_is_get)
                await Task.Delay(100);
            return true;
        }

        #region Все методы проверок

        public async Task<Result> tc_test(string tc_name)
        {
            Result result_output = new Result("Проверка " + tc_name);

            main_form.condition_tb.Text += "Начало проверки " + tc_name + Environment.NewLine;
            List<ControlButton> tc_button = main_form.main_panel.search_button_control().FindAll(x => x.Name.Contains(tc_name) && !x.Name.Contains("ind") && x.Visible);

            await main_form.power_set(1);
            for (int a = 0; a < tc_button.Count; a++)
            {
                if (!main_form.continue_cycle) return null;
                await button_on_off(tc_button[a], 0x01);
                await Task.Delay(200);
                result_output.add_new_test($"Тест {a + 1}");
                await buttons_result(tc_button, a, result_output, main_form.module_settings.all_min_max.Find(x => x.name == tc_name));
                await button_on_off(tc_button[a]);
            }

            main_form.condition_tb.Text += "Проверка " + tc_name + " завершена" + Environment.NewLine;
            await main_form.power_set(0);
            return result_output;
        }
        
        public async Task<Result> tu_test()
        {
            Result result_output = new Result("Проверка ТУ");

            main_form.condition_tb.Text += "Начало проверки ТУ" + Environment.NewLine;
            List<ControlButton> tc_button = main_form.main_panel.search_button_control("tu").FindAll(x => x.Visible);
            MinMaxNone tu_min_max = main_form.module_settings.all_min_max.Find(x => x.name == "tu");
            ControlButton button = main_form.main_panel.search_button_control("mtu_220v_tu")[0];

            await main_form.power_set(1);
            for (int a = tc_button.Count - 1; a >= 0; a--)
            {
                if (!main_form.continue_cycle) return null;
                await button_on_off(tc_button[a], 0x01);
                result_output.add_new_test($"Тест {a + 1}");
                await checkout(button, tu_min_max.min, tu_min_max.max, result_output);
                await button_on_off(tc_button[a]);
                await checkout(button, 0, 20, null);
            }

            main_form.condition_tb.Text += "Проверка ТУ" + " завершена" + Environment.NewLine;
            await main_form.power_set(0);
            return result_output;
        }

        public async Task<Result> power_test()
        {
            Result result_output = new Result("Проверка питания блока");
            main_form.condition_tb.Text += "Начало проверки питания" + Environment.NewLine;
            List<ControlButton> power_button = main_form.main_panel.search_button_control("power_button");
            power_button.Reverse();

            ControlButton current = main_form.main_panel.search_button_control("psc_current")[0];
            MinMaxNone min_max = main_form.module_settings.AllAddres.power_min_max;

            await button_on_off(power_button[0], 0x01);
            for (int a = 1; a < power_button.Count(); a++) await button_on_off(power_button[a]);
            
            for (int a = 0; a < main_form.module_settings.AllAddres.power_port_count.min; a++) 
            {
                await Task.Delay(1000);
                if (!main_form.continue_cycle) return null;
                result_output.add_new_test($"Тест {a + 1}");
                await checkout(current, min_max.min, min_max.max, result_output);
                if (a + 1 < main_form.module_settings.AllAddres.power_port_count.min)
                {
                    await button_on_off(power_button[a + 1], 0x01);
                    await button_on_off(power_button[a]);
                }
            }
            for (int a = 0; a < main_form.module_settings.AllAddres.power_port_count.min; a++) await button_on_off(power_button[a], 0x01);

            main_form.condition_tb.Text += "Проверка питания завершена..." + Environment.NewLine;
            return result_output;
        }
        
        public async Task<Result> module_power_test()
        {
            Result result_output = new Result("Проверка питания блока");
            main_form.condition_tb.Text += "Начало проверки питания" + Environment.NewLine;
            List<ControlButton> power_button = main_form.main_panel.search_button_control("power_button");
            power_button.Reverse();
            List<ControlButton> module_power_button = main_form.main_panel.search_button_control("Module Power").FindAll(x => x.Visible);
            if (module_power_button.Count == 0) return result_output;
            module_power_button.Reverse();

            ControlButton current = main_form.main_panel.search_button_control("psc_current")[0];
            MinMaxNone min_max = main_form.module_settings.AllAddres.module_power_min_max;

            await button_on_off(power_button[0], 0x01);
            for (int a = 1; a < power_button.Count; a++) await button_on_off(power_button[a]);
            
            for (int a = 0; a < main_form.module_settings.AllAddres.power_port_count.min; a++) 
            {
                await Task.Delay(250);
                if (!main_form.continue_cycle) return null;
                result_output.add_new_test($"Тест {a + 1}");
                await buttons_result(module_power_button, a, result_output, min_max);
                if (a + 1 < main_form.module_settings.AllAddres.power_port_count.min)
                {
                    await button_on_off(power_button[a + 1], 0x01);
                    await button_on_off(power_button[a]);
                }
            }
            for (int a = 0; a < main_form.module_settings.AllAddres.power_port_count.min; a++) await button_on_off(power_button[a], 0x01);

            main_form.condition_tb.Text += "Проверка питания завершена..." + Environment.NewLine;
            return result_output;
        }

        public async Task<Result> only_mtu_tu_test()
        {
            Result result_output = new Result("Проверка Ту блока");
            main_form.condition_tb.Text += "Начало проверки ту" + Environment.NewLine;

            List<ControlButton> tu_button = main_form.main_panel.search_button_control("tu");
            if (tu_button.Count == 0)
                return result_output;
            tu_button.Reverse();

            int loop = 50;
            List<float>[] item = new List<float>[3] { new List<float>(), new List<float>(), new List<float>() };
            

            for (int a=0; a < loop; a++)
            {
                for (int x = 0; x < 3; x++)
                {
                    item[x].Add(tu_button[x].Result);
                }

                await Task.Delay(100);
            }

            string[] tu = { "Максимальное значение ON", "Максимальное значение OFF", "Максимальное значение RF" };
            for (int a = 0; a < 3; a++)
            {
                result_output.add_new_test(tu[a]);
                result_output.add(item[a][0], item[a][0] < 20);
            }

            main_form.condition_tb.Text += "Проверка ту завершена..." + Environment.NewLine;
            return result_output;
        }

        public async Task<Result> entu_test()
        {
            Result result_output = new Result("Проверка EnTU");
            main_form.condition_tb.Text += "Проверка Entu" + Environment.NewLine;

            result_output.add_new_test();
            result_output.add(0, main_form.main_panel.search_button_control("EnTU")[0].BackColor == Color.Red);

            main_form.condition_tb.Text += "Проверка Entu завершена..." + Environment.NewLine;
            return result_output;
        }
        
        public async Task<Result> tc_12v()
        {
            Result result_output = new Result("Проверка TC 12B");
            main_form.condition_tb.Text += "Проверка TC 12B" + Environment.NewLine;
            ControlButton button = main_form.main_panel.search_button_control("mtu_TC_12v")[1];
            MinMaxNone tc_12v_min_max = main_form.module_settings.all_min_max.Find(x => x.name == "tc12v");

            result_output.add_new_test();
            await checkout(button, tc_12v_min_max.min, tc_12v_min_max.max, result_output);

            main_form.condition_tb.Text += "Проверка TC 12B завершена..." + Environment.NewLine;
            return result_output;
        }
        
        public async Task<Result> temperature_test()
        {
            Result result_output = new Result("Проверка температуры");
            main_form.condition_tb.Text += "Проверка температуры" + Environment.NewLine;
            ControlButton button = main_form.main_panel.search_button_control("Temperature")[0];

            result_output.add_new_test();
            await checkout(button, 15, 45, result_output);

            main_form.condition_tb.Text += "Проверка температуры завершена..." + Environment.NewLine;
            return result_output;
        }
        
        public async Task<Result> current0_test()
        {
            Result result_output = new Result("Проверка current0");
            main_form.condition_tb.Text += "Проверка current0" + Environment.NewLine;
            ControlButton button = main_form.main_panel.search_button_control("current 0")[0];
            
            float average_float = button.result;
            for (int a = 0; a < 20; a++)
            {
                average_float = (average_float + button.result) / 2;
                await Task.Delay(100);
            }
            result_output.add_new_test("Без нагрузки");
            result_output.add(value: average_float, condition: true);

            await button_on_off(button, 0x01);
            await Task.Delay(1000);
            average_float = button.result;
            for (int a = 0; a < 20; a++)
            {
                average_float = (average_float + button.result) / 2;
                await Task.Delay(100);
            }
            await button_on_off(button);

            result_output.add_new_test("C нагрузкой");
            result_output.add(value: average_float, condition: Math.Abs(result_output.all_test[0].test[0].Item1 - average_float) > (result_output.all_test[0].test[0].Item1 / 5));

            main_form.condition_tb.Text += "Проверка current0 завершена..." + Environment.NewLine;
            return result_output;
        }
        
        public async Task<Result> rs485_port_test()
        {
            Result result_output = new Result("Проверка обмена по портам");
            main_form.condition_tb.Text += "Проверка обмена" + Environment.NewLine;
            int count = 0;
            List<Modbus> module_ports = main_form.all_ports.FindAll(x => x.name == "Module Port");

            for (int a = 0; a < module_ports.Count(); a++) 
            {
                if (module_ports[a].exchange_counter < 10)
                {
                    result_output.add_new_test($"Порт канал {(char)((int)'a' + count)}");
                    result_output.add(condition: true);
                    count++;
                    if (count == (int)main_form.module_settings.AllAddres.rs485_port_count.min)
                        break;
                }
            }
            if (count < (int)main_form.module_settings.AllAddres.rs485_port_count.min)
            {
                for( ; count < (int)main_form.module_settings.AllAddres.rs485_port_count.min; count++)
                {
                    result_output.add_new_test($"Порт канал {(char)((int)'a' + count)}");
                    result_output.add(condition: false);
                }
            }
            main_form.condition_tb.Text += "Проверка обмена завершена..." + Environment.NewLine;
            return result_output;
        }

        public async Task<Result> ping_module()
        {
            Result result_output = new Result("Проверка пинга");
            main_form.condition_tb.Text += "Проверка пинга" + Environment.NewLine;

            result_output.add_new_test("Тест");
            result_output.add(0, await StaticClass.PingHost("192.168.5.127"));

            main_form.condition_tb.Text += "Проверка пинга завершена..." + Environment.NewLine;
            return result_output;
        }
        #endregion

        #region Доп классы

        public class Result
        {
            public string test_name;
            public List<Test> all_test;
            public int all_test_count = 0;
            public bool is_test_norm = true;

            public Result(string test_name = "")
            {
                this.test_name = test_name;
                all_test = new List<Test>();
            }

            public void add_new_test(string name = "")
            {
                all_test.Add(new Test(name));
            }

            public void add(float value = 0, bool condition = false)
            {
                all_test[all_test.Count - 1].add(value, condition);
                if (all_test[all_test.Count - 1].test.Count > all_test_count)
                    all_test_count = all_test[all_test.Count - 1].test.Count;
                if (!condition)
                    is_test_norm = false;
            }
        }

        public class Test
        {
            public string test_name;
            public List<(float, bool)> test;

            public Test(string test_name)
            {
                this.test = new List<(float, bool)>();
                this.test_name = test_name;
            }

            public void add(float value, bool condition)
            {
                test.Add((value, condition));
            }
        }

        #endregion
    }
}
