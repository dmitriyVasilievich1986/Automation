using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct ByteToFloat
{
    [FieldOffset(0)]
    private float Float1;
    [FieldOffset(0)]
    private byte b2;
    [FieldOffset(1)]
    private byte b1;
    [FieldOffset(2)]
    private byte b4;
    [FieldOffset(3)]
    private byte b3;
    public float Out(byte a1, byte a2, byte a3, byte a4)
    {
        b1 = a1;
        b2 = a2;
        b3 = a3;
        b4 = a4;
        return Float1;
    }
}

namespace Automation
{
    public class Modbus : SerialPort
    {
        private ByteToFloat byte_to_float = new ByteToFloat();

        string name;
        int exchange_counter = 0;
        public byte[] data_transmit;
        public byte[] data_receive = { };
        public byte[] data_interupt;
        public string transmit_array;
        public string receive_array;
        public List<float> result;

        public Modbus(
            string using_name = "",
            int using_speed = 115200,
            string using_port_name = "COM1"
            )
        {
            this.BaudRate = using_speed;
            this.PortName = using_port_name;
            this.name = using_name;
            this.DataBits = 8;
            this.DataReceived += new SerialDataReceivedEventHandler(receive);
        }

        private byte[] ModRTU_CRC(byte[] data, int count)
        {
            List<byte> output = new List<byte>();
            for (int item = 0; item < count; item++)
            {
                output.Add(data[item]);
            }
            ulong crc = 0xFFFF;
            for (int pos = 0; pos < count; pos++)
            {
                crc ^= (ulong)data[pos];
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 1) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else crc >>= 1;
                }
            }
            output.Add((byte)crc);
            output.Add((byte)(crc >> 8));
            return output.ToArray();
        }

        public void Transmit(byte[] data_to_send)
        {
            if (!this.IsOpen) return;

            if (data_interupt != null)
                data_transmit = ModRTU_CRC(data_interupt, data_interupt.Length);
            else
                data_transmit = ModRTU_CRC(data_to_send, data_to_send.Length);

            try
            { this.Write(data_transmit, 0, data_transmit.Length); }
            catch (Exception) { return; }

            transmit_array = "Передача: ";
            foreach (byte item in data_transmit) transmit_array += item.ToString("X2") + " ";

            if (exchange_counter <= 10)
                exchange_counter++;
        }

        public void set_interrupt(byte[] data)
        {
            if (!this.IsOpen || this.data_interupt != null) return;
            this.data_interupt = data;
        }

        public void receive(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (exchange_counter >= 10)
                exchange_counter = 0;
            else
                exchange_counter--;

            try
                { this.Read(data_receive, 0, this.BytesToRead); }
            catch (Exception) { return; }

            if (data_receive.Length < 3) return;
            if (ModRTU_CRC(data_receive, data_receive.Length - 2) != data_receive) return;

            receive_array = "Прием: ";
            foreach (byte a in data_receive) receive_array += a.ToString("X2") + " ";
        }
    }
}
