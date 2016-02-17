using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using System.Web;

using Newtonsoft.Json;
using ThisData.Models;
using ThisData.Helpers;

namespace ThisData
{
    public class Client 
    {
        private string _apiKey;
        
        public Client(string apiKey)
        {
            _apiKey = apiKey;
        }
        
        /// <summary>
        /// Sends login metadata for anomaly detection
        /// </summary>
        /// <param name="userId">A unique identifier for a user. ID or email etc</param>
        /// <param name="ip">The users IP address</param>
        /// <param name="name">The users full name</param>
        /// <param name="email">The users email address</param>
        /// <param name="userAgent">The browsers user_agent string</param>
        public async Task<HttpResponseMessage> Login(string userId, string ip, string name = "", string email = "", string userAgent = "")
        {
            return await Track(Verbs.LOG_IN, userId, ip, name, email, userAgent);
        }

        /// <summary>
        /// Sends login metadata for anomaly detection
        /// </summary>
        /// <param name="userId">A unique identifier for a user. ID or email etc</param>
        /// <param name="context">Pass in HttpContext and it will convienently extract IP and UserAgent</param>
        /// <param name="name">Optional - The users full name</param>
        /// <param name="email">Optional - The users email address</param>
        public async Task<HttpResponseMessage> Login(string userId, HttpContextBase context, string name = "", string email = "")
        {
            return await Track(Verbs.LOG_IN, userId, context.UserIpAddress(), name, email, context.Request.UserAgent);
        }

        /// <summary>
        /// Sends failed login metadata for anomaly detection
        /// </summary>
        /// <param name="userId">A unique identifier for a user. ID or email etc</param>
        /// <param name="ip">The users IP address</param>
        /// <param name="name">Optional - The users full name</param>
        /// <param name="email">Optional - The users email address</param>
        /// <param name="userAgent">Optional - The browsers user_agent string</param>
        public async Task<HttpResponseMessage> LoginDenied(string userId, string ip, string name = "", string email = "", string userAgent = "")
        {
            return await Track(Verbs.LOG_IN_DENIED, userId, ip, name, email, userAgent);
        }

        /// <summary>
        /// Sends login metadata for anomaly detection
        /// </summary>
        /// <param name="userId">A unique identifier for a user. ID or email etc</param>
        /// <param name="context">Pass in HttpContext and it will convienently extract IP and UserAgent</param>
        /// <param name="name">Optional - The users full name</param>
        /// <param name="email">Optional - The users email address</param>
        public async Task<HttpResponseMessage> LoginDenied(string userId, HttpContextBase context, string name = "", string email = "")
        {
            return await Track(Verbs.LOG_IN_DENIED, userId, context.UserIpAddress(), name, email, context.Request.UserAgent);
        }

        /// <summary>
        /// Async sends an activity message to ThisData API
        /// </summary>
        /// <param name="verb">The action taken by the user. See docs for supported verbs</param>
        /// <param name="userId">A unique identifier for a user. ID or email etc</param>
        /// <param name="ip">The users IP address</param>
        /// <param name="name">Optional - The users full name</param>
        /// <param name="email">Optional - The users email address</param>
        /// <param name="userAgent">Optional - The browsers user_agent string</param>
        public async Task<HttpResponseMessage> Track(string verb, string userId, string ip, string name = "", string email = "", string userAgent = "")
        {   
            var endpoint = string.Format("{0}events.json?api_key={1}", Defaults.Host, _apiKey);
            var client = new HttpClient();
            
            var activity = new Activity
            {
                Verb = verb,
                User = new UserInfo{
                    Id = userId,
                    Name = name,
                    Email = email
                },
                IP = ip,  
                UserAgent = userAgent
            };
            
            var payload = JsonConvert.SerializeObject(activity);
         
            var result = await client.PostAsync(
                endpoint,
                new StringContent(payload, Encoding.UTF8, "application/json")
            );
            
            return result;             
        }
        

    }
}
