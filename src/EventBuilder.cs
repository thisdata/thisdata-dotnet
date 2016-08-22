using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using ThisData.Models;

namespace ThisData
{
    public class EventBuilder
    {
        private static readonly Regex IpAddressRegex = new Regex(@"\A(?:\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b)(:[1-9][0-9]{0,4})?\z", RegexOptions.Compiled);

        /// <summary>
        /// Formats a message to send to the ThisData API
        /// </summary>
        /// <param name="request">HttpRequest object for the incoming request</param>
        /// <param name="verb">The action taken by the user. eg. log-in</param>
        /// <param name="userId">A unique identifier for the user. If omitted LogonUserIdentity.Name will be used</param>
        /// <param name="name">The full name of the user</param>
        /// <param name="email">The users email address for sending notifications</param>
        /// <param name="mobile">The users mobile phone number for sending SMS notifications</param>
        /// <param name="source">Used to indicate the source of the event and override company or app name in audit log and notifications</param>
        /// <param name="logoUrl">Used to override logo used in email notifications</param>
        public static Event Build(HttpRequest request, string verb, string userId = "", string name = "", string email = "", string mobile = "", 
            string source = "", string logoUrl = "", string sessionId = "", bool cookieExpected = false)
        {
            Event message = new Event();

            message.Verb = verb;
            message.Session.Id = sessionId;
            message.Session.CookieExpected = cookieExpected;
            message.User.Id = String.IsNullOrEmpty(userId) ? "anonymous" : userId;
            message.User.Name = name;
            message.User.Email = email;
            message.User.Mobile = mobile;

            try
            {
                message.UserAgent = request.UserAgent;
                message.IPAddress = GetIpAddress(request);
                message.Session.CookieId = GetCookieId(request);

                // Source options are only supported if source name is provided
                if (!String.IsNullOrEmpty(source))
                {
                    message.Source = new SourceOptions()
                    {
                        Name = source,
                        LogoUrl = logoUrl
                    };
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get basic request info: {0}", e.Message);
            }

            return message;
        }

        public static string GetCookieId(HttpRequest request)
        {
            string cookieId = null;

            try
            {
                var cookie = request.Cookies[Defaults.JavascriptCookieName];
                if (cookie != null)
                {
                    cookieId = cookie.Value;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get cookie: {0}", e.Message);
            }

            return cookieId;
        }

        public static string GetIpAddress(HttpRequest request)
        {
            string strIp = null;

            try
            {
                strIp = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (strIp != null && strIp.Trim().Length > 0)
                {
                    string[] addresses = strIp.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (addresses.Length > 0)
                    {
                        // first one = client IP per http://en.wikipedia.org/wiki/X-Forwarded-For
                        strIp = addresses[0];
                    }
                }

                if (!IsValidIpAddress(strIp))
                {
                    strIp = string.Empty;
                }

                // if that's empty, get their ip via server vars
                if (strIp == null || strIp.Trim().Length == 0)
                {
                    strIp = request.ServerVariables["REMOTE_ADDR"];
                }

                if (!IsValidIpAddress(strIp))
                {
                    strIp = string.Empty;
                }

                // if that's still empty, get their ip via .net's built-in method
                if (strIp == null || strIp.Trim().Length == 0)
                {
                    strIp = request.UserHostAddress;
                }

                // make sure we strip off port number if exists
                if (!String.IsNullOrEmpty(strIp))
                {
                    string[] ipParts = strIp.Split(':');
                    if (ipParts.Length > 1)
                    {
                        strIp = ipParts[0];
                    }
                }

                // address is mandatory so if we have failed to get an address
                // we can assume its probably local testing
                if (String.IsNullOrEmpty(strIp))
                {
                    strIp = "127.0.0.1";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get IP address: {0}", ex.Message);
            }

            return strIp;
        }

        public static bool IsValidIpAddress(string strIp)
        {
            if (strIp != null)
            {
                return IpAddressRegex.IsMatch(strIp.Trim());
            }
            return false;
        }
    }
}
