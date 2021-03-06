﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace ThisData
{
    public interface IHttpTransport
    {
        void Post(string url, object data);

        T Post<T>(string url, object data);

        T Get<T>(string url, NameValueCollection  queryParams);
    }
}
