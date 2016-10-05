using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ThisData.Models
{
    [DataContract]
    public class EventsResult
    {
        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "results")]
        public List<EnrichedEvent> Results { get; set; }
    }
}
