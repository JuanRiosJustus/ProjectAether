using BloomFilter;
using System.Text;
using ProjectAether.src.Utils;
using System;

namespace ProjectAether.src.web
{
    public class WebPage : IComparable
    {
        // the name of the url
        private string url;
        // the host of the webpage
        private string host;
        // amount of characters found
        private int characters;
        // amount of lines found
        private int lines;
        // the amount of exacth matches to the search phrase
        private int searchPhrases;
        // the amount of times the words in the phrase appear
        private int searchTokens;
        // the amount of images contained here
        private int images;
        // the amount of tokens delimited by whitespace, not counting
        private int words;
        // sample text of the search phrase if found
        private string sampleText;
        // set of inbound links
        private int[] inboundUrls;
        
        public string getUrl() { return url; }
        public void setUrl(string link) { if (url == null) { url = link; } }

        public int getCharacterCount() { return characters; }
        public void setCharacterCount(int count) { if (characters == 0) { characters = count; } }

        public int getLineCount() { return lines; }
        public void setLineCount(int count) { if (lines == 0) { lines = count; } }

        public int getSearchPhraseCount() { return searchPhrases; }
        public void setSearchPhraseCount(int count) { if (searchPhrases == 0) { searchPhrases = count; } }

        public int getSearchTokensCount() { return searchTokens; }
        public void setSearchTokensCount(int count) { if (searchTokens == 0) { searchTokens = count; } }

        public int getLinkCount() { return inboundUrls.Length; }

        public int getImageCount() { return images; }
        public void setImageCount(int count) { if (images == 0) { images = count; }  }

        public string getSampleText() { return sampleText; }
        public void setSampleText(string sample) { if (sampleText == null) { sampleText = sample; } }

        public int getWordCount() { return words; }
        public void setWordCount(int amt) { if (words == 0) { words = amt; } }

        public string getHost() { return host; }
        public void setHost(string pageHost) { if (host == null) { host = pageHost; } }

        public void setInboundUrls(int[] urls) { if (inboundUrls == null) { inboundUrls = urls; } }
        public int[] getInboundUrls() { return inboundUrls; } 
        public bool isInboundUrl(string str)
        {
            int hash = str.GetSpecialHashCode();
            for (int i = 0; i < inboundUrls.Length; i++)
            {
                if (inboundUrls[i] == hash) { return true; }
            }
            return false;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            WebPage page = obj as WebPage;
            if (page == null)
            {
                return false;
            }
            else
            {
                return page.GetHashCode() == GetHashCode();
            }
        }

        public int score()
        {
            int val = searchTokens * searchPhrases;
            if (val < 1) { return -1; }
            val += (val > 0 && inboundUrls.Length > 100 ? -inboundUrls.Length : inboundUrls.Length);
            val += (images > 20 ? -images : images);
            return val;
        }

        public override int GetHashCode() { return  url.GetSpecialHashCode(); }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Url[" + url + "] ");
            sb.Append("Host[" + host + "] ");
            sb.Append("Sample[" + sampleText + "] ");
            sb.Append("Inbounds[");
            for (int i = 0; i < inboundUrls.Length; i++)
            {
                sb.Append(" " + inboundUrls[i] + " ");
            }
            sb.Append("]");
            return sb.ToString();
        }

        public string representation()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Webpage( ");
            sb.Append("Url( \"" + url + "\" ) ");
            sb.Append("Sample( \"" + sampleText + "\" ) ");
            sb.Append("Matches( \"" + searchPhrases + "/" + searchTokens + "\" )");
            sb.Append(" )");
            return sb.ToString();
        }

        public int CompareTo(object obj)
        {
            WebPage pageToCompare = obj as WebPage;
            if (pageToCompare == null)
            {
                return 1;
            }
            else if (pageToCompare.getSearchPhraseCount() < searchPhrases)
            {
                return 1;
            }
            else if (pageToCompare.getSearchPhraseCount() < searchPhrases)
            {
                return -1;
            }
            else
            {
                if (pageToCompare.getSearchTokensCount() < searchTokens)
                {
                    return 1;
                }
                else if (pageToCompare.getSearchTokensCount() > searchTokens)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
