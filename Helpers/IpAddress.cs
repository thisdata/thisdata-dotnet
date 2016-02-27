using System.Web;

namespace ThisData.Helpers
{
    public static class IpAddressHelpers
    {
        /// <summary>
        /// The best available IP address for a remote user
        /// </summary>
        /// <param name="context"></param>
        /// <returns>IP address as a string</returns>
        public static string UserIpAddress(this HttpContextBase context)
        {
            try
            {
                string ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                // HTTP_X_FORWARDED_FOR can return multiple addresses. If this happens take the last one.
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.Contains(","))
                    {
                        string[] ips = ip.Split(',');
                        ip = ips[ips.Length - 1];
                    }
                }

                // If IP still empty then go for the HTTP_X_CLUSTER_CLIENT_IP
                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
                }

                // If IP still empty then go for the REMOTE_ADDR
                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Request.ServerVariables["REMOTE_ADDR"];
                }
                return ip;
            }
            catch
            {
                return "";
            }
        }
    }
}
