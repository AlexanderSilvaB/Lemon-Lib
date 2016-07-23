using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace LemonLib.Web.Http
{
    public class ServerRequest : EventArgs
    {
        public bool Handled { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public StreamSocket Socket { get; set; }
        public string Input { get; set; }
        public byte[] InputData { get; set; }
        public string Output { get; set; }
        public byte[] OutputData { get; set; }
    }
}
