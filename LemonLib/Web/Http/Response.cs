using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LemonLib.Web.Http
{
    public class Response
    {
        protected byte[] Data { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public Content Headers { get; private set; }
        public Content Cookies { get; private set; }
        public Encoding Encoding { get; private set; }
        public Exception Exception { get; private set; }

        public Response(HttpResponseMessage response, string encoding)
        {
            this.Cookies = new Content();
            this.Encoding = Encoding.GetEncoding(encoding ?? "UTF-8");
            this.Headers = new Content();
            this.Exception = null;
            this.StatusCode = response.StatusCode;
            LoadContents(response);
        }

        public Response(Exception exception, string encoding)
        {
            this.Cookies = new Content();
            this.Encoding = Encoding.GetEncoding(encoding ?? "UTF-8");
            this.Headers = new Content();
            this.Exception = exception;
            this.StatusCode = 0;
        }

        public byte[] GetBytes()
        {
            return Data;
        }

        public string GetString()
        {
            if (Data == null)
                return null;
            return Encoding.GetString(Data);
        }

        protected async Task LoadContents(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Data = await response.Content.ReadAsByteArrayAsync();
            }
            if (response.Headers != null)
            {
                foreach (var pair in response.Headers)
                {
                    if (pair.Key.Equals("Set-Cookie"))
                    {
                        foreach (var cookie in pair.Value)
                        {
                            if (cookie.Contains('='))
                            {
                                string[] splitCookie = cookie.Split('=');
                                Cookies.Add(splitCookie[0], splitCookie[1]);
                            }
                            else
                            {
                                Cookies.Add(cookie);
                            }
                        }
                    }
                    Headers[pair.Key] = string.Join(";", pair.Value);
                }

            }
        }
    }
}
