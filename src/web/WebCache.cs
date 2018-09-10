using BloomFilter;
using IntoTheAether.src.structures;
using ProjectAether.src.structures;
using ProjectAether.src.Utils;
using ProjectAether.src.web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ProjectAether.src.internet
{
    public class WebCache
    {
        private HashSet<WebPage> setOfSites = new HashSet<WebPage>();
        private Queue<WebPage> queueOfWebpages = new Queue<WebPage>();
        private WebPage latestWebpage = null;
        private Filter<string> visitedWebpageUrls = new Filter<string>(ApplicationConstants.GREATER_STORAGE_LIMIT);
        private Queue<string> webpageUrlsToVisit = new Queue<string>();

        /// <summary>
        /// Adds sites to the cache if not already
        /// visited as per seen in the set.
        /// </summary>
        /// <param name="urls"></param>
        public void tryDepositingUrlsToQueue(Queue<string> urls)
        {
            while (urls.Count > 0)
            {
                string currentURL = urls.Dequeue();
                if (visitedWebpageUrls.Contains(currentURL)) { continue; }
                webpageUrlsToVisit.Enqueue(currentURL);
                visitedWebpageUrls.Add(currentURL);
            }
        }
        /// <summary>
        /// Adds sites to the cache if not already in the cache
        /// i.e. it has not been added already.
        /// </summary>
        /// <param name="url"></param>
        public void tryAddingToQueue(string url)
        {
            if (visitedWebpageUrls.Contains(url)) { return; }
            webpageUrlsToVisit.Enqueue(url);
            visitedWebpageUrls.Add(url);
        }
        /// <summary>
        /// Removes all urls from the cache that need to be visited
        /// </summary>
        /// <returns></returns>
        public Queue<string> releaseUrlsToVisit()
        {
            Queue<string> temp = new Queue<string>();
            while (webpageUrlsToVisit.Count > 0)
            {
                string currentURL = webpageUrlsToVisit.Dequeue();
                temp.Enqueue(currentURL);
            }
            return temp;
        }

        public WebPage getLatestWebsite() { return latestWebpage; }
    }
}
