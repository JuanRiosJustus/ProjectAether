using ProjectAether.src.structures;
using ProjectAether.src.web;
using ProjectAether.src.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IntoTheAether.src.core;

namespace ProjectAether.src.globals
{
    public class Hub
    {
        private static Hub m_instance = null;
        private Hub() { }
        public static Hub getInstance()
        {
            if (m_instance == null)
            {
                m_instance = new Hub();
            }
            return m_instance;
        }

        private WebPagePool webpagePool = WebPagePool.getInstance();

        ////////////////// MAIN METHODS /////////////////////////////////////////////
        /// <summary>
        /// Adds the webpage pool
        /// </summary>
        /// <param name="page"></param>
        public void add(WebPage page) { webpagePool.add(page); }

        public string analyze() { return new HubAnalyzer(webpagePool.toArray()).analyzation(); }
        
        public int size() { return webpagePool.size(); }
    }
}
