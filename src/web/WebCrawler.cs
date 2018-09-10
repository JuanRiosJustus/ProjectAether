using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace ProjectAether.src.internet
{
    public class WebCrawler
    {
        private HtmlWeb client = new HtmlWeb();
        private Queue<string> texts = null;
        private Queue<string> links = null;
        private Queue<string> images = null;
        private string host = null;

        private static readonly string HTTP = "http";
        private static readonly string HREF = "href";
        private static readonly string SRC = "src";

        public WebCrawler() { client.UserAgent = ApplicationConstants.APPLICATION_USER_AGENT; }

        /// <summary>
        /// Fetches the information from the 
        /// </summary>
        /// <param name="destination"></param>
        public bool tryCrawl(string url)
        {
            try
            {
                releaseAll();
                Uri uri = new Uri(url);
                HtmlDocument doc = client.Load(url);
                texts = getTexts(doc.DocumentNode);
                links = getLinks(doc.DocumentNode, uri.Host);
                images = getImages(doc.DocumentNode);
                host = uri.Host;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves all the texts from the given root.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private Queue<string> getTexts(HtmlNode root)
        {
            Queue<string> texts = new Queue<string>();
            foreach (HtmlNode line in root.DescendantsAndSelf())
            {
                if (line.HasChildNodes == false) { continue; } 
                if (String.IsNullOrWhiteSpace(line.InnerText)) { continue; }
                texts.Enqueue(line.InnerText.Trim().ToLower());
            }
            return texts;
        }
        /// <summary>
        /// Retrieves all the links found within the document
        /// </summary>
        /// <param name="root"></param>
        private Queue<string> getLinks(HtmlNode root, string host)
        {
            HtmlNodeCollection collection = root.SelectNodes("//a[@href]");
            if (collection == null) { return new Queue<string>(); }
            Queue<string> links = new Queue<string>();
            foreach (HtmlNode link in collection)
            {
                string url = link.GetAttributeValue(HREF, String.Empty);
                // link cant be empty, white space or null
                if (String.IsNullOrWhiteSpace(url)) { continue; }
                if (url.StartsWith(HTTP))
                {
                    links.Enqueue(url);
                }
                else
                {
                    links.Enqueue(host + url);
                }
            }
            return links;
        }
        /// <summary>
        /// retrieves all the media related links from the given
        /// queue and constructs a new queue representing all the 
        /// media related to static pictures -png, jpg, gif, bmp
        /// </summary>
        /// <param name="links"></param>
        /// <returns></returns>
        private Queue<string> getImages(HtmlNode root)
        {
            HtmlNodeCollection collection = root.SelectNodes("//img");
            if (collection == null) { return new Queue<string>(); }
            Queue<string> media = new Queue<string>();
            foreach(HtmlNode image in collection)
            {
                string img = image.GetAttributeValue(SRC, String.Empty);
                if (String.IsNullOrWhiteSpace(img)) { continue; }
                if (img.StartsWith(HTTP) == false) { continue; }
                media.Enqueue(img);
            }
            return media;
        }

        public Queue<string> releaseTexts()
        {
            Queue<string> temp = texts;
            texts = null;
            return temp;
        }

        public Queue<string> releaseWebpages()
        {
            Queue<string> temp = links;
            links = null;
            return temp;
        }

        public Queue<string> releaseImages()
        {
            Queue<string> temp = images;
            images = null;
            return temp;
        }

        public string releaseHost()
        {
            string temp = host;
            host = null;
            return temp;
        }

        public void releaseAll() { texts = null; links = null; images = null; host = null; }
    }
}
