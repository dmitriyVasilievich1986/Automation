using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    class data_sending
    {

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
