using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.Utils
{
    public static class WebExtensionMethods
    {
        /// <summary>
        /// Returns true if the given url can be pinged
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool isWebsiteAvailable(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.AllowAutoRedirect = false;
            request.Method = "HEAD";
            try
            {
                request.GetResponse();
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
