using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemonLib.Web.Http
{
    public class Content : Dictionary<string, object>
    {
        public void Add(string key)
        {
            Add(key, null);
        }

        public override string ToString()
        {
            return ToString('&');
        }

        public string ToString(char splitter)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                var pair = this.ElementAt(i);
                if (pair.Value != null)
                    builder.Append(pair.Key + "=" + pair.Value);
                else
                    builder.Append(pair.Key);
                if (i < this.Count - 1)
                    builder.Append(splitter);
            }
            return builder.ToString();
        }
    }
}
