using BloomFilter;
using System;
using System.Collections.Generic;
using ProjectAether.src.Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.web
{
    public class WebSite
    {
        private string url;
        private int score;
        private int cost;
        private List<WebPage> m_pages = new List<WebPage>();

        public WebSite(string siteMainUrl) { url = siteMainUrl; }

        /// <summary>
        /// Adds the given webpage to the site if contained
        /// </summary>
        /// <param name="page"></param>
        public void add(WebPage page) { if (m_pages.Contains(page) == false) { m_pages.Add(page); } }
        /// <summary>
        /// Returns the webpage at the given index
        /// </summary>
        /// <param name="index"></param>
        public WebPage get(int index) { return m_pages[index]; } 
        /// <summary>
        /// Returns the amount of webpages contained within the site
        /// </summary>
        /// <returns></returns>
        public int size() { return m_pages.Count; }

        public override int GetHashCode() { return url.GetSpecialHashCode(); }
        public override bool Equals(object obj)
        {
            WebSite site = obj as WebSite;
            if (site == null)
            {
                return false;
            }
            else
            {
                return site.ToString().GetSpecialHashCode() == url.GetSpecialHashCode();
            }
        }
        public override string ToString() { return url; }
    }
}
