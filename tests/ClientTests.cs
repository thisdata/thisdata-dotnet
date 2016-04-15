using System;
using NUnit.Framework;

using ThisData;

namespace ThisData.Net.Tests
{
    [TestFixture]
    public class ClientTests
    {
        string signature = "291264d1d4b3857e872d67b7587d3702b28519a0e3ce689d688372b7d31f6af484439a1885f21650ac073e48119d496f44dc97d3dc45106409d345f057443c6b";
        string payload = "{\"version\":1,\"was_user\":null,\"alert\":{\"id\":533879540905150463,\"description\":null}}";

        [Test]
        public void ValidateWebhook_WithValidSecret()
        {
            var client = new ThisData.Client("");
            string secret = "hello";

            Assert.IsTrue(client.ValidateWebhook(secret, signature, payload));
        }

        [Test]
        public void ValidateWebhook_WithInvalidSecret()
        {
            var client = new ThisData.Client("");
            string secret = "goodbye";

            Assert.IsFalse(client.ValidateWebhook(secret, signature, payload));
        }
    }
}
