using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class DataSending
    {
        public string port_name;
        public DataAddress address;
        public byte[] data;

        public DataSending(DataAddress using_addres, byte[] using_data, string port_name = "Control Port")
        {
            this.port_name = port_name;
            data = using_data;
            address = using_addres;
        }

        public byte[] send_data()
        {
            byte[] output = data;
            output[0] = address.Addres;
            return output;
        }
    }

    public class DataAddress
    {
        byte addres;
        public string name;

        public DataAddress(byte using_addres, string name = "")
        {
            this.Addres = using_addres;
            this.name = name;
        }

        public byte Addres
        {
            set { addres = value; }
            get { return addres; }
        }
    }

    public class CheckButtonClass
    {
        public DataAddress module;
        public byte[] address;
        public string port = "";
        public string button_text = "";

        public CheckButtonClass(
            DataAddress module,
            byte[] address,
            string port
            )
        {
            this.module = module;
            this.address = address;
            this.port = port;
        }
    }
}
