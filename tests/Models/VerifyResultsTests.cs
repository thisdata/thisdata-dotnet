using System;
using System.Web;
using System.IO;

using NUnit.Framework;

using ThisData;

namespace ThisData.Net.Tests
{
    [TestFixture]
    class VerifyResultTests
    {
        private string _payload;

        [SetUp]
        public void Setup()
        {
            _payload = "{\"score\":0.900,\"risk_level\":\"red\",\"triggers\":[\"Accessed from the Tor network\", \"China is a new country\"],\"messages\":[]}";
        }

        [Test]
        public void CanDeserializeRiskyPayload()
        {
            var payload = SimpleJson.DeserializeObject<ThisData.Models.VerifyResult>(_payload, new DataContractJsonSerializerStrategy());

            Assert.AreEqual(0.900, payload.Score);
            Assert.AreEqual("red", payload.RiskLevel);
            Assert.AreEqual(2, payload.Triggers.Length);
            Assert.AreEqual("Accessed from the Tor network", payload.Triggers[0]);
            Assert.AreEqual(0, payload.Messages.Length);
        }
    }
}


