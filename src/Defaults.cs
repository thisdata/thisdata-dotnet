namespace ThisData
{
    public class Defaults
    {
        public static string TrackEndpoint = "https://api.thisdata.com/v1/events.json";
        public static string VerifyEndpoint = "https://api.thisdata.com/v1/verify.json";

        /// <summary>
        /// ThisData's JS library (optional) adds a cookie with this name
        /// </summary>
        public static string JavascriptCookieName = "__tdli";
    }
}