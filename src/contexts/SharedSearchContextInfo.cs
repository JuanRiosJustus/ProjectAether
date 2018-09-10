using BloomFilter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.contexts
{
    public class SharedSearchContextInfo
    {
        private int totalUrlsVisited = 0;
        private int threadsCounted = 0;
        private int imagesCounted = 0;
        private int threadsAsleep = 0;
        private Stopwatch stopwatch = new Stopwatch();
        private ConcurrentDictionary<string, int> threadMap = new ConcurrentDictionary<string, int>();
        private Filter<string> imageUrlSet = new Filter<string>(ApplicationConstants.GREATER_STORAGE_LIMIT);
        private ConcurrentQueue<string> imageUrls = new ConcurrentQueue<string>();

        public SharedSearchContextInfo() { stopwatch.Start();  }


        public void incrementThreadSleepCounter() { threadsAsleep++; }
        public int getThreadSleepCounter() { return threadsAsleep; }
        public void decrementThreadSleepCounter() { threadsAsleep--; }
        /// <summary>
        /// Attempts to retrieves the next element in the queue
        /// which returns true if allowed
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool tryRetrievingImageUrl(out string url)
        {
            return imageUrls.TryDequeue(out url);
        }

        /// <summary>
        /// Attempts to add the given urls to the underlying queue
        /// if not contained within the internal set
        /// </summary>
        /// <param name="urls"></param>
        public void tryEnqueueImageUrl(Queue<string> urls)
        {
            while(urls.Count > 0)
            {
                string picUrl = urls.Dequeue();
                if (imageUrlSet.Contains(picUrl)) { continue; }
                imageUrls.Enqueue(picUrl);
                imagesCounted++;
                imageUrlSet.Add(picUrl);
            }
        }
        public int getElapsedTimeInSeconds() { return (int)(stopwatch.ElapsedMilliseconds / 1000.0); }
        public int getImagesFound() { return imagesCounted; }
        public void incrementUrlsTraversed() { totalUrlsVisited++; }
        public int amountOfWebpageUrlsVisited() { return totalUrlsVisited; }
        public void incrementThreadCounter() { threadsCounted++; }
        public int amountOfThreads() { return threadsCounted; }
        public bool hasRemainingThreads() { return threadsCounted > 0; }
        public void decrementThreadCounter() { threadsCounted--; }
        public void addThreadId(string id) { threadMap[id] = 1; }
        public void addToThreadScore(string id, int amount)
        {
            if (threadMap.ContainsKey(id))
            {
                threadMap[id] += amount;
            }
            else
            {
                threadMap[id] = 1;
            }
        }
        public string viewOfThreadMap()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in threadMap.Keys)
            {
                sb.Append(str + " -> " + threadMap[str] + "\n");
            }
            return sb.ToString();
        }
    }
}
