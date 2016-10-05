namespace ThisData
{
    public class Defaults
    {
        public static string BaseUrl = "https://api.thisdata.com/";
        public static string EventsEndpoint = "/v1/events";
        public static string VerifyEndpoint = "/v1/verify";

        /// <summary>
        /// ThisData's JS library (optional) adds a cookie with this name
        /// </summary>
        public static string JavascriptCookieName = "__tdli";
    }
}