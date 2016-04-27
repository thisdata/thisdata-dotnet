using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Web;
using System.Security.Cryptography;
using System.IO;

using ThisData.Models;

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
        /// <param name="source">Used to indicate the source of the event and override company or app name in audit log and notifications</param>
        /// <param name="logoUrl">Used to override logo used in email notifications</param>
        public void Track(string verb, string userId = "", string name = "", string email = "", string mobile = "", string source = "", string logoUrl = "")
        {
            _currentAuditMessage = BuildAuditMessage(verb, userId, name, email, mobile, source, logoUrl);
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
        /// <param name="source">Used to indicate the source of the event and override company or app name in audit log and notifications</param>
        /// <param name="logo_url">Used to override logo used in email notifications</param>
        public void TrackAsync(string verb, string userId = "", string name = "", string email = "", string mobile = "", string source = "", string logoUrl = "")
        {
            AuditMessage message = BuildAuditMessage(verb, userId, name, email, mobile, source, logoUrl);

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

        /// <summary>
        /// Validates a webhook payload using shared secret
        /// </summary>
        /// <param name="secret">A secret string entered via ThisData settings page</param>
        /// <returns></returns>
        public bool ValidateWebhook(string secret)
        {
            HttpRequest request = GetHttpRequest();
            string signature = request.Headers["X-Signature"];

            if (String.IsNullOrEmpty(signature))
            {
                // No signature used
                return true;
            }
            else
            {
                // Validate the signature
                return ValidateWebhook(secret, signature, GetHttpRequestBody(request));
            }
        }

        /// <summary>
        /// Validates a webhook payload using shared secret
        /// </summary>
        /// <param name="secret">A secret string entered via ThisData settings page</param>
        /// <param name="signature">A signature from the X-Signature header on a request</param>
        /// <param name="payload">The webhook body</param>
        /// <returns></returns>
        public bool ValidateWebhook(string secret, string signature, string payload)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] key = encoding.GetBytes(secret);
            byte[] payloadBytes = encoding.GetBytes(payload);

            using (HMACSHA512 hmac = new HMACSHA512(key))
            {
                byte[] hmacBytes = hmac.ComputeHash(payloadBytes);
                string payloadSignature = BitConverter.ToString(hmacBytes).Replace("-", "");
                return signature.Equals(payloadSignature, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// The webhook body content from a POST request
        /// </summary>
        /// <returns></returns>
        public string GetWebhookPayload()
        {
            return GetHttpRequestBody(GetHttpRequest());
        }

        #region Private

        private AuditMessage BuildAuditMessage(string verb, string userId = "", string name = "", string email = "", string mobile = "", string source = "", string logoUrl = "")
        {
            AuditMessage message = null;
            HttpRequest request = GetHttpRequest();

            if (String.IsNullOrEmpty(userId))
            {
                userId = GetSessionId();
            }

            if (request != null)
            {
                message = AuditMessageBuilder.Build(request, verb, userId, name, email, mobile, source, logoUrl);
            }

            return message;
        }

        private string GetHttpRequestBody(HttpRequest request)
        {
            string body = "";
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }

        private HttpRequest GetHttpRequest()
        {
            HttpRequest request = null;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                try
                {
                    request = context.Request;
                }
                catch (HttpException ex)
                {
                    System.Diagnostics.Trace.WriteLine("Error retrieving HttpRequest {0}", ex.Message);
                }
            }

            return request;
        }

        public string GetSessionId()
        {
            string id = String.Empty;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                try
                {
                    id = context.Session.SessionID;
                }
                catch (NullReferenceException ex)
                {
                    System.Diagnostics.Trace.WriteLine("Error retrieving SessionId {0}", ex.Message);
                }
            }

            return id;
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
