using System;
using System.Web;
using System.IO;

using NUnit.Framework;

using ThisData;

namespace ThisData.Net.Tests
{
    [TestFixture]
    public class ClientTests
    {
        private HttpRequest _request;
        private string _signature;
        private Client _client;
        private string _payload;

        [SetUp]
        public void Setup()
        {
            _client = new ThisData.Client("");
            _request = new HttpRequest(string.Empty, "https://thisdata.com", string.Empty);
            _signature = "291264d1d4b3857e872d67b7587d3702b28519a0e3ce689d688372b7d31f6af484439a1885f21650ac073e48119d496f44dc97d3dc45106409d345f057443c6b";
            
            // Webhook Payload
            _payload = "{\"version\":1,\"was_user\":null,\"alert\":{\"id\":533879540905150463,\"description\":null}}";

            HttpContext.Current = new HttpContext(_request, new HttpResponse(new StringWriter()));
        }

        [Test]
        public void ValidateWebhook_WithValidSecret()
        {
            string secret = "hello";

            Assert.IsTrue(_client.ValidateWebhook(secret, _signature, _payload));
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
        public void GetSessionId_WhenNoSessionAvailable()
        {
            string id = _client.GetSessionId();

            Assert.AreEqual(string.Empty, id);
        }
    }
}
