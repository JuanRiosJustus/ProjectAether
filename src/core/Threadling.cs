using ProjectAether.src.contexts;
using System.Collections.Concurrent;
using System.Threading;

namespace ProjectAether.src.configs
{
    public class Threadling
    {
        private bool m_used;
        private Thread m_thread;
        private SearchContext m_context;

        public Threadling(ConfiguredSettings settings, ConcurrentQueue<string> mainDisplayQueue, ConcurrentQueue<string> secondaryDisplayQueue, string id)
        {
            m_context = new SearchContext(settings, mainDisplayQueue, secondaryDisplayQueue, id);
            m_thread = new Thread(m_context.search);
            m_thread.IsBackground = true;
            m_used = false;
        }

        /// <summary>
        /// Starts the underlying search context
        /// </summary>
        public void start()
        {
            if (m_used == false)
            {
                m_thread.Start();
                m_used = true;
            }
        }
        /// <summary>
        /// Stops the underlying search context's search
        /// </summary>
        public bool isAlive() { return m_thread.IsAlive; }
        public void callback() { m_context.callback(); }
        public double getProgressToSearchLimit() { return m_context.getProgressToSearchLimit(); }
        public bool tryRetrievingImageUrl(out string url) { return m_context.tryRetrievingImageUrl(out url); }
        public void tryDestroyingSharedState() { m_context.tryDestroyingSharedState(); }
    }
}
