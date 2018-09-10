using BloomFilter;
using ProjectAether.src.configs;
using ProjectAether.src.contexts;
using ProjectAether.src.Utils;
using ProjectAether.src.web;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ProjectAether.src.services
{
    public class WebProcessor
    {
        private bool usingFilter = true;
        private string searchPhrase = null;
        private List<string> searchTokens = null;
        private static readonly int MIN_AMOUNT_CHARS = 3;
        private static readonly int MAX_AMOUNT_CHARS = 200;

        public WebProcessor(ConfiguredSettings settings)
        {
            searchPhrase = settings.get(ConfiguredConstants.SEARCH_PHRASE_LBL).ToLower();
            usingFilter = StringExtensionMethods.ToBoolean(settings.get(ConfiguredConstants.USE_FILTER_LBL));
            string[] terms = searchPhrase.Split(' ');
            searchTokens = new List<string>();
            for (int i = 0; i < terms.Length; i++)
            {
                searchTokens.Add(terms[i].ToLower());
            }
            searchTokens.Add(searchPhrase);
        }
        
        /// <summary>
        /// Filters the given list and returns the list, filtered
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public void tryBasicFilter(Queue<string> texts)
        {
            if (!usingFilter) { return; }
            Filter<string> set = new Filter<string>(ApplicationConstants.GREATER_STORAGE_LIMIT);
            int count = texts.Count;
            while (count > 0)
            {
                string str = texts.Dequeue();
                count--;
                if (set.Contains(str)) { continue; }
                if (failsFilter(str)) { continue; }
                texts.Enqueue(str);
                set.Add(str);
            }
        }
        /// <summary>
        /// Basic filtering test to check if a given string will fail
        /// </summary>
        /// <param name="set"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool failsFilter(string str)
        {
            if (str.Length < MIN_AMOUNT_CHARS) { return true; }
            if (str.Length > MAX_AMOUNT_CHARS) { return true; }
            if (str.StartsOrEndsSpecially()) { return true; }
            if (str.HasMoreRedundantCharactersThan(MIN_AMOUNT_CHARS)) { return true; }
            // the string passes the basic filter
            return false;
        }
        /// <summary>
        /// Removes duplicate elements from the queue
        /// </summary>
        /// <param name="queue"></param>
        private void filterDuplicates(Queue<string> queue)
        {
            Filter<string> filter = new Filter<string>(ApplicationConstants.GREATER_STORAGE_LIMIT);
            for (int i = 0; i < queue.Count; i++)
            {
                string str = queue.Dequeue();
                if (!filter.Contains(str))
                {
                    queue.Enqueue(str);
                    filter.Add(str);
                }
            }
        }
        /// <summary>
        /// Retrieves a string displaying the amount of characters, lines, and search term occurances in a string
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public WebPage constructWebsite(Queue<string> texts, Queue<string> links, Queue<string> images, string url, string host)
        {
            filterDuplicates(texts);
            filterDuplicates(links);
            filterDuplicates(images);
            int chars = 0;
            int phraseMatches = 0;
            int words = 0;
            int tokenMatches = 0;
            string sample = String.Empty;
            for (int j = 0; j < texts.Count; j++)
            {
                string line = texts.Dequeue();
                chars += line.Length;
                words += line.getChunks();
                // Use the current line and fine the phraseMatchs and term matches
                for (int i = 0; i < searchTokens.Count; i++)
                {
                    // check when it is the search phrase
                    if (line.Contains(searchTokens[i]) && searchTokens[i].Equals(searchPhrase))
                    {
                        phraseMatches += line.Occurrences(searchTokens[i]);
                        string phrase = line.Substring(line.IndexOf(searchPhrase));
                        if (phrase.Length > sample.Length) { sample = phrase; }
                    }
                    if (line.Contains(searchTokens[i]))
                    {
                        tokenMatches += line.Occurrences(searchPhrase);
                    }
                }
                texts.Enqueue(line);
            }
            for (int i = 0; i < searchTokens.Count; i++)
            {
                if (url.Contains(searchTokens[i])) { tokenMatches++; }
                if (host.Contains(searchTokens[i])) { tokenMatches++; }
            }
            if (url.Contains(searchPhrase)) { phraseMatches++; }

            WebPage page = new WebPage();
            page.setUrl(url);
            page.setCharacterCount(chars);
            page.setLineCount(texts.Count);
            page.setSearchPhraseCount(phraseMatches);
            page.setSearchTokensCount(tokenMatches);
            page.setInboundUrls(getHashesOfInboundUrls(links));
            page.setImageCount(images.Count);
            page.setWordCount(words);
            // remove new lines if any found
            page.setSampleText(sample.Replace("\n", ""));
            page.setHost(host);
            return page;
        }

        /// <summary>
        /// Returns the hashes of the inbound links
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        private int[] getHashesOfInboundUrls(Queue<string> urls)
        {
            int[] inboundUrls = new int[urls.Count];
            HashSet<int> set = new HashSet<int>();
            for (int i = 0; i < inboundUrls.Length; i++)
            {
                string str = urls.Dequeue();

                inboundUrls[i] = str.GetSpecialHashCode();
                urls.Enqueue(str);
            }
            return inboundUrls;
        }
    }
}
