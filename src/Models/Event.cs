using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class Event
    {
        public Event()
        {
            this.Session = new Session();
        }

        [DataMember (Name = "verb")]
        public string Verb { get; set; }

        [DataMember(Name = "ip")]
        public string IPAddress { get; set; }

        [DataMember(Name = "user_agent")]
        public string UserAgent { get; set; }

        [DataMember(Name = "user")]
        public Profile User { get; set; }

        [DataMember(Name = "source")]
        public SourceOptions Source { get; set; }

        [DataMember(Name = "session")]
        public Session Session { get; set; }
    }
}

