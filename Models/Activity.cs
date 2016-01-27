using Newtonsoft.Json;

namespace ThisData.Models
{
    public class Activity
    {
        public Activity(){}
        
        public Activity(string verb, string userId, string ip)
        {
            this.Verb = verb;
            this.IP = ip;
            this.User = new UserInfo(userId);
        }

        [JsonPropertyAttribute(PropertyName = "verb")]
        public string Verb;
        
        [JsonPropertyAttribute(PropertyName = "ip")]
        public string IP;
        
        [JsonPropertyAttribute(PropertyName = "user_agent")]
        public string UserAgent;
        
        [JsonPropertyAttribute(PropertyName = "user")]
        public UserInfo User;             
    }
}