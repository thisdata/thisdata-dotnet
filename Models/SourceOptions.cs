using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class SourceOptions
    {
        /// <summary>
        /// Company or app name to use in audit log and notifications
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Logo to use in email notifications
        /// </summary>
        [DataMember(Name = "logo_url")]
        public string LogoUrl { get; set; }
    }
}