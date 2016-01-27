using Newtonsoft.Json;

namespace ThisData.Models
{
    public class UserInfo
    {
        public UserInfo(){}
        
        public UserInfo(string id)
        {
            this.Id = id;
        }
        
        [JsonPropertyAttribute(PropertyName = "id")]
        public string Id;                
        
        [JsonPropertyAttribute(PropertyName = "name")]
        public string Name;
        
        [JsonPropertyAttribute(PropertyName = "email")]
        public string Email;        
    }
}