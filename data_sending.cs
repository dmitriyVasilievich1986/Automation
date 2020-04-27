using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class DataSending
    {
        DataAddress address;
        public byte[] data;

        public DataSending(DataAddress using_addres, byte[] using_data)
        {
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

        public DataAddress(byte using_addres)
        {
            this.Addres = using_addres;
        }

        public byte Addres
        {
            set { addres = value; }
            get { return addres; }
        }
    }
}
