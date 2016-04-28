using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class Alert
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}