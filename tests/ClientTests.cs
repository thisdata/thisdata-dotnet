using System;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;
using Moq;

using ThisData;
using ThisData.Models;

namespace ThisData.Net.Tests
{
    [TestFixture]
    public class ClientTests
    {
        private HttpRequest _request;
        private string _signature;
        private Client _client;
        private string _payload;
        private string _apiKey;

        [SetUp]
        public void Setup()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            var httpsMock = new Mock<IHttpTransport>();
            httpsMock.Setup(transport => transport.Post(It.IsAny<string>(), It.IsAny<Event>()));
            httpsMock.Setup(transport => transport.Post<VerifyResult>(It.IsAny<string>(), It.IsAny<Event>()))
                .Returns(new VerifyResult() { Score = 0.9 });

            _apiKey = "fake-key";
            _client = new ThisData.Client(_apiKey, httpsMock.Object);
            _request = new HttpRequest(string.Empty, "https://thisdata.com", string.Empty);
            _signature = "291264d1d4b3857e872d67b7587d3702b28519a0e3ce689d688372b7d31f6af484439a1885f21650ac073e48119d496f44dc97d3dc45106409d345f057443c6b";
            
            // Webhook Payload
            _payload = "{\"version\":1,\"was_user\":null,\"alert\":{\"id\":533879540905150463,\"description\":null}}";

            HttpContext.Current = new HttpContext(_request, new HttpResponse(new StringWriter()));
        }

        private string GetSignature(string payload, string secret)
        {
            string payloadSignature;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] key = encoding.GetBytes(secret);
            byte[] payloadBytes = encoding.GetBytes(payload);

            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] hmacBytes = hmac.ComputeHash(payloadBytes);
                payloadSignature = BitConverter.ToString(hmacBytes).Replace("-", "").ToLower();
            }

            return payloadSignature;
        }

        [Test]
        public void SignParams_WithSecret()
        {
            string secret = "hello";
            string payload = "{\"user\":{\"id\":\"john.titor\",\"email\":\"john.titor@thisdata.com\"}}";

            string expectedSignature = GetSignature(payload, secret);
            string signature = _client.SignParams(payload, secret);

            Assert.AreEqual(expectedSignature, signature);
        }

        [Test]
        public void SignParams_WithOutSecret()
        {
            string payload = "{\"user\":{\"id\":\"john.titor\",\"email\":\"john.titor@thisdata.com\"}}";

            string expectedSignature = GetSignature(payload, _apiKey);
            string signature = _client.SignParams(payload);

            Assert.AreEqual(expectedSignature, signature);
        }

        [Test]
        public void ValidateWebhook_WithValidSecret()
        {
            string secret = "hello";

            Assert.IsTrue(_client.ValidateWebhook(secret, _signature, _payload));
        }

        [Test]
        public void Verify_WithParams()
        {
            Assert.IsInstanceOf<VerifyResult>(_client.Verify(userId: "123456", name: "John Titor"));
        }

        [Test]
        public void ValidateWebhook_WithInvalidSecret()
        {
            string secret = "goodbye";

            Assert.IsFalse(_client.ValidateWebhook(secret, _signature, _payload));
        }

        [Test]
        public void Track_EventWithUserId()
        {
            Assert.DoesNotThrow(() => _client.Track(Verbs.LOG_IN, userId: "123456"));
        }

        [Test]
        public void Track_EventWithoutUserId()
        {
            Assert.DoesNotThrow(() => _client.Track(Verbs.LOG_IN));
        }

        [Test]
        public void GetEvents_WithoutParams()
        {
            Assert.DoesNotThrow(() => _client.GetEvents());
        }

        [Test]
        public void GetEvents_WithtParams()
        {
            var verbs = new string[] { "log-in", "log-in-denied" };

            Assert.DoesNotThrow(() => _client.GetEvents(userId: "johntitor", verbs: verbs));
        }

        [Test]
        public void GetSessionId_WhenNoSessionAvailable()
        {
            string id = _client.GetSessionId();

            Assert.AreEqual(string.Empty, id);
        }
    }
}
