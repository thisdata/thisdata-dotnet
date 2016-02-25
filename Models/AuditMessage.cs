using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class AuditMessage
    {
        [DataMember (Name = "verb")]
        public string Verb { get; set; }

        [DataMember(Name = "ip")]
        public string IPAddress { get; set; }

        [DataMember(Name = "user_agent")]
        public string UserAgent { get; set; }

        [DataMember(Name = "user")]
        public UserDetails User { get; set; }
    }
}