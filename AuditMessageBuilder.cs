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
    public class AuditMessageBuilder
    {
        private static readonly Regex IpAddressRegex = new Regex(@"\A(?:\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b)(:[1-9][0-9]{0,4})?\z", RegexOptions.Compiled);

        public static AuditMessage Build(HttpRequest request, string verb, string userId = "", string name = "", string email = "", string mobile = "")
        {
            AuditMessage message = new AuditMessage();

            message.Verb = verb;

            try
            {
                message.UserAgent = request.UserAgent;
                message.IPAddress = GetIpAddress(request);
                message.User = GetUserDetails(request, name, email, mobile);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get basic request info: {0}", e.Message);
            }

            return message;
        }

        private static UserDetails GetUserDetails(HttpRequest request, string userId = "", string name = "", string email = "", string mobile = "")
        {
            UserDetails user = new UserDetails();

            try
            {
                user.Id = String.IsNullOrEmpty(userId) ? request.LogonUserIdentity.Name : userId;
                user.Name = name;
                user.Email = email;
                user.Mobile = mobile;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get basic user info: {0}", e.Message);
            }

            return user;
        }

        private static string GetIpAddress(HttpRequest request)
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to get IP address: {0}", ex.Message);
            }

            return strIp;
        }

        private static bool IsValidIpAddress(string strIp)
        {
            if (strIp != null)
            {
                return IpAddressRegex.IsMatch(strIp.Trim());
            }
            return false;
        }
    }
}
