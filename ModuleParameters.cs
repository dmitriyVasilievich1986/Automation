using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class ModuleParameters
    {
        public string module_name;
        public List<CheckButtonClass> din16 = new List<CheckButtonClass>();
        public List<CheckButtonClass> din16_ind = new List<CheckButtonClass>();
        public List<CheckButtonClass> din16_ind_value = new List<CheckButtonClass>();
        public List<CheckButtonClass> din32 = new List<CheckButtonClass>();
        public List<CheckButtonClass> kf = new List<CheckButtonClass>();
        public List<CheckButtonClass> kf_ind = new List<CheckButtonClass>();
        public List<CheckButtonClass> kf_ind_value = new List<CheckButtonClass>();
        public List<CheckButtonClass> tc = new List<CheckButtonClass>();
        public List<CheckButtonClass> power = new List<CheckButtonClass>();
        public List<CheckButtonClass> tu = new List<CheckButtonClass>();
        public List<CheckButtonClass> tu_ind = new List<CheckButtonClass>();
        public List<CheckButtonClass> tu_ind_value = new List<CheckButtonClass>();
        public List<CheckButtonClass> tu_color = new List<CheckButtonClass>();
        public List<CheckButtonClass> u_out_value = new List<CheckButtonClass>();
        public List<CheckButtonClass> u_out_color = new List<CheckButtonClass>();
        public List<CheckButtonClass> i_out_value = new List<CheckButtonClass>();
        public List<CheckButtonClass> i_out_color = new List<CheckButtonClass>();
        public List<CheckButtonClass> dout_value = new List<CheckButtonClass>();
        public List<CheckButtonClass> dout_color = new List<CheckButtonClass>();
        public List<CheckButtonClass> temperature_value = new List<CheckButtonClass>();
        public List<CheckButtonClass> akk_value = new List<CheckButtonClass>();
        public CheckButtonClass curr0_value = new CheckButtonClass(module: new DataAddress(1), address: new byte[2] { 0, 0 }, port: "Module Port");
        public List<DataSending> data_to_send = new List<DataSending>();

        public MinMaxNone din_min_max = new MinMaxNone(name: "din", describe: "Din min/max: ");
        public MinMaxNone kf_min_max = new MinMaxNone(name: "kf", describe: "KF min/max: ");
        public MinMaxNone tc_min_max = new MinMaxNone(name: "tc", describe: "TC min/max: ");
        public MinMaxNone tu_min_max = new MinMaxNone(name: "tu", describe: "TU min/max: ");
        public MinMaxNone tc_12v_min_max = new MinMaxNone(name: "tc12v", describe: "TC 12B min/max: ");
        public MinMaxNone power_min_max = new MinMaxNone(name: "power", describe: "Ток потребления: ");
        public MinMaxNone module_power_min_max = new MinMaxNone(name: "module power", describe: "Питание min/max: ");
        public MinMaxNone power_port_count = new MinMaxNone(name: "power_port_count", describe: "Количество портов питания: ");
        public MinMaxNone rs485_port_count = new MinMaxNone(name: "rs485", describe: "Количество портов обмена: ");

        public TestMethod din_test = new TestMethod("din");
        public TestMethod kf_test = new TestMethod("kf");
        public TestMethod tc_test = new TestMethod("tc");
        public TestMethod power_test = new TestMethod("Проверка тока потребления по всем каналам");
        public TestMethod module_power_test = new TestMethod("Питание показания самого модуля");
        public TestMethod only_mtu_tu_test = new TestMethod("Только MTU tu");
        public TestMethod entu_test = new TestMethod("Проверка Entu");
        public TestMethod tu_test = new TestMethod("Проверка ТУ");
        public TestMethod tc_12v_test = new TestMethod("Проверка TC 12B");
        public TestMethod current0_test = new TestMethod("Проверка Current0");
        public TestMethod temperature_test = new TestMethod("Проверка температуры");
        public TestMethod rs485_ports_test = new TestMethod("Проверка обмена портов rs485");
        public TestMethod module_ping = new TestMethod("Проверка пинга ethernet");

        public ModuleParameters(string module_name = "")
        {
            this.module_name = module_name;
            for (int a = 0; a < 16; a++)
            {
                din16.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                din16_ind.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                din16_ind_value.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                din32.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                dout_value.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                dout_color.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
            }
            for (int a = 0; a < 4; a++)
            {
                kf.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                kf_ind.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                kf_ind_value.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                tc.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                tu.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                tu_ind.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                tu_ind_value.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                tu_color.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                power.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
            }
            for(int a = 0; a < 2; a++)
            {
                u_out_value.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                u_out_color.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                i_out_value.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                i_out_color.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                temperature_value.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
                akk_value.Add(new CheckButtonClass(
                    module: new DataAddress(1),
                    address: new byte[2] { 0, 0 },
                    port: "Module Port"
                    ));
            }
        }

        public List<MinMaxNone> all_min_max
        {
            get
            {
                return new List<MinMaxNone>() { module_power_min_max, power_min_max, din_min_max, kf_min_max, tc_min_max, tu_min_max, power_port_count, rs485_port_count, tc_12v_min_max };
            }
        }

        public List<TestMethod> all_tests { get { return new List<TestMethod>() { din_test, kf_test, tc_test, tu_test, power_test, module_power_test, only_mtu_tu_test, entu_test, rs485_ports_test, module_ping, tc_12v_test, current0_test, temperature_test }; } }
    }

    public class MinMaxNone
    {
        public float min;
        public float max;
        public float none;
        public string name;
        public string describe;

        public MinMaxNone(float min = 0, float max = 0, float none = 0, string name = "", string describe = "")
        {
            this.name = name;
            this.min = min;
            this.max = max;
            this.none = none;
            this.describe = describe;
        }

        public string min_max_get()
        {
            string output = min.ToString();
            if (max != 0) output += " " + max.ToString();
            if (none != 0) output += " " + none.ToString();
            return output;
        }

        public void min_max_set(string min_max)
        {
            string[] new_value;
            try { min = float.Parse(min_max); }
            catch { }
            if (min_max.Contains(' '))
                new_value = min_max.Split();
            else if (min_max.Contains('-'))
                new_value = min_max.Split('-');
            else if (min_max.Contains('/'))
                new_value = min_max.Split('/');
            else
                return;
            try { min = float.Parse(new_value[0]); }
            catch { }
            try { max = float.Parse(new_value[1]); }
            catch { }
            try { none = float.Parse(new_value[2]); }
            catch { }
        }
    }

    public class AllSettings
    {
        public DataAddress dout_din16 = new DataAddress(20, "Dout Din16");
        public DataAddress dout_din32 = new DataAddress(21, "Dout Din32");
        public DataAddress dout_control = new DataAddress(19, "Dout Control");
        public DataAddress mtu5 = new DataAddress(16, "MTU5");
        public DataAddress psc = new DataAddress(17, "PSC");
        public DataAddress module = new DataAddress(1, "Module");

        ModuleParameters all_addres = new ModuleParameters();

        public ModuleParameters AllAddres
        {
            get { return all_addres; }
            set
            {
                all_addres = value;
                for (int a = 0; a < 16; a++)
                {
                    all_addres.din16[a].module = this.module;
                    all_addres.din32[a].module = this.module;
                }
                for (int a = 0; a < 3; a++)
                {
                    all_addres.kf[a].module = this.module;
                    all_addres.tc[a].module = this.module;
                }
                for (int a = 0; a < 4; a++)
                {
                    all_addres.tu[a].module = this.module;
                    all_addres.tu_color[a].module = this.module;
                }
                foreach (DataSending ds in all_addres.data_to_send)
                    ds.address = this.module;
            }
        }

        public string name
        {
            get
            {
                return AllAddres.module_name;
            }
        }

        public List<MinMaxNone> all_min_max
        {
            get
            {
                return AllAddres.all_min_max;
            }
        }

        public List<DataAddress> all_modules_addres
        {
            get
            {
                return new List<DataAddress>() { dout_din16, dout_din32, dout_control, mtu5, psc, module };
            }
        }
    }

    public class TestMethod
    {
        public string name = "";
        public bool test = false;

        public TestMethod(string name = "", bool test = false)
        {
            this.test = test;
            this.name = name;
        }
    }
}
