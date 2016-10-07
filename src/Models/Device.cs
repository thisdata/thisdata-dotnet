using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class Device
    {
        /// <summary>
        /// A unique device identifier. Typically used for tracking mobile devices.
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "user_agent")]
        public string UserAgent { get; private set; }

        [DataMember(Name = "browser")]
        public string Browser { get; private set; }

        [DataMember(Name = "platform")]
        public string Platform { get; private set; }

        [DataMember(Name = "os")]
        public string OS { get; private set; }

        [DataMember(Name = "mobile")]
        public bool Mobile { get; private set; }

        [DataMember(Name = "bot")]
        public bool Bot { get; private set; }

        [DataMember(Name = "version")]
        public int[] Version { get; private set; }

        [DataMember(Name = "comment")]
        public string[] Comment { get; private set; }
    }
}