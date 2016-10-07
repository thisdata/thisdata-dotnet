using System;
using System.Web;
using System.IO;
using System.Reflection;

using NUnit.Framework;

using ThisData;

namespace ThisData.Net.Tests
{
    [TestFixture]
    class EventsResultTests
    {
        private string _payload;

        [SetUp]
        public void Setup()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("ThisData.Net.Tests.Stub.event_results.json"))
            using (var reader = new StreamReader(stream))
            {
                _payload = reader.ReadToEnd();
            }
        }

        [Test]
        public void CanDeserializeEventsPayload()
        {
            var payload = SimpleJson.DeserializeObject<ThisData.Models.EventsResult>(_payload, new DataContractJsonSerializerStrategy());

            Assert.IsInstanceOf<ThisData.Models.EnrichedEvent>(payload.Results[0]);
            Assert.AreEqual(1, payload.Total);
            Assert.AreEqual("Chrome", payload.Results[0].Device.Browser);
            Assert.AreEqual("New Zealand", payload.Results[0].Location.Address.CountryName);
        }
    }
}


