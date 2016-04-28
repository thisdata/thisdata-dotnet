using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class Profile
    {
        /// <summary>
        /// A unique identifier for the end user
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The end users full name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The end users email address for notifications
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// The end users mobile phone number for SMS notifications
        /// </summary>
        [DataMember(Name = "mobile")]
        public string Mobile { get; set; }    
    }
}