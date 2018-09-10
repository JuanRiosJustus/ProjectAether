using IntoTheAether.src.structures;
using ProjectAether.src.structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntoTheAether.src.web
{
    public class WebHostPolicy
    {
        private static readonly int CACHE_SIZE = 15;
        private static readonly int MIN_THREAD_SLEEP = 20000;
        private static readonly int MAX_THREAD_SLEEP = 50000;

        private LRUCache<string, ThreadSleeper> webpageHosts = new LRUCache<string, ThreadSleeper>(CACHE_SIZE);

        /// <summary>
        /// Set the appropriate timeout per website
        /// returns the host that was evicted from timeout
        /// </summary>
        /// <param name="hostname"></param>
        public void markHostForTimeout(string hostname)
        {
            if (webpageHosts.get(hostname) == null)
            {
                webpageHosts.put(hostname, new ThreadSleeper(MIN_THREAD_SLEEP));
            }
            else
            {
                ThreadSleeper sleeper = webpageHosts.get(hostname);
                if (sleeper.getTimeoutLimit() < MAX_THREAD_SLEEP) { sleeper.addTenSeconds(); }
                sleeper.restart();
            }
        }

        /// <summary>
        /// Check that the cache has seen host of a given url
        /// if so, sleep until the host's timeout is finished
        /// (PREVENTS EXCESSIVE CONSECUTIVE PINGS)
        /// </summary>
        /// <param name="url"></param>
        public void tryHandling(string url)
        {
            string[] keys = webpageHosts.getKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                // check if url is a key, if it is, sleep for the remaining time left
                if (url.Contains(keys[i]) == false) { continue; }
                webpageHosts.get(keys[i]).trySleeping();
                return;
            }
        }

        /// <summary>
        /// Check that the cache has seen host of a given url
        /// if so, sleep until the host's timeout is finished
        /// (PREVENTS EXCESSIVE CONSECUTIVE PINGS)
        /// </summary>
        /// <param name="url"></param>
        public void handle(string hostKey) { webpageHosts.get(hostKey).trySleeping(); }

        /// <summary>
        /// Checks the url if its host is contained within the 
        /// If not found, returns null, else returns the key associated with the url A.K.A. the host of the url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string getHost(string url)
        {
            string[] arr = webpageHosts.getKeys();
            for (int i = 0; i < arr.Length; i++)
            {
                if (url.Contains(arr[i]))
                {
                    return arr[i];
                }
            }
            return null;
        }
        
        public string timeouts()
        {
            StringBuilder sb = new StringBuilder();
            string[] keys = webpageHosts.getKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                sb.Append(keys[i] + "\n");
            }
            return sb.ToString();
        }
    }
}
