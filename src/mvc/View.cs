using System;
using System.Collections.Concurrent;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ProjectAether.src.structure
{
    public class View
    {
        private MainWindow m_mainWindow;
        private TextBox m_mainInputTextField;
        private TextBox m_mainDisplayTextBox;
        private TextBox m_secondaryDisplayTextBox;
        private ProgressBar m_progressBar;
        private Image m_imageContainer;
        private Button m_mainButton;

        public void setMainButton(Button btn)
        {
            if (m_mainButton == null)
            {
                m_mainButton = btn;
            }
        }

        public void setImageContainer(Image image)
        {
            if (m_imageContainer == null)
            {
                m_imageContainer = image;
            }
        }

        public void setProgressBar(ProgressBar bar)
        {
            if (m_progressBar == null)
            {
                m_progressBar = bar;
            }
        }

        public void setMainWindow(MainWindow mw)
        {
            if (m_mainWindow == null)
            {
                m_mainWindow = mw;
            }
        }

        public void setMainInputTextField(TextBox tb)
        {
            if (m_mainInputTextField == null)
            {
                m_mainInputTextField = tb;
            }
        }

        public void setMainDisplayTextBox(TextBox tb)
        {
            if (m_mainDisplayTextBox == null)
            {
                m_mainDisplayTextBox = tb;
            }
        }

        public void setSecondaryDisplayTexBox(TextBox tb)
        {
            if (m_secondaryDisplayTextBox == null)
            {
                m_secondaryDisplayTextBox = tb;
            }
        }
        /// <summary>
        /// Sets the view back to its initial state
        /// </summary>
        public void setToDefaultState()
        {
            m_mainButton.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => m_mainButton.IsEnabled = true));
            m_mainInputTextField.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => m_mainInputTextField.IsEnabled = true));
        }

        public void updateImageContainder(string img)
        {
            m_imageContainer.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => m_imageContainer.Source = new BitmapImage(new Uri(img))));
        }

        public void updateProgressBar(double d)
        {
            m_progressBar.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => m_progressBar.Value = d));
        }
       

        public void appendToMainTextBox(string str) { appendTextToTextBox(m_mainDisplayTextBox, str); }
        public void appendToSecondaryTextBox(string str) { appendTextToTextBox(m_secondaryDisplayTextBox, str); }

        private void appendTextToTextBox(TextBox textbox, string text)
        {
            textbox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => textbox.AppendText(text + "\n")));
        }

        private void clearTextFromTextBox(TextBox textbox)
        {
            textbox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => textbox.Clear()));
        }

        private void setCaretOnBottomTextBox(TextBox textbox)
        {
            textbox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => textbox.ScrollToEnd()));
        }

        /// <summary>
        /// Main method that writes info to the screen
        /// </summary>
        /// <param name="mainDisplayQueue"></param>
        /// <param name="secondaryDisplayQueue"></param>
        public void handleAppendage(ConcurrentQueue<string> mainDisplayQueue, ConcurrentQueue<string> secondaryDisplayQueue)
        {
            if (mainDisplayQueue.Count > 0)
            {
                string str = "";
                mainDisplayQueue.TryDequeue(out str);
                appendTextToTextBox(m_mainDisplayTextBox, str);
                setCaretOnBottomTextBox(m_mainDisplayTextBox);
            }

            if (secondaryDisplayQueue.Count > 0)
            {
                string str = "";
                secondaryDisplayQueue.TryDequeue(out str);
                clearTextFromTextBox(m_secondaryDisplayTextBox);
                appendTextToTextBox(m_secondaryDisplayTextBox, str);
            }
        }
    }
}
