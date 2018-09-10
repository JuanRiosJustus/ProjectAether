using ProjectAether.src.structure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.contexts
{
    public class BaseContext
    {
        protected readonly ConcurrentQueue<string> mainDisplayQueue;
        protected readonly ConcurrentQueue<string> secondaryDisplayQueue;

        public BaseContext(ConcurrentQueue<string> mainDisplayQueue, ConcurrentQueue<string> secondaryDisplayQueue)
        {
            this.mainDisplayQueue = mainDisplayQueue;
            this.secondaryDisplayQueue = secondaryDisplayQueue;
        }

        public ConcurrentQueue<string> getMainDisplayQueue() { return mainDisplayQueue; }
        public ConcurrentQueue<string> getSecondaryDisplayQueue() { return secondaryDisplayQueue; }
    }
}
