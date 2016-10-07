using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace ThisData
{
    public class QueryStringBuilder
    {
        private NameValueCollection _params; 

        public QueryStringBuilder()
        {
            _params = new NameValueCollection();
        }

        public void Add(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _params.Add(key, value);
            }
        }

        public void Add(string key, int value)
        {
            Add(key, value.ToString());
        }

        public void Add(string key, DateTime? value)
        {
            if (value.HasValue)
            {
                Add(key, value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            }
        }

        public void Add(string key, string[] values)
        {
            if (values != null)
            {
                foreach (string value in values)
                {
                    Add(string.Format("{0}[]", key), value);
                }
            }
        }

        public NameValueCollection Params
        {
            get
            {
                return _params;
            }
        }
    }
}
