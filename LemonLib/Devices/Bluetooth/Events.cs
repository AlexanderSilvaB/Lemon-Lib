using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemonLib.Devices.Bluetooth
{
    public class SerialEventArgs : EventArgs
    {
        public string Message { get; set; }
        public byte[] Bytes { get; set; }
        public Exception Error { get; set; }
    }

    public class ServerConnectionEventArgs : EventArgs
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
