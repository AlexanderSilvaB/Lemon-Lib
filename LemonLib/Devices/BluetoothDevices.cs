using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

namespace LemonLib.Devices
{
    public class BluetoothDevice
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public bool IsPaired { get; set; }
        public DeviceInformation Device { get; set; }
        public BluetoothDevice() { }
        public BluetoothDevice(DeviceInformation device)
        {
            Id = device.Id;
            IsPaired = device.Pairing.IsPaired;
            Name = device.Name;
            Device = device;
            int i = Id.LastIndexOf("&");
            int f = Id.IndexOf("_", i);
            string address = Id.Substring(i + 1, f - i - 1);
            Address = "";
            for (i = 0, f = 1; i < address.Length; i++, f++)
            {
                Address += address[i];
                if (f == 2)
                {
                    f = 0;
                    Address += ":";
                }
            }
            if (Address.EndsWith(":"))
                Address = Address.Substring(0, Address.Length - 1);
        }
    }
}
