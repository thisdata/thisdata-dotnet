using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Web;

using ThisData.Models;
using ThisData.Helpers;

namespace ThisData
{
    public class Client 
    {
        private string _apiKey;

        [ThreadStatic]
        private static AuditMessage _currentAuditMessage;

        public Client(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// Tracks an event to the ThisData API
        /// </summary>
        /// <param name="verb">The action taken by the user. eg. log-in</param>
        /// <param name="userId">A unique identifier for the user. If omitted LogonUserIdentity.Name will be used</param>
        /// <param name="name">The full name of the user</param>
        /// <param name="email">The users email address for sending notifications</param>
        /// <param name="mobile">The users mobile phone number for sending SMS notifications</param>
        public void Track(string verb, string userId = "", string name = "", string email = "", string mobile = "")
        {
            _currentAuditMessage = BuildAuditMessage(verb);
            Send(_currentAuditMessage);
        }

        /// <summary>
        /// Asyncronously tracks an event to the ThisData API
        /// </summary>
        /// <param name="verb">The action taken by the user. eg. log-in</param>
        /// <param name="userId">A unique identifier for the user. If omitted LogonUserIdentity.Name will be used</param>
        /// <param name="name">The full name of the user</param>
        /// <param name="email">The users email address for sending notifications</param>
        /// <param name="mobile">The users mobile phone number for sending SMS notifications</param>
        public void TrackAsync(string verb, string userId = "", string name = "", string email = "", string mobile = "")
        {
            AuditMessage message = BuildAuditMessage(verb);

            ThreadPool.QueueUserWorkItem(c =>
            {
                try
                {
                    _currentAuditMessage = message;
                    Send(_currentAuditMessage);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Error sending async audit message {0}", ex.Message));
                }
            });
        }

        #region Private

        private AuditMessage BuildAuditMessage(string verb, string userId = "", string name = "", string email = "")
        {
            AuditMessage message = null;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                HttpRequest request = null;
                try
                {
                    request = context.Request;
                }
                catch (HttpException ex)
                {
                    System.Diagnostics.Trace.WriteLine("Error retrieving HttpRequest {0}", ex.Message);
                }

                if (request != null)
                {
                    message = AuditMessageBuilder.Build(request, verb, name, email);
                }
            }

            return message;
        }

        protected WebClient CreateWebClient()
        {
          var client = new WebClient();
          client.Headers.Add("content-type", "application/json; charset=utf-8");
          client.Encoding = System.Text.Encoding.UTF8;

          return client;
        }

        private void Send(AuditMessage message)
        {
            string endpoint = string.Format("{0}?api_key={1}", Defaults.ApiEndpoint, _apiKey);
            string payload = null;

            try
            {
              payload = SimpleJson.SerializeObject(message);
            }
            catch (Exception ex)
            {
              System.Diagnostics.Trace.WriteLine(string.Format("Error serializing audit message {0}", ex.Message));
            }

            if (message != null)
            {
                try
                {
                    using (var client = CreateWebClient())
                    {
                        client.UploadString(endpoint, payload);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Error sending audit activity to ThisData {0}", ex.Message));
                }
            }
        }

        #endregion
    }
}
