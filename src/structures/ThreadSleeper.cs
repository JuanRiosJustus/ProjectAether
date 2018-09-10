using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.structures
{
    public class ThreadSleeper
    {
        private int sleepTimer;
        private Stopwatch watch = new Stopwatch();

        public ThreadSleeper(int sleepTimeSecondsAsMilliseconds)
        {
            sleepTimer = sleepTimeSecondsAsMilliseconds;
            watch.Start();
        }
        /// <summary>
        /// Puts the thread on hold until the elapsed time has reached the sleeptime
        /// </summary>
        public void trySleeping()
        {
            while (watch.ElapsedMilliseconds < sleepTimer)
            {
                // While the thread has not reached a certain time when trying to handle
                // loop until the elapsed time reaches/surpasses the sleeptimer
            }
            watch.Restart();
        }
        public int getTimeoutLimit() { return sleepTimer; }
        public void addTenSeconds() { sleepTimer += 10000; }
        public void restart() { watch.Restart(); }
    }
}
