using ProjectAether.src.contexts;
using ProjectAether.src.globals;
using ProjectAether.src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.web
{
    public class WebUtils
    {
        private static readonly int MAX_SHOWABLE_CHARS = 35;

        /// <summary>
        /// Detects if this videos
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public bool isValidWebpageURL(string url)
        {
            if (String.IsNullOrWhiteSpace(url)) { return false; }
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute) && !isCorrectScheme(url)) { return false; }
            if (!isPingable(url)) { return false; }
            return true;
        }
        /// <summary>
        /// Determines if the url has the correct scheme
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool isCorrectScheme(string url)
        {
            Uri result = null;
            bool isCorrect = Uri.TryCreate(url, UriKind.Absolute, out result);
            return isCorrect && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
        /// <summary>
        /// Returns true if the given url can be pinged
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool isPingable(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri.AbsoluteUri);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creats display output for the primary vew
        /// </summary>
        /// <param name="page"></param>
        /// <param name="threadId"></param>
        /// <returns></returns>
        public string createPrimaryDisplayView(WebPage page, string threadId)
        {
            StringBuilder sb = new StringBuilder(threadId);
            sb.Append(" located " + constructMockpage(page).representation());
            return sb.ToString();
        }

        /// <summary>
        /// Constructs a new instance of a website given the url, occurrences and sample
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sample"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public WebPage constructMockpage(WebPage page)
        {
            WebPage mockpage = new WebPage();
            mockpage.setUrl(page.getUrl().Truncate( false, MAX_SHOWABLE_CHARS));
            mockpage.setSampleText(page.getSampleText().Truncate(true, MAX_SHOWABLE_CHARS));
            mockpage.setSearchPhraseCount(page.getSearchPhraseCount());
            mockpage.setSearchTokensCount(page.getSearchTokensCount());
            mockpage.setInboundUrls(page.getInboundUrls());
            mockpage.setHost(page.getHost());
            mockpage.setImageCount(page.getImageCount());
            mockpage.setWordCount(page.getWordCount());
            return mockpage;
        }
        /// <summary>
        /// Constructs a new instance of a website given the url, occurrences and sample
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sample"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public WebPage constructWebpageOutput(WebPage page)
        {
            WebPage mockpage = new WebPage();
            mockpage.setUrl(page.getUrl());
            mockpage.setSampleText(page.getSampleText().Truncate(true, MAX_SHOWABLE_CHARS));
            mockpage.setInboundUrls(page.getInboundUrls());
            mockpage.setHost(page.getHost());
            return mockpage;
        }
        public string createSecondaryDisplayView(SharedSearchContextState searchState)
        {
            return "Elapsed time: " + searchState.getContextInfo().getElapsedTimeInSeconds() + "\n" +
                "Sites visited: " + Hub.getInstance().size() + "\n" +
                      "Sites queued: " + searchState.sizeOfQueue() + "\n" +
                   "Threads: " + searchState.getContextInfo().amountOfThreads() + "\n" +
                   "Threads sleeping: " + searchState.getContextInfo().getThreadSleepCounter() + "\n" +
                   "Images Found: " + searchState.getContextInfo().getImagesFound() + "\n" +
                            searchState.getContextInfo().viewOfThreadMap();
        }
    }
}
