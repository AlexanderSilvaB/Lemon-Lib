using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace LemonLib.Web.Http
{
    public class Server
    {
        protected const uint BufferSize = 8192;
        protected StreamSocketListener Listener;

        public delegate Task AsyncEventHandler(object sender, ServerRequest args);

        public AsyncEventHandler OnRequest;

        public int Port { get; set; }

        public Server(int port = 8080)
        {
            Port = port;
            Listener = new StreamSocketListener();
        }

        public async Task Start()
        {
            string host = GetIP() + ":" + Port;
            await Listener.BindEndpointAsync(new HostName(GetIP()), Port.ToString());
            Listener.ConnectionReceived += Listener_ConnectionReceived;
        }

        public Uri GetAddress(string endpoint = "/")
        {
            string host = "http://" + GetIP() + ":" + Port + endpoint;
            return new Uri(host);
        }

        public static string GetIP()
        {
            foreach (HostName localHostName in NetworkInformation.GetHostNames())
            {
                if (localHostName.IPInformation != null)
                {
                    if (localHostName.Type == HostNameType.Ipv4)
                    {
                        return localHostName.ToString();
                    }
                }
            }
            return null;
        }

        protected async Task WriteResponseAsync(string input, StreamSocket socket, string method, string path, IOutputStream os)
        {
            ServerRequest request = new ServerRequest();
            request.Path = path;
            request.Method = method;
            request.Input = input;
            request.InputData = Encoding.UTF8.GetBytes(input);
            request.Socket = socket;
            await OnRequest(this, request);

            using (Stream resp = os.AsStreamForWrite())
            {
                bool notFound = false;
                if (request.Handled)
                {
                    if (request.Output != null && request.OutputData == null)
                    {
                        request.OutputData = Encoding.UTF8.GetBytes(request.Output);
                    }
                    if (request.OutputData != null)
                    {
                        string header = string.Format("HTTP/1.1 200 OK\r\n" +
                                "Content-Length: {0}\r\n" +
                                "Connection: close\r\n\r\n", request.OutputData.Length);
                        byte[] headArray = Encoding.UTF8.GetBytes(header);
                        await resp.WriteAsync(headArray, 0, headArray.Length);
                        await resp.WriteAsync(request.OutputData, 0, request.OutputData.Length);
                    }
                    else
                    {
                        notFound = true;
                    }

                }
                else
                {
                    notFound = true;
                }

                if (notFound)
                {
                    string header = string.Format("HTTP/1.1 404 Not Found\r\n" +
                            "Content-Length: 0\r\n" +
                            "Connection: close\r\n\r\n");
                    byte[] headArray = Encoding.UTF8.GetBytes(header);
                    await resp.WriteAsync(headArray, 0, headArray.Length);
                }
                await resp.FlushAsync();
            }
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StringBuilder request = new StringBuilder();
            using (IInputStream input = args.Socket.InputStream)
            {

                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }
            using (IOutputStream output = args.Socket.OutputStream)
            {
                string requestMethod = request.ToString().Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');
                await WriteResponseAsync(request.ToString(), args.Socket, requestParts[0], requestParts[1], output);
            }
        }
    }
}
