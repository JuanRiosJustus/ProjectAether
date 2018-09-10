using IntoTheAether.src.structures;
using IntoTheAether.src.web;
using ProjectAether.src.configs;
using ProjectAether.src.internet;
using ProjectAether.src.services;
using ProjectAether.src.structures;
using ProjectAether.src.Utils;
using ProjectAether.src.web;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ProjectAether.src.contexts
{
    public class SearchContext : BaseContext
    {
        private string contextualId = null;
        private int maxPageSearchLimit = 0;
        private bool callable = false;
        private string currentUrl = null;
        private TraversalStyle traversalStyle = null;
        private ConfiguredSettings configuredSettings = null;
        private static SharedSearchContextState sharedSearchContext;

        public SearchContext(ConfiguredSettings configs, ConcurrentQueue<string> mainQueue, ConcurrentQueue<string> secondaryQueue, string id) : base(mainQueue, secondaryQueue)
        {
            contextualId = id;
            configuredSettings = configs;

            bool isUsingQueue = configs.get(ConfiguredConstants.USE_QUEUE_LBL).ToBoolean();
            if (sharedSearchContext == null)
            {
                sharedSearchContext = new SharedSearchContextState(isUsingQueue);
                sharedSearchContext.getContextInfo().addThreadId(contextualId);
            }
            else
            {
                sharedSearchContext.getContextInfo().addThreadId(contextualId);
            }
            maxPageSearchLimit = StringExtensionMethods.ToInteger(configs.get(ConfiguredConstants.SEARCH_LIMIT_LBL));
            traversalStyle = TraversalStyle.getTraversalType(configs.get(ConfiguredConstants.TRAVERSAL_STYLE_LBL));
            currentUrl = StringExtensionMethods.URLifyParseAddition(configs.get(ConfiguredConstants.ENTRY_POINT_LBL), configs.get(ConfiguredConstants.SEARCH_PHRASE_LBL));
        }

        /// <summary>
        /// Performs the base operations of a crawler
        /// </summary>
        public void search()
        {
            mainDisplayQueue.Enqueue(" ");
            mainDisplayQueue.Enqueue(contextualId + " started exploring.");
            callable = true;
            sharedSearchContext.getContextInfo().incrementThreadCounter();
            explore();
            sharedSearchContext.getContextInfo().decrementThreadCounter();
            callable = false;
            mainDisplayQueue.Enqueue(" ");
            mainDisplayQueue.Enqueue(contextualId + " returned.");
        }

        /// <summary>
        /// Searches the given url once for information.
        /// </summary
        /// <param name="url"></param>
        /// <returns></returns>
        private void explore()
        {
            WebUtils utils = new WebUtils();
            WebCache cache = new WebCache();
            WebCrawler crawler = new WebCrawler();
            WebHostPolicy policy = new WebHostPolicy();
            WebProcessor processor = new WebProcessor(configuredSettings);
            ThreadSleeper sleeper = new ThreadSleeper(5000);
            // init the queue if not already created, 
            if (sizeOfQueue() < 1) { initQueue(cache, currentUrl); }

            // traverse as long as the visited urls is less than the limit, is callable, and URL collection is not empty
            while (amountOfWebpageUrlsTraversed() < maxPageSearchLimit && callable && !isQueueEmpty())
            {
                string currentWebpageUrl = dequeueWebpageUrl(traversalStyle, cache);
                
                // ensure the url is valid and has not been visited already
                if (!utils.isValidWebpageURL(currentWebpageUrl) || hasWebpageUrlBeenVisied(currentWebpageUrl)) { continue; }
                
                // try to timeout checking shared state and current thread
                handlePotentialTimeout(policy, utils, currentWebpageUrl);

                // if the crawl returns false, then it is an unsupported url
                if (!crawler.tryCrawl(currentWebpageUrl)) { continue; }

                setWebpageUrlAsVisited(currentWebpageUrl);

                // Retrieve all the texts found by the crawler
                Queue<string> texts = crawler.releaseTexts();
                Queue<string> webpageUrls = crawler.releaseWebpages();
                Queue<string> imageUrls = crawler.releaseImages();
                string currentWebpageHost = crawler.releaseHost();

                // filters the texts potentially and handles the links/images/etc
                WebPage page = processor.constructWebsite(texts, webpageUrls, imageUrls, currentWebpageUrl, currentWebpageHost);
                processor.tryBasicFilter(texts);

                // handles the cache to context communication for the newly discovered site URLS
                addWebpageUrlsToQueue(cache, page, webpageUrls, imageUrls);
                // enqueue the website to the hub
                sendToHub(page);

                // Update the state object
                sharedSearchContext.getContextInfo().addToThreadScore(contextualId, page.getSearchPhraseCount());
                sharedSearchContext.getContextInfo().incrementUrlsTraversed();

                // construct the display for the end user
                mainDisplayQueue.Enqueue(utils.createPrimaryDisplayView(page, contextualId));

                // consturct the secondary display for the end user
                secondaryDisplayQueue.Enqueue(utils.createSecondaryDisplayView(sharedSearchContext));
                
                // try to set webpage for timeout on all threads
                addOrUpdatePolicy(policy, currentWebpageHost);
                sleeper.trySleeping();
            }
            secondaryDisplayQueue.Enqueue(utils.createSecondaryDisplayView(sharedSearchContext));
        }
        /// <summary>
        /// handles the processing based on the traversal method chosen
        /// and sends it to the contexts state
        /// </summary>
        /// <param name="states"></param>
        /// <param name="cache"></param>
        /// <param name="page"></param>
        /// <param name="links"></param>
        private void addWebpageUrlsToQueue(WebCache cache, WebPage page, Queue<string> webpageUrls, Queue<string> imageUrls)
        {
            bool isAddable = false;
            if (traversalStyle == TraversalStyle.STRICT_RELEVANCE_SEARCH)
            {
                if (page.getSearchPhraseCount() > 0 || page.getSearchTokensCount() > 0) { isAddable = true; }
            }
            else { isAddable = true; }

            if (isAddable)
            {
                // check that the urls have not been seen
                cache.tryDepositingUrlsToQueue(webpageUrls);
                // enqueue the non seen urls
                tryDepositingUrlsToQueue(cache.releaseUrlsToVisit());
                // send images to ui
                tryEnqueueImagesToUI(imageUrls);
            }
        }

        private void handlePotentialTimeout(WebHostPolicy policy, WebUtils utils, string webpageUrl)
        {
            string host = policy.getHost(webpageUrl);
            if (host != null)
            {
                sharedSearchContext.getContextInfo().incrementThreadSleepCounter();
                secondaryDisplayQueue.Enqueue(utils.createSecondaryDisplayView(sharedSearchContext));
                policy.handle(host);
                sharedSearchContext.tryMarkForHostTimeout(webpageUrl);
                sharedSearchContext.getContextInfo().decrementThreadSleepCounter();
                secondaryDisplayQueue.Enqueue(utils.createSecondaryDisplayView(sharedSearchContext));
            }
        }

        private int sizeOfQueue() { return sharedSearchContext.sizeOfQueue(); }
        private bool isQueueEmpty() { return sharedSearchContext.isQueueEmpty(); }
        private string dequeueWebpageUrl(TraversalStyle style, WebCache cache) { return sharedSearchContext.dequeueWebpageUrl(style, cache); }
        private bool hasWebpageUrlBeenVisied(string url) { return sharedSearchContext.hasWebpageUrlBeenVisited(url); }
        private void initQueue(WebCache cache, string url) { cache.tryAddingToQueue(url); sharedSearchContext.tryDepositingUrlsToQueue(cache.releaseUrlsToVisit()); }
        private void setWebpageUrlAsVisited(string url) { sharedSearchContext.setWebpageUrlAsVisited(url); }
        private void tryEnqueueImagesToUI(Queue<string> picUrls) { sharedSearchContext.getContextInfo().tryEnqueueImageUrl(picUrls); }
        private int amountOfWebpageUrlsTraversed() { return sharedSearchContext.getContextInfo().amountOfWebpageUrlsVisited(); }
        private void sendToHub(WebPage page) { sharedSearchContext.sendToHub(page); }
        private void tryDepositingUrlsToQueue(Queue<string> pages) { sharedSearchContext.tryDepositingUrlsToQueue(pages); }
        private void addOrUpdatePolicy(WebHostPolicy policy, string webpageHost)
        {
            policy.markHostForTimeout(webpageHost);
            sharedSearchContext.tryMarkForHostTimeout(webpageHost);
        }

        // These are used to stop the thread / check its state
        public bool isExploring() { return callable; }
        public void callback() { callable = false; }
        public double getProgressToSearchLimit() { return (double)sharedSearchContext.getContextInfo().amountOfWebpageUrlsVisited() / maxPageSearchLimit; }
        public bool tryRetrievingImageUrl(out string url)  { return sharedSearchContext.getContextInfo().tryRetrievingImageUrl(out url); }
        public void tryDestroyingSharedState()
        {
            if (sharedSearchContext.getContextInfo().amountOfThreads() == 0)
            {
                sharedSearchContext = null;
            }
        }
    }
}
