using ProjectAether.src.web;
using System.Collections.Concurrent;

namespace ProjectAether.src.structures
{
    public class WebPagePool
    {
        private static WebPagePool instance = null;
        private WebPagePool() { }
        public static WebPagePool getInstance()
        {
            if (instance == null)
            {
                instance = new WebPagePool();
            }
            return instance;
        }

        private ConcurrentDictionary<WebPage, byte> pool = new ConcurrentDictionary<WebPage, byte>();
        /// <summary>
        /// Adds the given page to the pool of pages if not contained already
        /// </summary>
        /// <param name="page"></param>
        public void add(WebPage page)
        {
            if (!pool.ContainsKey(page))
            {
                pool[page] = 0;
            }
        }
        /// <summary>
        /// Returns the pool as an array instance
        /// </summary>
        /// <returns></returns>
        public WebPage[] toArray()
        {
            WebPage[] ret = new WebPage[pool.Count];
            int ind = 0;
            foreach (WebPage page in pool.Keys)
            {
                ret[ind] = page;
                ind++;
            }
            return ret;
        }

        /// <summary>
        /// Returns the size of the pool
        /// </summary>
        /// <returns></returns>
        public int size() { return pool.Count; }
        
    }
}
