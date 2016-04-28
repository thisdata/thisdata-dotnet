using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class WebhookPayload : Event
    {
        [DataMember(Name = "version")]
        public int Version { get; set; }

        [DataMember(Name = "was_user")]
        public bool? WasUser { get; set; }

        [DataMember(Name = "alert")]
        public Alert Alert { get; set; }
    }
}
