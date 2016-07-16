using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

namespace LemonLib.Devices.Bluetooth
{
    public class Device
    {

        public enum DataModes { Byte, String };

        // The Id of the Service Name SDP attribute
        public enum AttributeId { SDP = (UInt16)0x100 };

        // The SDP Type of the Service Name SDP attribute.
        // The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
        //    -  the Attribute Type size in the least significant 3 bits,
        //    -  the SDP Attribute Type value in the most significant 5 bits.
        public enum AttributeType { SDP = (byte)((4 << 3) | 5) };

        public static Windows.Devices.Radios.Radio Radio = null;


        public string Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public bool IsPaired { get; set; }
        public DeviceInformation device { get; set; }
        public Device() { }
        public Device(DeviceInformation device)
        {
            Id = device.Id;
            IsPaired = device.Pairing.IsPaired;
            Name = device.Name;
            this.device = device;
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

        public static async Task<bool> RequestBluetoothAccess()
        {
            var accessLevel = await Windows.Devices.Radios.Radio.RequestAccessAsync();
            if (accessLevel != Windows.Devices.Radios.RadioAccessStatus.Allowed)
            {
                return false;
            }
            else
            {
                var radios = await Windows.Devices.Radios.Radio.GetRadiosAsync();
                foreach (var radio in radios)
                {
                    if (radio.Kind == Windows.Devices.Radios.RadioKind.Bluetooth)
                    {
                        Radio = radio;
                        break;
                    }
                }

                return true;
            }

        }

        public static async Task<bool> TurnBluetooth(bool on)
        {
            if (Radio != null)
            {
                var access = await Radio.SetStateAsync(on ? Windows.Devices.Radios.RadioState.On : Windows.Devices.Radios.RadioState.Off);
                if (access == Windows.Devices.Radios.RadioAccessStatus.Allowed)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool? IsOn()
        {
            if (Radio != null)
            {
                return Radio.State == Windows.Devices.Radios.RadioState.On;
            }
            return null;
        }

        public static async Task<Device[]> GetDevices(string uuid)
        {
            Windows.Devices.Bluetooth.Rfcomm.RfcommServiceId serviceid = Windows.Devices.Bluetooth.Rfcomm.RfcommServiceId.FromUuid(new Guid(uuid));
            List<Device> devices = new List<Device>();
            var chatServiceDeviceCollection = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Bluetooth.Rfcomm.RfcommDeviceService.GetDeviceSelector(serviceid));
            foreach (var chatServiceDevice in chatServiceDeviceCollection)
            {
                devices.Add(new Device(chatServiceDevice));
            }
            return devices.ToArray();
        }

        public static async Task<Device[]> GetDevices(Windows.Devices.Bluetooth.Rfcomm.RfcommServiceId serviceid = null)
        {
            if(serviceid == null)
            {
                serviceid = Windows.Devices.Bluetooth.Rfcomm.RfcommServiceId.SerialPort;
            }
            List<Device> devices = new List<Device>();
            var chatServiceDeviceCollection = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Bluetooth.Rfcomm.RfcommDeviceService.GetDeviceSelector(serviceid));
            foreach (var chatServiceDevice in chatServiceDeviceCollection)
            {
                devices.Add(new Device(chatServiceDevice));
            }
            return devices.ToArray();
        }
    }
}
