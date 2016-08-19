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
        private static Event _currentAuditMessage;

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
        public void Track(string verb, string userId = "", string name = "", string email = "", string mobile = "", string source = "",
            string logoUrl = "", string sessionId = "", bool cookieExpected = false)
        {
            _currentAuditMessage = BuildAuditMessage(verb, userId, name, email, mobile, source, logoUrl, sessionId, cookieExpected);
            Send(_currentAuditMessage);
        }

        /// <summary>
        /// Tracks an event to the ThisData API
        /// </summary>
        /// <param name="message">Valid ThisData event</param>
        public void Track(Event message)
        {
            Send(message);
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
        public void TrackAsync(string verb, string userId = "", string name = "", string email = "", string mobile = "", string source = "", 
            string logoUrl = "", string sessionId = "", bool cookieExpected = false)
        {
            Event message = BuildAuditMessage(verb, userId, name, email, mobile, source, logoUrl, sessionId, cookieExpected);

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
        /// <returns>The webhook body as json string</returns>
        public WebhookPayload GetWebhookPayload()
        {
            return DeserializeWebhookPayload(GetHttpRequestBody());
        }

        /// <summary>
        /// The validated webhook body content from a POST request
        /// </summary>
        /// <param name="secret">A secret string entered via ThisData settings page, Defaults to API key</param>
        /// <returns>Null if invalid or the webhook body as json string if valid</returns>
        public WebhookPayload GetWebhookPayload(string secret = "")
        {
            if (string.IsNullOrEmpty(secret))
            {
                secret = _apiKey;
            }

            HttpRequest request = GetHttpRequest();
            string signature = request.Headers["X-Signature"];
            string json = GetHttpRequestBody();

            if (ValidateWebhook(secret, signature, json))
            {
                // Its valid
                return DeserializeWebhookPayload(json);
            }

            return null;
        }

        /// <summary>
        /// Returns the current session id if available
        /// </summary>
        /// <returns></returns>
        public string GetSessionId()
        {
            string id = String.Empty;

            try
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    id = context.Session.SessionID;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Error retrieving SessionId {0}", ex.Message);
            }

            return id;
        }

        #region Private

        private WebhookPayload DeserializeWebhookPayload(string json)
        {
            return SimpleJson.DeserializeObject<WebhookPayload>(json, new DataContractJsonSerializerStrategy());
        }

        private Event BuildAuditMessage(string verb, string userId = "", string name = "", string email = "", string mobile = "", string source = "",
            string logoUrl = "", string sessionId = "", bool cookieExpected = false)
        {
            Event message = null;
            HttpRequest request = GetHttpRequest();

            if (String.IsNullOrEmpty(userId))
            {
                userId = GetSessionId();
            }

            if (request != null)
            {
                message = EventBuilder.Build(request, verb, userId, name, email, mobile, source, logoUrl, sessionId, cookieExpected);
            }

            return message;
        }

        private string GetHttpRequestBody()
        {
            HttpRequest request = GetHttpRequest();
            string body = "";

            if (request != null)
            {
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    body = reader.ReadToEnd();
                }
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
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Error retrieving HttpRequest {0}", ex.Message);
                }
            }

            return request;
        }

        protected WebClient CreateWebClient()
        {
          var client = new WebClient();
          client.Headers.Add("content-type", "application/json; charset=utf-8");
          client.Encoding = System.Text.Encoding.UTF8;

          return client;
        }

        private void Send(Event message)
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
