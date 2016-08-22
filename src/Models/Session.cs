using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class Session
    {
        /// <summary>
        /// If you use a database to track sessions, you can send us the session ID
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// When using our Javascript tracking library, a cookie is created. The cookie's ID should be sent using this reserved key name
        /// </summary>
        [DataMember(Name = "td_cookie_id")]
        public string CookieId { get; set; }

        /// <summary>
        /// Send true when using our optional Javascript tracking library, and we'll know to expect a cookie
        /// </summary>
        [DataMember(Name = "td_cookie_expected")]
        public bool CookieExpected { get; set; }
    }
}