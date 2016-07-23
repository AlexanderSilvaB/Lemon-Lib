using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using LemonLib.Web.Http;

namespace LemonLib.Web
{
    public class RestFul : Requester
    {
        
        protected Uri uri;

        public Content Data = new Content();

        public RestFul(Uri uri)
        {
            this.uri = uri;
        }

        public void SetData(Content data)
        {
            if (data != null)
                this.Data = data;
        }

        public void SetData(params object[] data)
        {
            this.Data.Clear();
            int len = data.Length;
            int pairs = len / 2;
            for (int i = 0; i < pairs * 2; i += 2)
            {
                this.Data.Add(data[i].ToString(), data[i + 1].ToString());
            }
            if (len > pairs * 2)
            {
                this.Data.Add(data[len - 1].ToString(), null);
            }
        }

        protected async new Task<Response> RequestAsync(string url, RequestType type, Content data = null)
        {
            return await base.RequestAsync(url, type, null);
        }

        protected async new Task<Response> GetAsync(string url = "")
        {
            return await base.GetAsync(url);
        }

        public async Task<Response> GetAsync(string endpoint = "", Content data = null)
        {
            string url = GetUrl(endpoint, data);
            return await base.GetAsync(url);
        }

        public async new Task<Response> PostAsync(string endpoint = "", Content data = null)
        {
            string url = GetUrl(endpoint);
            return await base.PostAsync(url, data);
        }

        public async new Task<Response> PutAsync(string endpoint = "", Content data = null)
        {
            string url = GetUrl(endpoint);
            return await base.PutAsync(url, data);
        }

        protected async new Task<Response> DeleteAsync(string url)
        {
            return await base.DeleteAsync(url);
        }

        public async Task<Response> DeleteAsync(string endpoint = "", Content data = null)
        {
            string url = GetUrl(endpoint, data);
            return await base.DeleteAsync(url);
        }

        protected string GetUrl(string endpoint = "", Content data = null)
        {
            StringBuilder builder = new StringBuilder(uri.ToString());
            if (endpoint.Length > 0 && !uri.ToString().EndsWith("/") && !endpoint.StartsWith("/"))
                builder.Append("/");
            if (endpoint.Length > 0 && uri.ToString().EndsWith("/") && endpoint.StartsWith("/"))
                endpoint = endpoint.Substring(1);
            builder.Append(endpoint);
            if ((this.Data != null && this.Data.Count > 0) || (data != null && data.Count > 0))
            {
                bool first = false, added = false;
                builder.Append("?");
                if (data != null && data.Count > 0)
                {
                    builder.Append(data.ToString());
                    first = true;
                }
                if (this.Data != null && this.Data.Count > 0)
                {
                    if (first && !added)
                    {
                        builder.Append("&");
                        added = true;
                        first = true;
                    }
                    builder.Append(this.Data.ToString());
                    first = true;
                }
            }
            return builder.ToString();
        }

    }
}
