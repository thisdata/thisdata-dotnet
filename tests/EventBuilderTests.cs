using System;
using System.Web;
using System.IO;

using NUnit.Framework;

using ThisData;

namespace ThisData.Net.Tests
{
    [TestFixture]
    public class EventBuilderTests
    {
        private HttpRequest _request;

        [SetUp]
        public void Setup()
        {
            _request = new HttpRequest(string.Empty, "https://thisdata.com", string.Empty);

            HttpContext.Current = new HttpContext(_request, new HttpResponse(new StringWriter()));
        }

        [Test]
        public void IsValidIpAddress_WithInvalidIpAddress()
        {
            Assert.IsFalse(EventBuilder.IsValidIpAddress("1234.123.123.123"));
        }

        [Test]
        public void ValidateWebhook_WithValidIpAddress()
        {
            Assert.IsTrue(EventBuilder.IsValidIpAddress("123.123.123.123"));
        }

        [Test]
        public void GetUserDetails_SetsAnonymousUserIdWhenNoneProvided()
        {
            var user = EventBuilder.GetUserDetails(_request);
            StringAssert.AreEqualIgnoringCase("anonymous", user.Id);
        }

        [Test]
        public void GetUserDetails_UsesAllSuppliedValues()
        {
            string userId = "1234";
            string name = "Bird";
            string email = "bird@thisdata.com";
            string mobile = "123456789";
            
            var user = EventBuilder.GetUserDetails(_request, userId, name, email, mobile);

            StringAssert.AreEqualIgnoringCase(userId, user.Id);
            StringAssert.AreEqualIgnoringCase(name, user.Name);
            StringAssert.AreEqualIgnoringCase(email, user.Email);
            StringAssert.AreEqualIgnoringCase(mobile, user.Mobile);
        }
    }
}
