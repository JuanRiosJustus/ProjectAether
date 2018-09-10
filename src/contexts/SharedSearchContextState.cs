using BloomFilter;
using IntoTheAether.src.web;
using ProjectAether.src.configs;
using ProjectAether.src.globals;
using ProjectAether.src.internet;
using ProjectAether.src.Utils;
using ProjectAether.src.web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectAether.src.contexts
{
    public class SharedSearchContextState
    {
        private WebPageUrlCollection webpageUrlQueue = null;
        private SharedSearchContextInfo contextInfo = null;
        private ConcurrentDictionary<int, byte> visitedWebpageUrlHashes = null;
        private ConcurrentDictionary<byte, WebHostPolicy> singleShardHostPolicy = null;
        private Filter<string> visitedWebpageUrls = null;

        public SharedSearchContextState(bool usingQueue)
        {
            webpageUrlQueue = new WebPageUrlCollection(usingQueue);
            contextInfo = new SharedSearchContextInfo();
            visitedWebpageUrlHashes = new ConcurrentDictionary<int, byte>();
            visitedWebpageUrls = new Filter<string>(ApplicationConstants.GREATER_STORAGE_LIMIT);
            singleShardHostPolicy = new ConcurrentDictionary<byte, WebHostPolicy>();
            singleShardHostPolicy[0] = new WebHostPolicy();
        }
        
        public SharedSearchContextInfo getContextInfo() { return contextInfo; }
        
        public void tryHandlePotentialTimeout(string webpageUrl)
        {
            WebHostPolicy policy = null;
            if (singleShardHostPolicy.TryGetValue(0, out policy))
            {
                string host = policy.getHost(webpageUrl);
                policy.handle(host);
            }
        }
        /// <summary>
        /// Attempts to add the given host to the global policy
        /// </summary>
        /// <param name="webpagePolicy"></param>
        public void tryMarkForHostTimeout(string webpagePolicy)
        {
            WebHostPolicy policy = null;
            if (singleShardHostPolicy.TryGetValue(0, out policy))
            {
                policy.markHostForTimeout(webpagePolicy);
            }
        }
        /// <summary>
        /// Try to add the to map of websites if not already within
        /// </summary>
        /// <param name="page"></param>
        public void sendToHub(WebPage page)
        {
            if (visitedWebpageUrlHashes.ContainsKey(page.GetHashCode()) == false)
            {
                // add the site to list of sites sent to the hub/sites found
                visitedWebpageUrlHashes[page.GetHashCode()] = 0;
                // enqueue the website to the hub
                Hub.getInstance().add(page);
            }
        }
        /// <summary>
        /// Constructs the output file
        /// </summary>
        /// <param name="configs"></param>
        public void constructOutputFile(ConfiguredSettings configs)
        {
            StringBuilder sb = new StringBuilder(ApplicationConstants.LESSER_DIVIDER + "\n");
            sb.Append(configs.ToString());
            sb.Append(ApplicationConstants.LESSER_DIVIDER + "\n");
            /*foreach (WebPage site in m_webpadeUrls.Values)
            {
                sb.Append(site.ToString() + "\n");
            }*/
            sb.Append(ApplicationConstants.LESSER_DIVIDER + "\n");
            //File.WriteAllText("AetherReport.txt", sb.ToString());
        }


        // keps track of the threads and their usefullness?
        public void setWebpageUrlAsVisited(string url) { visitedWebpageUrls.Add(url); }
        public bool hasWebpageUrlBeenVisited(string url) { return visitedWebpageUrls.Contains(url); }
        
        /// <summary>
        /// Adds links to be traversd so long as they have not been contained
        /// within the underlying set
        /// </summary>
        /// <param name="urls"></param>
        public void tryDepositingUrlsToQueue(Queue<string> urls)
        {
            while(urls.Count > 0)
            {
                string currentUrl = urls.Dequeue();
                if (visitedWebpageUrls.Contains(currentUrl)) { continue; }
                webpageUrlQueue.add(currentUrl);
            }
        }
        public bool isQueueEmpty() { return webpageUrlQueue.isEmpty(); }
        public int sizeOfQueue() { return webpageUrlQueue.size(); }

        /// <summary>
        /// Gets the next link based on the traversal type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string dequeueWebpageUrl(TraversalStyle type, WebCache cache)
        {
            string link = String.Empty;
            if (type == TraversalStyle.DEFAULT_TRAVERSAL_SEARCH)
            {
                webpageUrlQueue.tryRelease(out link);
            }
            else if (type == TraversalStyle.STRICT_RELEVANCE_SEARCH)
            {
                webpageUrlQueue.tryRelease(out link);
            }
            else if (type == TraversalStyle.PRECENTAGE_SHIFT_SEARCH)
            {
                link = precentageShiftDequeue(link);
            }
            else if (type == TraversalStyle.RANDOM_HALF_SEARCH)
            {
                link = randomHalfDequeue(link);
            }
            else if (type == TraversalStyle.NEXT_DIFFERING_SEARCH)
            {
                link = nextDifferingDequeue(cache, link);
            }
            if (link == String.Empty)
            {
                webpageUrlQueue.tryRelease(out link);
            }
            return link;
        }
        /// <summary>
        /// Operation to perform the next differing traversal style
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        private string nextDifferingDequeue(WebCache cache, string link)
        {
            if (cache.getLatestWebsite() == null) { return link; }
            Random random = new Random();
            // swap up until half nodes have been traversed
            int val = random.Next(webpageUrlQueue.size() / 2) + 1;
            Uri last = new Uri(cache.getLatestWebsite().getUrl());
            Uri newest = null;
            bool foundDiffering = false;
            while (val > 0 && webpageUrlQueue.size() > 0 && foundDiffering == false)
            {
                val--;
                // swap to new site
                if (!webpageUrlQueue.tryRelease(out link)) { continue; }
                
                // create uri and check that the current dequeued link is different than current
                if (Uri.TryCreate(link, UriKind.Absolute, out newest))
                {
                    // if the domains/hosts differ, use it
                    string newURL = newest.Authority;
                    string oldURL = last.Authority;
                    if (newURL.Equals(oldURL) == false) { foundDiffering = true; } 
                }
                webpageUrlQueue.add(link);
            }
            return link;
        }
        /// <summary>
        /// Shifts through the queue which may 
        /// potentially enqueue or dequeue the current element
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private string randomHalfDequeue(string link)
        {
            Random random = new Random();
            int val = random.Next(webpageUrlQueue.size() / 2) + 1;
            while (val > 0 && webpageUrlQueue.size() > 0)
            {
                if (webpageUrlQueue.tryRelease(out link))
                {
                    webpageUrlQueue.add(link);
                }
                val--;
            }
            return link;
        }
        /// <summary>
        /// Shifts the queue around by dequeueing and enqueueing
        /// 20% of the elements
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private string precentageShiftDequeue(string link)
        {
            int val = IntegerExtensionMethods.getNthPercentageOf(webpageUrlQueue.size(), 0.2f) + 1;
            while (val > 0 && webpageUrlQueue.size() > 0)
            {
                if (webpageUrlQueue.tryRelease(out link))
                {
                    webpageUrlQueue.add(link);
                }
                val--;
            }
            return link;
        }
    }
}
