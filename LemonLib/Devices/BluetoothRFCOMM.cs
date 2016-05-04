using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.Radios;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace LemonLib.Devices
{
    public class BluetoothRfcomm
    {
        // The Id of the Service Name SDP attribute
        public const UInt16 SdpServiceNameAttributeId = 0x100;

        // The SDP Type of the Service Name SDP attribute.
        // The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
        //    -  the Attribute Type size in the least significant 3 bits,
        //    -  the SDP Attribute Type value in the most significant 5 bits.
        public const byte SdpServiceNameAttributeType = (4 << 3) | 5;

        // The value of the Service Name SDP attribute
        public string SdpServiceName;

        public bool IsConnected = false;

        protected Radio BluetoothRadio = null;

        protected string ServiceName = null;
        protected StreamSocket chatSocket = null;
        protected DataWriter chatWriter = null;
        protected RfcommDeviceService chatService = null;

        public event EventHandler<BluetoothSDPEventArgs> BluetoothSDPEvent;

        public BluetoothRfcomm(string serviceName = "Bluetooth Rfcomm Chat Service")
        {
            SdpServiceName = serviceName;
        }


        public string GetDeviceName()
        {
            if (chatService == null || chatService.Device == null)
                return null;
            return chatService.Device.Name;
        }

        public async Task<bool> RequestBluetoothAccess()
        {
            var accessLevel = await Radio.RequestAccessAsync();
            if (accessLevel != RadioAccessStatus.Allowed)
            {
                return false;
            }
            else
            {
                var radios = await Radio.GetRadiosAsync();
                foreach (var radio in radios)
                {
                    if (radio.Kind == RadioKind.Bluetooth)
                    {
                        BluetoothRadio = radio;
                        break;
                    }
                }

                return true;
            }

        }

        public async Task<bool> TurnBluetooth(bool on)
        {
            if (BluetoothRadio != null)
            {
                var access = await BluetoothRadio.SetStateAsync(on ? RadioState.On : RadioState.Off);
                if (access == RadioAccessStatus.Allowed)
                {
                    return true;
                }
            }
            return false;
        }

        public bool? IsOn()
        {
            if (BluetoothRadio != null)
            {
                return BluetoothRadio.State == RadioState.On;
            }
            return null;
        }

        public async Task<BluetoothDevice[]> GetDevices()
        {
            List<BluetoothDevice> devices = new List<BluetoothDevice>();
            var chatServiceDeviceCollection = await DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            foreach (var chatServiceDevice in chatServiceDeviceCollection)
            {
                devices.Add(new BluetoothDevice(chatServiceDevice));
            }
            return devices.ToArray();
        }

        private void Clear()
        {
            IsConnected = false;
            ServiceName = null;
            chatService = null;
        }

        public async Task<bool> Connect(BluetoothDevice device)
        {
            chatService = await RfcommDeviceService.FromIdAsync(device.Id);

            var attributes = await chatService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey(SdpServiceNameAttributeId))
            {
                Clear();
                return false;
            }

            var attributeReader = DataReader.FromBuffer(attributes[SdpServiceNameAttributeId]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != SdpServiceNameAttributeType)
            {
                Clear();
                return false;
            }

            var serviceNameLength = attributeReader.ReadByte();
            // The Service Name attribute requires UTF-8 encoding.
            attributeReader.UnicodeEncoding = UnicodeEncoding.Utf8;
            ServiceName = attributeReader.ReadString(serviceNameLength);

            lock (this)
            {
                chatSocket = new StreamSocket();
            }
            try
            {
                await chatSocket.ConnectAsync(chatService.ConnectionHostName, chatService.ConnectionServiceName);

                chatWriter = new DataWriter(chatSocket.OutputStream);

                DataReader chatReader = new DataReader(chatSocket.InputStream);
                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex)
            {
                Clear();
                return false;
            }
            IsConnected = true;
            return true;
        }

        public async Task<bool> Write(string text)
        {
            try
            {
                if (text.Length != 0)
                {
                    chatWriter.WriteUInt32((uint)text.Length);
                    chatWriter.WriteString(text);
                    await chatWriter.StoreAsync();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private async void ReceiveStringLoop(DataReader chatReader)
        {
            BluetoothSDPEventArgs eventArgs = new BluetoothSDPEventArgs();
            try
            {
                uint size = await chatReader.LoadAsync(sizeof(uint));
                if (size < sizeof(uint))
                {
                    eventArgs.Error = new Exception("Remote device terminated connection");
                    BluetoothSDPEvent?.Invoke(this, eventArgs);
                    return;
                }

                uint stringLength = chatReader.ReadByte();
                System.Diagnostics.Debug.WriteLine(stringLength + " - Size");
                uint actualStringLength = await chatReader.LoadAsync(stringLength);
                if (actualStringLength != stringLength)
                {
                    eventArgs.Error = new Exception("Remote device closed connection");
                    BluetoothSDPEvent?.Invoke(this, eventArgs);
                    return;
                }

                eventArgs.Message = chatReader.ReadString(stringLength);
                BluetoothSDPEvent?.Invoke(this, eventArgs);



                ReceiveStringLoop(chatReader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (chatSocket == null)
                    {
                        eventArgs.Error = new Exception("Remote device disconnected", ex);
                        BluetoothSDPEvent?.Invoke(this, eventArgs);
                        // Do not print anything here -  the user closed the socket.
                        // HResult = 0x80072745 - catch this (remote device disconnect) ex = {"An established connection was aborted by the software in your host machine. (Exception from HRESULT: 0x80072745)"}
                    }
                    else
                    {
                        eventArgs.Error = new Exception("Read stream failed with error", ex);
                        BluetoothSDPEvent?.Invoke(this, eventArgs);
                    }
                }
            }
        }

        public void Disconnect()
        {
            if (chatWriter != null)
            {
                chatWriter.DetachStream();
                chatWriter = null;
            }


            if (chatService != null)
            {
                chatService.Dispose();
                chatService = null;
            }
            lock (this)
            {
                if (chatSocket != null)
                {
                    chatSocket.Dispose();
                    chatSocket = null;
                }
            }
            Clear();
        }

        public class BluetoothSDPEventArgs : EventArgs
        {
            public string Message { get; set; }
            public byte[] Bytes { get; set; }
            public Exception Error { get; set; }
        }
    }
}