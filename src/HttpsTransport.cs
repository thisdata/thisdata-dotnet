using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Specialized;

namespace ThisData
{
    public class HttpsTransport : IHttpTransport
    {
        private string _baseUrl;
        private string _apiKey;

        public HttpsTransport(string apiKey, string baseUrl)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
        }

        public T Get<T>(string url, NameValueCollection queryParams = null)
        {
            try
            {
                using (var client = CreateWebClient())
                {
                    if (queryParams != null)
                    {
                        foreach (string key in queryParams)
                        {
                            client.QueryString[key] = queryParams[key];
                        }
                    }

                    string res = client.DownloadString(url);
                    return SimpleJson.DeserializeObject<T>(res, new DataContractJsonSerializerStrategy());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Error sending audit activity to ThisData {0}", ex.Message));
                return default(T);
            }
        }
        
        public void Post(string url, object data)
        {
            try
            {
                using (var client = CreateWebClient())
                {
                    string res = client.UploadString(url, Stringify(data));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Error sending audit activity to ThisData {0}", ex.Message));
            }
        }

        public T Post<T>(string url, object data)
        {
            try
            {
                using (var client = CreateWebClient())
                {
                    string res = client.UploadString(url, Stringify(data));
                    return SimpleJson.DeserializeObject<T>(res, new DataContractJsonSerializerStrategy());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Error sending audit activity to ThisData {0}", ex.Message));
                return default(T);
            }            
        }

        #region Private

        private string Stringify(object data)
        {
            string payload = "";

            try
            {
                payload = SimpleJson.SerializeObject(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Error serializing audit message {0}", ex.Message));
            }

            return payload;
        }

        protected WebClient CreateWebClient()
        {
            var client = new WebClient();

            client.BaseAddress = _baseUrl;

            client.Headers.Add("User-Agent", "thisdata-dotnet");
            client.Headers.Add("Content-Type", "application/json");
            client.Encoding = System.Text.Encoding.UTF8;

            client.QueryString = new NameValueCollection();
            client.QueryString.Add("api_key", _apiKey);

            return client;
        }

        #endregion
    }
}
