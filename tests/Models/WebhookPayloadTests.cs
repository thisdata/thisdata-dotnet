using System;
using System.Web;
using System.IO;

using NUnit.Framework;

using ThisData;

namespace ThisData.Net.Tests
{
    [TestFixture]
    class WebhookPayloadTests
    {
        private string _wasNotMePayload;
        private string _payload;

        [SetUp]
        public void Setup()
        {
            // Webhook Payloads
            _payload = "{\"version\":1,\"was_user\":null,\"alert\":{\"id\":533879540905150463,\"description\":null}}";

            _wasNotMePayload = "{\"version\" :  1,\"was_user\" : false,\"alert\" : {\"id\" : 11223344,\"description\" : \"Eve Smith logged in from a new location\" },"
                             + "\"ip\" : \"1.2.3.4\",\"verb\" : \"log-in\",\"user_agent\" : \"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_3)...\",\"user\" : {\"id\" : "
                             + "\"112233\",\"name\" : \"Eve Smith\",\"email\" : \"eve.smith@domain.com\"}}";
        }

        [Test]
        public void CanDeserializeWasNotMeWebhookPayload()
        {
            var payload = SimpleJson.DeserializeObject<ThisData.Models.WebhookPayload>(_wasNotMePayload, new DataContractJsonSerializerStrategy());

            Assert.AreEqual(false, payload.WasUser);
            Assert.AreEqual(1, payload.Version);

            // Alert
            Assert.AreEqual("Eve Smith logged in from a new location", payload.Alert.Description);
            Assert.AreEqual(11223344, payload.Alert.Id);
            
            // User
            Assert.AreEqual("112233", payload.User.Id);
            Assert.AreEqual("Eve Smith", payload.User.Name);
            Assert.AreEqual("eve.smith@domain.com", payload.User.Email);

            // Event
            Assert.AreEqual("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_3)...", payload.UserAgent);
            Assert.AreEqual("log-in", payload.Verb);
            Assert.AreEqual("1.2.3.4", payload.IPAddress);
        }

        [Test]
        public void CanDeserializePayloadWithNullWasUserField()
        {
            var payload = SimpleJson.DeserializeObject<ThisData.Models.WebhookPayload>(_payload, new DataContractJsonSerializerStrategy());

            Assert.AreEqual(null, payload.WasUser);
        }
    }
}


