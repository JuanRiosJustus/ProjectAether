using System;
using System.Threading;

namespace ProjectAether.src.structure
{
    public class Controller
    {
        private Model m_model;
        private View m_view;

        public Controller(Model model, View view)
        {
            m_model = model;
            m_view = view;
        }

        public void search(string query)
        {
            // start the web crawlers
            new Thread( () => { m_model.searchAndDeployAetherlings( query ); } ).Start();

            // wait for the web crawlers to start, or for a time out, whichever comes first
            int timeout = ApplicationConstants.TIMEOUT_COUNTDOWN;
            while (m_model.isSearching() == false && timeout > 0)
            {
                Thread.Sleep(ApplicationConstants.FIVE_SECONDS_AS_MS);
                timeout--;
            }
            if (timeout < 0) { releaseMessages(); return; }
            // keep gettings the output and printing until all threads have stopped.
            startSecondaryDisplayUpdaterThread();
            startImageShowerThread();
            startProgressBarThread();
        }

        /// <summary>
        /// Starts the thread responsible of polling the images
        /// </summary>
        private void startImageShowerThread()
        {
            new Thread(() =>
            {
                while (m_model.isSearching())
                {
                    string imageUrl = string.Empty;
                    if (m_model.tryRetrievingImageUrl(out imageUrl))
                    {
                        m_view.updateImageContainder(imageUrl);
                        Thread.Sleep(ApplicationConstants.TEN_SECONDS_AS_MS);
                    }
                }
                releaseMessages();
            }).Start();
        }
        /// <summary>
        /// Starts the thread which updates the seconday display
        /// </summary>
        private void startSecondaryDisplayUpdaterThread()
        {
            new Thread(() =>
            {
                while (m_model.isSearching())
                {
                    m_view.handleAppendage(m_model.getMainDisplayQueue(), m_model.getSecondaryDisplayQueue());
                }
                releaseMessages();
            }).Start();
        }

        /// <summary>
        /// Starts the thread responsible of showing
        /// and updating the progress bar
        /// </summary>
        private void startProgressBarThread()
        {
            new Thread(() =>
            {
                while (m_model.isSearching())
                {
                    m_view.updateProgressBar(m_model.getPercentageOfFinishedSearch() * 100);
                    Thread.Sleep(ApplicationConstants.FIVE_SECONDS_AS_MS);
                }
                releaseMessages();
            }).Start();
        }
        /// <summary>
        /// Releases the messages to their screen 
        /// </summary>
        public void releaseMessages()
        {
            string str = String.Empty;
            while(m_model.getMainDisplayQueue().Count > 0)
            {
                str = String.Empty;
                m_model.getMainDisplayQueue().TryDequeue(out str);
                m_view.appendToMainTextBox(str);
            }

            while (m_model.getSecondaryDisplayQueue().Count > 0)
            {
                str = String.Empty;
                m_model.getSecondaryDisplayQueue().TryDequeue(out str);
                m_view.appendToSecondaryTextBox(str);
            }
            tryResettingButtonState();
        }
        private void tryResettingButtonState() { m_view.setToDefaultState(); }
        public void tryDestroyingSharedState() { m_model.tryDestroyingSharedState(); }
        public void stopSearch() { m_model.stopSearch(); }
        public bool isSearching() { return m_model.isSearching(); }
    }
}
