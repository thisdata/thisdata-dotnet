
using System;
using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class Event
    {
        public Event()
        {
            this.User = new Profile();
            this.Session = new Session();
            this.Device = new Device();
        }

        [DataMember(Name = "verb")]
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

        [DataMember(Name = "device")]
        public Device Device { get; set; }
    }
    
    [DataContract]
    public class EnrichedEvent : Event
    {
        [DataMember(Name = "id")]
        public string Id { get; private set; }

        [DataMember(Name = "published")]
        public DateTime Published { get; private set; }

        [DataMember(Name = "location")]
        public Location Location { get; private set; }

        [DataMember(Name = "overall_score")]
        public double RiskScore { get; private set; }

        [DataMember(Name = "raw")]
        public object Raw { get; private set; }
    }
}

