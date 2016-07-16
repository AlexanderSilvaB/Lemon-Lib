using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.Radios;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace LemonLib.Devices.Bluetooth.Serial
{
    public class Client
    {
        // The value of the Service Name SDP attribute
        protected string SdpServiceName;

        public bool IsConnected = false;

        public Device.DataModes DataMode = Device.DataModes.String;

        protected string ServiceName = null;
        protected StreamSocket socket = null;
        protected DataWriter writer = null;
        protected RfcommDeviceService chatService = null;

        public event EventHandler<SerialEventArgs> OnReceive;

        public Client(string serviceName = "Bluetooth Serial Service")
        {
            SdpServiceName = serviceName;
        }


        public string GetDeviceName()
        {
            if (chatService == null || chatService.Device == null)
                return null;
            return chatService.Device.Name;
        }

        private void Clear()
        {
            IsConnected = false;
            ServiceName = null;
            chatService = null;
        }

        public async Task<bool> Connect(Device device)
        {
            chatService = await RfcommDeviceService.FromIdAsync(device.Id);

            var attributes = await chatService.GetSdpRawAttributesAsync();
            if (!attributes.ContainsKey((UInt16)Device.AttributeId.SDP))
            {
                Clear();
                return false;
            }

            var attributeReader = DataReader.FromBuffer(attributes[(UInt16)Device.AttributeId.SDP]);
            var attributeType = attributeReader.ReadByte();
            if (attributeType != (byte)Device.AttributeType.SDP)
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
                socket = new StreamSocket();
            }
            try
            {
                await socket.ConnectAsync(chatService.ConnectionHostName, chatService.ConnectionServiceName);

                writer = new DataWriter(socket.OutputStream);

                DataReader reader = new DataReader(socket.InputStream);
                if (DataMode == Device.DataModes.Byte)
                    ReceiveByteLoop(reader);
                else if (DataMode == Device.DataModes.String)
                    ReceiveStringLoop(reader);
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
                    writer.WriteUInt32((uint)text.Length);
                    writer.WriteString(text);
                    await writer.StoreAsync();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Write(byte[] data)
        {
            try
            {
                if (data.Length != 0)
                {
                    writer.WriteBytes(data);
                    await writer.StoreAsync();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private async void ReceiveStringLoop(DataReader reader)
        {
            SerialEventArgs eventArgs = new SerialEventArgs();
            try
            {
                uint size = await reader.LoadAsync(sizeof(uint));

                if (size < sizeof(uint))
                {
                    eventArgs.Error = new Exception("Remote device terminated connection");
                    OnReceive?.Invoke(this, eventArgs);
                    return;
                }

                uint stringLength = reader.ReadUInt32();
                uint actualStringLength = await reader.LoadAsync(stringLength);
                if (actualStringLength != stringLength)
                {
                    eventArgs.Error = new Exception("Remote device closed connection");
                    OnReceive?.Invoke(this, eventArgs);
                    return;
                }

                eventArgs.Message = reader.ReadString(stringLength);
                OnReceive?.Invoke(this, eventArgs);



                if (DataMode == Device.DataModes.Byte)
                    ReceiveByteLoop(reader);
                else if (DataMode == Device.DataModes.String)
                    ReceiveStringLoop(reader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (socket == null)
                    {
                        eventArgs.Error = new Exception("Remote device disconnected", ex);
                        OnReceive?.Invoke(this, eventArgs);
                        // Do not print anything here -  the user closed the socket.
                        // HResult = 0x80072745 - catch this (remote device disconnect) ex = {"An established connection was aborted by the software in your host machine. (Exception from HRESULT: 0x80072745)"}
                    }
                    else
                    {
                        eventArgs.Error = new Exception("Read stream failed with error", ex);
                        OnReceive?.Invoke(this, eventArgs);
                    }
                }
                reader.DetachStream();
            }
        }

        private async void ReceiveByteLoop(DataReader reader)
        {
            SerialEventArgs eventArgs = new SerialEventArgs();
            try
            {
                uint size = await reader.LoadAsync(1);
                byte bt = reader.ReadByte();
                eventArgs.Bytes = new byte[] { bt };
                OnReceive?.Invoke(this, eventArgs);



                if (DataMode == Device.DataModes.Byte)
                    ReceiveByteLoop(reader);
                else if (DataMode == Device.DataModes.String)
                    ReceiveStringLoop(reader);
            }
            catch (Exception ex)
            {
                lock (this)
                {
                    if (socket == null)
                    {
                        eventArgs.Error = new Exception("Remote device disconnected", ex);
                        OnReceive?.Invoke(this, eventArgs);
                        // Do not print anything here -  the user closed the socket.
                        // HResult = 0x80072745 - catch this (remote device disconnect) ex = {"An established connection was aborted by the software in your host machine. (Exception from HRESULT: 0x80072745)"}
                    }
                    else
                    {
                        eventArgs.Error = new Exception("Read stream failed with error", ex);
                        OnReceive?.Invoke(this, eventArgs);
                    }
                }
                reader.DetachStream();
            }
        }

        public void Disconnect()
        {
            if (writer != null)
            {
                writer.DetachStream();
                writer = null;
            }


            if (chatService != null)
            {
                chatService.Dispose();
                chatService = null;
            }
            lock (this)
            {
                if (socket != null)
                {
                    socket.Dispose();
                    socket = null;
                }
            }
            Clear();
        }
    }
}