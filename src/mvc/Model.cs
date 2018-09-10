using ProjectAether.src.configs;
using ProjectAether.src.contexts;
using ProjectAether.src.Utils;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ProjectAether.src.structure
{
    public class Model
    {
        private Random random = new Random();
        private Threadling[] m_threadlings = null;
        private AetherRepository m_repository = new AetherRepository();
        private ConfiguredSettings m_settings = null;
        private ConcurrentQueue<string> m_mainDisplayQueue = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> m_secondaryDisplayQueue = new ConcurrentQueue<string>();

        /// <summary>
        /// Deploys the underlying threads to begin crawling the web
        /// </summary>
        /// <param name="query"></param>
        public void searchAndDeployAetherlings(string query)
        {
            ConfigurationContext inputContext = new ConfigurationContext(m_mainDisplayQueue, m_secondaryDisplayQueue, query);
            m_settings = inputContext.releaseConfiguredSettings();
            m_mainDisplayQueue.Enqueue(" ");
            m_mainDisplayQueue.Enqueue(ApplicationConstants.STARTED_THREADLING_DEPLOYMENT);
            m_mainDisplayQueue.Enqueue(" ");
            constructAndDeployAetherlings(m_settings);
            m_mainDisplayQueue.Enqueue(" ");
            m_mainDisplayQueue.Enqueue(ApplicationConstants.FINISHED_THREADLING_DEPLOYMENT);
        }

        /// <summary>
        /// Constructs multiple search context instance
        /// </summary>
        /// <param name="contexts"></param>
        private void constructAndDeployAetherlings(ConfiguredSettings configs)
        {
            int deploys = Int32.Parse(configs.get(ConfiguredConstants.THREADLING_COUNT_LBL));
            m_threadlings = new Threadling[deploys];
            string[] faces = m_repository.getRandomFaces(deploys);
            for (int i = 0; i < m_threadlings.Length; i++)
            {
                string contextId = StringExtensionMethods.generateUsername(6);
                m_threadlings[i] = new Threadling(configs, m_mainDisplayQueue, m_secondaryDisplayQueue, faces[i]);
                m_threadlings[i].start();
                // threads must be space out to prevent intersecting traversals
                Thread.Sleep(ApplicationConstants.TEN_SECONDS_AS_MS);
            }
        }
        
        public ConcurrentQueue<string> getMainDisplayQueue() { return m_mainDisplayQueue; }
        public ConcurrentQueue<string> getSecondaryDisplayQueue() { return m_secondaryDisplayQueue; }
        
        /// <summary>
        /// returns the amount of precent of the crawling that
        /// if finished
        /// </summary>
        /// <returns></returns>
        public double getPercentageOfFinishedSearch() { return m_threadlings[0].getProgressToSearchLimit(); }

        /// <summary>
        /// if available, returns a string representing an image url
        /// </summary>
        /// <returns></returns>
        public bool tryRetrievingImageUrl(out string url) { return m_threadlings[0].tryRetrievingImageUrl(out url); }
        /// <summary>
        /// Sets the shared state to null if there are no 
        /// </summary>
        public void tryDestroyingSharedState()
        {
            if (m_threadlings == null || m_threadlings[0] == null) { return; }
            m_threadlings[0].tryDestroyingSharedState();
        }

        /// <summary>
        /// Returns true if there are threads currently running.
        /// </summary>
        /// <returns></returns>
        public bool isSearching()
        {
            if (m_threadlings == null) { return false; }
            bool hasActiveThread = false;
            for (int i = 0; i < m_threadlings.Length; i++)
            {
                if (m_threadlings[i] == null) { continue; }
                if (m_threadlings[i].isAlive()) { hasActiveThread = true; break; }
            }
            return hasActiveThread;
        }

        /// <summary>
        /// Stops all the threads by setting it to inactive
        /// </summary>
        public void stopSearch()
        {
            if (m_threadlings == null) { return; }
            // stop all threads
            for (int i = 0; i < m_threadlings.Length; i++)
            {
                if (m_threadlings[i] != null)
                {
                    m_threadlings[i].callback();
                }
            }
        }
    }
}
