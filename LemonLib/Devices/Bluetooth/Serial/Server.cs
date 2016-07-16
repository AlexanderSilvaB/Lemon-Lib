using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace LemonLib.Devices.Bluetooth.Serial
{
    public class Server
    {

        // The value of the Service Name SDP attribute
        protected string SdpServiceName;
        protected RfcommServiceId serviceId = null;

        protected StreamSocket socket;
        protected DataWriter writer;
        protected RfcommServiceProvider rfcommProvider;
        protected StreamSocketListener socketListener;

        public bool IsConnected = false;

        public Device.DataModes DataMode = Device.DataModes.String;

        public event EventHandler<SerialEventArgs> OnReceive;
        public event EventHandler<ServerConnectionEventArgs> OnConnect;

        public Server(string serviceName, string uuid)
        {
            SdpServiceName = serviceName;
            Guid UUID = new Guid(uuid);
            serviceId = RfcommServiceId.FromUuid(UUID);
        }

        public Server(string serviceName = "Bluetooth Serial Service", RfcommServiceId serviceid = null)
        {
            SdpServiceName = serviceName;
            if (serviceId == null)
                serviceId = RfcommServiceId.SerialPort;
            else
                serviceId = serviceid;
        }

        public RfcommServiceId GetServiceID()
        {
            return serviceId;
        }

        public async Task<bool> Start()
        {
            try
            {
                rfcommProvider = await RfcommServiceProvider.CreateAsync(serviceId);
                // Create a listener for this service and start listening
                socketListener = new StreamSocketListener();
                socketListener.ConnectionReceived += OnConnectionReceived;

                await socketListener.BindServiceNameAsync(rfcommProvider.ServiceId.AsString(), SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                // Set the SDP attributes and start Bluetooth advertising
                InitializeServiceSdpAttributes(rfcommProvider);
                rfcommProvider.StartAdvertising(socketListener);

                return true;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return false;
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

        /// <summary>
        /// Initialize the Rfcomm service's SDP attributes.
        /// </summary>
        /// <param name="rfcommProvider">The Rfcomm service provider to initialize.</param>
        protected void InitializeServiceSdpAttributes(RfcommServiceProvider rfcommProvider)
        {
            var sdpWriter = new DataWriter();

            // Write the Service Name Attribute.

            sdpWriter.WriteByte((byte)Device.AttributeType.SDP);

            // The length of the UTF-8 encoded Service Name SDP Attribute.
            sdpWriter.WriteByte((byte)SdpServiceName.Length);

            // The UTF-8 encoded Service Name value.
            sdpWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            sdpWriter.WriteString(SdpServiceName);

            // Set the SDP Attribute on the RFCOMM Service Provider.
            rfcommProvider.SdpRawAttributes.Add((UInt16)Device.AttributeId.SDP, sdpWriter.DetachBuffer());
        }

        /// <summary>
        /// Invoked when the socket listener accepted an incoming Bluetooth connection.
        /// </summary>
        /// <param name="sender">The socket listener that accecpted the connection.</param>
        /// <param name="args">The connection accept parameters, which contain the connected socket.</param>
        protected void OnConnectionReceived( StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            IsConnected = true;

            ServerConnectionEventArgs device = new ServerConnectionEventArgs();
            device.Address = args.Socket.Information.RemoteAddress.DisplayName;
            device.Name = args.Socket.Information.RemoteHostName.DisplayName;


            // Don't need the listener anymore
            socketListener.Dispose();
            socketListener = null;

            socket = args.Socket;

            writer = new DataWriter(socket.OutputStream);

            var reader = new DataReader(socket.InputStream);

            OnConnect?.Invoke(this, device);

            if (DataMode == Device.DataModes.Byte)
                ReceiveByteLoop(reader);
            else if (DataMode == Device.DataModes.String)
                ReceiveStringLoop(reader);
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

        /// <summary>
        /// Cleanup Bluetooth resources
        /// </summary>
        public void Disconnect()
        {
            if (rfcommProvider != null)
            {
                rfcommProvider.StopAdvertising();
                rfcommProvider = null;
            }

            if (socketListener != null)
            {
                socketListener.Dispose();
                socketListener = null;
            }

            if (writer != null)
            {
                writer.DetachStream();
                writer = null;
            }

            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }

            IsConnected = false;
        }
    }
}
