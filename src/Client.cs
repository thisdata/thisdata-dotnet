using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Specialized;

using ThisData.Models;

namespace ThisData
{
    public class Client 
    {
        private string _apiKey;
        private string _trackEndpoint;
        private string _verifyEndpoint;

        [ThreadStatic]
        private static Event _currentAuditMessage;

        private IHttpTransport _transport;

        public Client(string apiKey, IHttpTransport transport)
        {
            _apiKey = apiKey;
            _transport = transport;
        }

        public Client(string apiKey)
            : this(apiKey, new HttpsTransport(apiKey, Defaults.BaseUrl))
        {
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
        /// <param name="sessionId">If you use a database to track sessions, you can send us the session ID</param>
        /// <param name="cookieExpected">Send true when using our optional Javascript tracking library, and we'll know to expect a cookie</param>
        /// <param name="deviceId">A unique device identifier. Typically used for tracking mobile devices.</param>
        public void Track(string verb, string userId = "", string name = "", string email = "", string mobile = "", string source = "",
            string logoUrl = "", string sessionId = "", bool cookieExpected = false, string deviceId = "")
        {
            _currentAuditMessage = BuildAuditMessage(verb, userId, name, email, mobile, source, logoUrl, sessionId, cookieExpected, deviceId);
            _transport.Post(Defaults.EventsEndpoint, _currentAuditMessage);
        }

        /// <summary>
        /// Tracks an event to the ThisData API
        /// </summary>
        /// <param name="message">Valid ThisData event</param>
        public void Track(Event message)
        {
            _transport.Post(Defaults.EventsEndpoint, _currentAuditMessage);
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
        /// <param name="sessionId">If you use a database to track sessions, you can send us the session ID</param>
        /// <param name="cookieExpected">Send true when using our optional Javascript tracking library, and we'll know to expect a cookie</param> 
        /// <param name="deviceId">A unique device identifier. Typically used for tracking mobile devices.</param> 
        public void TrackAsync(string verb, string userId = "", string name = "", string email = "", string mobile = "", string source = "",
            string logoUrl = "", string sessionId = "", bool cookieExpected = false, string deviceId = "")
        {
            Event message = BuildAuditMessage(verb, userId, name, email, mobile, source, logoUrl, sessionId, cookieExpected, deviceId);

            ThreadPool.QueueUserWorkItem(c =>
            {
                try
                {
                    _currentAuditMessage = message;
                    _transport.Post(Defaults.EventsEndpoint, _currentAuditMessage);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("Error sending async audit message {0}", ex.Message));
                }
            });
        }

        /// <summary>
        /// Get a risk score for user based on current context
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="mobile"></param>
        /// <param name="source"></param>
        /// <param name="logoUrl"></param>
        /// <param name="sessionId"></param>
        /// <param name="cookieExpected"></param>
        /// <returns></returns>
        public VerifyResult Verify(string userId = "", string name = "", string email = "", string mobile = "", string source = "", 
            string sessionId = "", bool cookieExpected = false)
        {
            _currentAuditMessage = BuildAuditMessage(Verbs.VERIFY, userId, name, email, mobile, source, "", sessionId, cookieExpected);
            return Verify(_currentAuditMessage);
        }

        /// <summary>
        /// Get a risk score for user based on current context
        /// </summary>
        /// <param name="message">Current event context for the user</param>
        /// <returns></returns>
        public VerifyResult Verify(Event message)
        {
            _currentAuditMessage = message;
            _currentAuditMessage.Verb = Verbs.VERIFY;
            return _transport.Post<VerifyResult>(Defaults.VerifyEndpoint, _currentAuditMessage);
        }

        public EventsResult GetEvents(string userId = "", string[] verbs = null, string source = "", int limit = 50, int offset = 0, DateTime? after = null, DateTime? before = null)
        {
            QueryStringBuilder query = new QueryStringBuilder();
            query.Add("user_id", userId);
            query.Add("verbs", verbs);
            query.Add("source", source);
            query.Add("limit", limit);
            query.Add("offset", offset);
            query.Add("before", before);
            query.Add("after", after);

            return _transport.Get<EventsResult>("/events", query.Params);
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
            string logoUrl = "", string sessionId = "", bool cookieExpected = false, string deviceId = "")
        {
            Event message = null;
            HttpRequest request = GetHttpRequest();

            if (String.IsNullOrEmpty(userId))
            {
                userId = GetSessionId();
            }

            if (request != null)
            {
                message = EventBuilder.Build(request, verb, userId, name, email, mobile, source, logoUrl, sessionId, cookieExpected, deviceId);
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

        #endregion
    }
}
