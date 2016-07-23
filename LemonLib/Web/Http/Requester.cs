using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LemonLib.Web.Http
{
    public struct HttpAuthorization
    {
        public string Schema { get; set; }
        public string Parameter { get; set; }
        public HttpAuthorization(string schema, string parameter)
        {
            Schema = schema;
            Parameter = parameter;
        }
    }

    public class Requester
    {
        
        
        public enum RequestType { Get, Post, Put, Delete };

        public string Encoding = null;
        public string Referer = null;
        public string UserAgent = null;
        public string ContentType = null;

        public HttpAuthorization? Authorization = null;
        public Content Cookies = new Content();
        public Content Headers = new Content();

        public void SetAuthUser(string user, string password)
        {
            SetAuth("user", user + ":" + password);
        }

        public void SetAuthKerberos(string parameters)
        {
            SetAuth("kerberos", parameters);
        }

        public void SetAuthBasic(string user, string password = null)
        {
            SetAuth("basic", user + (password != null ? ":" + password : ""));
        }

        public void SetAuth(string schema, string parameters = null)
        {
            this.Authorization = new HttpAuthorization(schema, Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(parameters)));
        }

        public async Task<Response> GetAsync(string url)
        {
            return await RequestAsync(url, RequestType.Get);
        }

        public async Task<Response> PostAsync(string url, Content data = null)
        {
            return await RequestAsync(url, RequestType.Post, data);
        }

        public async Task<Response> PutAsync(string url, Content data = null)
        {
            return await RequestAsync(url, RequestType.Put, data);
        }

        public async Task<Response> DeleteAsync(string url)
        {
            return await RequestAsync(url, RequestType.Delete);
        }

        public async Task<Response> RequestAsync(string url, RequestType type, Content data = null)
        {
            HttpClient client = new HttpClient();
            System.Net.Http.MultipartFormDataContent contents = null;
            if (this.Cookies != null && this.Cookies.Count > 0)
            {
                client.DefaultRequestHeaders.Add("Cookie", this.Cookies.ToString(';'));
            }
            if (this.Headers != null && this.Headers.Count > 0)
            {
                foreach (var pair in this.Headers)
                    if (pair.Value != null)
                        client.DefaultRequestHeaders.Add(pair.Key, pair.Value.ToString());
            }
            if (this.Encoding != null)
            {
                client.DefaultRequestHeaders.Add("Encoding", this.Encoding);
            }
            if (this.Referer != null)
            {
                client.DefaultRequestHeaders.Add("Referer", this.Referer);
            }
            if (this.UserAgent != null)
            {
                client.DefaultRequestHeaders.Add("User-Agent", this.UserAgent);
            }
            if (this.ContentType != null)
            {
                client.DefaultRequestHeaders.Add("Content-Type", this.ContentType);
            }
            if (this.Authorization != null)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(this.Authorization.Value.Schema, this.Authorization.Value.Parameter);
            }
            if (data != null && data.Count > 0)
            {
                contents = new MultipartFormDataContent();
                foreach (var pair in data)
                {
                    if (pair.Value.GetType() == typeof(byte[]))
                    {
                        contents.Add(new ByteArrayContent(pair.Value as byte[]), pair.Key);
                    }
                    else if (pair.Value.GetType() == typeof(StorageFile))
                    {
                        contents.Add(new StreamContent(await (pair.Value as StorageFile).OpenStreamForReadAsync()), pair.Key, (pair.Value as StorageFile).Name);
                    }
                    else
                    {
                        contents.Add(new StringContent(pair.Value.ToString()), pair.Key);
                    }
                }
            }
            HttpResponseMessage response = null;
            try
            {
                switch (type)
                {
                    case RequestType.Get:
                        response = await client.GetAsync(url);
                        break;
                    case RequestType.Post:
                        response = await client.PostAsync(url, contents);
                        break;
                    case RequestType.Put:
                        response = await client.PutAsync(url, contents);
                        break;
                    case RequestType.Delete:
                        response = await client.DeleteAsync(url);
                        break;
                }
                if (response == null)
                {
                    return null;
                }
                return new Response(response, this.Encoding);
            }
            catch (Exception ex)
            {
                return new Response(ex, this.Encoding);
            }
        }
    }
}
