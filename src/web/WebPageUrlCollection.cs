using System;
using System.Collections.Concurrent;

namespace ProjectAether.src.web
{
    public class WebPageUrlCollection
    {
        private bool useQueue = true;
        private ConcurrentQueue<string> queue = null;
        private ConcurrentStack<string> stack = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="usingQueue"></param>
        public WebPageUrlCollection(bool usingQueue)
        {
            useQueue = usingQueue;
            if (useQueue)
            {
                queue = new ConcurrentQueue<string>();
            }
            else
            {
                stack = new ConcurrentStack<string>();
            }
        }

        /// <summary>
        /// Adds the item to the collection
        /// </summary>
        /// <param name="collectable"></param>
        public void add(string collectable)
        {
            if (useQueue)
            {
                queue.Enqueue(collectable);
            }
            else
            {
                stack.Push(collectable);
            }
        }
        /// <summary>
        /// Attempts to release the element in the collection
        /// </summary>
        /// <param name="collectable"></param>
        /// <returns></returns>
        public bool tryRelease(out string collectable)
        {
            bool ret = false;
            if (useQueue)
            {
                ret = queue.TryDequeue(out collectable);
            }
            else
            {
                ret = stack.TryPop(out collectable);
            }
            return ret;
        }
        /// <summary>
        /// Checks to see if the collection is empty
        /// </summary>
        /// <returns></returns>
        public bool isEmpty()
        {
            bool empty = false;
            if (useQueue)
            {
                empty = queue.Count == 0;
            }
            else
            {
                empty = stack.Count == 0;
            }
            return empty;
        }
        /// <summary>
        /// returns true if the collection contains items
        /// </summary>
        /// <returns></returns>
        public bool isntEmpty()
        {
            if (useQueue)
            {
                return queue.Count > 0;
            }
            else
            {
                return stack.Count > 0;
            }
        }
        /// <summary>
        /// Returns the size of the collection
        /// </summary>
        /// <returns></returns>
        public int size()
        {
            int count = 0;
            if (useQueue)
            {
                count = queue.Count;
            }
            else
            {
                count = stack.Count;
            }
            return count;
        }
    }
}
