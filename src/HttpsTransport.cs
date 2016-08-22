using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Web;
using System.Security.Cryptography;
using System.IO;

namespace ThisData
{
    public class HttpsTransport : IHttpTransport
    {
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
            client.Headers.Add("content-type", "application/json; charset=utf-8");
            client.Encoding = System.Text.Encoding.UTF8;

            return client;
        }

        #endregion
    }
}
