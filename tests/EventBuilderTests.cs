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
            var ev = EventBuilder.Build(_request, Verbs.LOG_IN);
            StringAssert.AreEqualIgnoringCase("anonymous", ev.User.Id);
        }

        [Test]
        public void CanBuildEventWithAllProperties()
        {
            _request.Cookies.Add(new HttpCookie(Defaults.JavascriptCookieName, "cookie-monster"));

            var ev = EventBuilder.Build(
                request: _request, 
                verb: Verbs.LOG_IN,
                userId: "1234",
                name: "John Titor",
                email: "john.titor@thisdata.com",
                mobile: "555.123.4567",
                source: "Titor Inc",
                logoUrl: "http://thisdata.com/titor.png",
                sessionId: "999session",
                cookieExpected: true);

            StringAssert.AreEqualIgnoringCase("log-in", ev.Verb);
            StringAssert.AreEqualIgnoringCase(null, ev.UserAgent);
            StringAssert.AreEqualIgnoringCase("1234", ev.User.Id);
            StringAssert.AreEqualIgnoringCase("John Titor", ev.User.Name);
            StringAssert.AreEqualIgnoringCase("555.123.4567", ev.User.Mobile);
            StringAssert.AreEqualIgnoringCase("Titor Inc", ev.Source.Name);
            StringAssert.AreEqualIgnoringCase("http://thisdata.com/titor.png", ev.Source.LogoUrl);
            StringAssert.AreEqualIgnoringCase("999session", ev.Session.Id);
            Assert.IsTrue(ev.Session.CookieExpected);
            StringAssert.AreEqualIgnoringCase("cookie-monster", ev.Session.CookieId);
        }

        [Test]
        public void CanBuildEventWithoutOptionalProperties()
        {
            var ev = EventBuilder.Build(
                request: _request,
                verb: Verbs.LOG_IN);

            StringAssert.AreEqualIgnoringCase("log-in", ev.Verb);
            StringAssert.AreEqualIgnoringCase(null, ev.UserAgent);
        }

        [Test]
        public void CanBuildEventWithoutCookie()
        {
            _request.Cookies.Clear();

            var ev = EventBuilder.Build(
                request: _request,
                verb: Verbs.LOG_IN);

            StringAssert.AreEqualIgnoringCase(null, ev.Session.CookieId);
        }
    }
}
