using ProjectAether;
using ProjectAether.src.structures;
using ProjectAether.src.Utils;
using ProjectAether.src.web;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IntoTheAether.src.core
{
    public class HubAnalyzer
    {
        private Graph<string> sitegraph = null;
        private StringBuilder builder = new StringBuilder();
        private int maxSpacing = 0;

        public HubAnalyzer(WebPage[] pages)
        {
            if (pages == null || pages.Length < 1)
            {
                builder.Append("Nothing to analyze" + "\r\n");
            }
            else
            {
                sitegraph = constructWebsiteGraph(pages);
                builder.Append(ApplicationConstants.GREATER_DIVIDER + "\r\n");
                builder.Append(constructWebsiteGraphVisual(sitegraph));
                builder.Append(ApplicationConstants.GREATER_DIVIDER + "\r\n");
                builder.Append(basicAnalysis(pages));
                builder.Append(ApplicationConstants.GREATER_DIVIDER + "\r\n");
                StreamWriter sw = new StreamWriter("AetherReport.txt", false);
                sw.WriteLine(builder.ToString());
                sw.Flush();
                sw.Close();
            }
        }
        // Ensure to find the max length of a given number
        private void ensureEnoughSpacing(int num)
        {
            num = num.charactersWithin();
            if (num > maxSpacing) { maxSpacing = num + 1; }
        }
        // ensure enough spaces for printing
        private string addEnoughSpaces(int num)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(num);
            while (sb.Length < maxSpacing)
            {
                sb.Insert(0, ' ');
            }
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        private string constructWebsiteGraphVisual(Graph<string> graph)
        {
            StringBuilder sb = new StringBuilder();
            int[,] g = graph.toMultiDimensionalArray();
            // get largest digit for printing reasons
            for (int i  = 0; i < g.GetLength(0); i++)
            {
                for (int j = 0; j < g.GetLength(1); j++)
                {
                    ensureEnoughSpacing(g[i, j]);
                }
            }
            int n = 0;
            for (int i = 0; i < graph.size(); i++)
            {
                for (int j = 0; j < graph.size(); j++)
                {
                    sb.Append("[" + addEnoughSpaces(g[i, j]) + "]");
                }
                if (i > 0) { sb.Append(" ( " + n + " == " + graph.get(n) + " )"); n++; }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        /// <summary>
        /// Creates a basic output for the given pages
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        private string basicAnalysis(WebPage[] pages)
        {
            int mostLinks = 0;
            int mostSearchPhraseMatches = 0;
            int mostSearchTokenMatches = 0;
            int mostImages = 0;
            List<WebPage> sortedPages = new List<WebPage>();

            for (int i = 0; i < pages.Length; i++)
            {
                WebPage current = pages[i];
                sortedPages.Add(current);
                // check for the page with the most links
                if (current.getInboundUrls().Length > pages[mostLinks].getInboundUrls().Length) { mostLinks = i; }
                // check for the most search phases found
                if (current.getSearchPhraseCount() > pages[mostSearchPhraseMatches].getSearchPhraseCount()) { mostSearchPhraseMatches = i; }
                // check with has most search term matches
                if (current.getSearchTokensCount() > pages[mostSearchTokenMatches].getSearchTokensCount()) { mostSearchTokenMatches = i; }
                // check for most images
                if (current.getImageCount() > pages[mostImages].getImageCount()) { mostImages = i; }
            }
            sortedPages.Sort();

            StringBuilder sb = new StringBuilder();
            sb.Append("Most relevant webpage is " + pages[mostSearchPhraseMatches].getUrl() + "\r\n");
            sb.Append(ApplicationConstants.LESSER_DIVIDER + "\r\n");
            sb.Append("Webpage with most links found at  " + pages[mostLinks].getUrl() + "\r\n");
            sb.Append(ApplicationConstants.LESSER_DIVIDER + "\r\n");
            sb.Append("Webpage with most images is " + pages[mostImages].getUrl() + "\r\n");
            return sb.ToString();
        }
        /// <summary>
        /// Constructs a connection graph of 
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        private Graph<string> constructWebsiteGraph(WebPage[] pages)
        {
            Dictionary<int, WebPage> hashmap = new Dictionary<int, WebPage>();
            Graph<string> graph = new Graph<string>();
            HashSet<string> set = new HashSet<string>();
            // add all hosts to graph once, assign special hash with its page to dictionary
            for (int i = 0; i < pages.Length; i++)
            {
                hashmap.Add(pages[i].getUrl().GetSpecialHashCode(), pages[i]);
                if (set.Contains(pages[i].getHost())) { continue; }
                graph.add(pages[i].getHost());
                set.Add(pages[i].getHost());
            }

            // Check each pages connection, assign graph weights respectively
            for (int i = 0; i < pages.Length; i++)
            {
                WebPage from = pages[i];
                int[] siteHashes = from.getInboundUrls();
                for (int j = 0; j < siteHashes.Length; j++)
                {
                    int hashCode = siteHashes[j];
                    if (!hashmap.ContainsKey(hashCode)) { continue; }
                    WebPage to = hashmap[hashCode];
                    if (from.getHost().GetSpecialHashCode() == to.getHost().GetSpecialHashCode()) { continue; }
                    int weight = to.getInboundUrls().Length + graph.weightBetween(from.getHost(), to.getHost());
                    // set the weight as the amount of links found at the endpoint node
                    graph.connect(from.getHost(), hashmap[hashCode].getHost(), (weight < 1 ? 1 : weight));
                }
            }
            return graph;
        }

        /// <summary>
        /// Returns the result of the analyzers analyzation of the given webpages
        /// </summary>
        /// <returns></returns>
        public string analyzation()
        {
            string str = builder.ToString();
            builder.Clear();
            return str;
        }
    }
}
