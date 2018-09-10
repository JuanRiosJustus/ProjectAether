using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntoTheAether.src.structures
{
    public class LRUCache<K, V>
    {
        private class LRUNode
        {
            public K key = default(K);
            public V value = default(V);
            public LRUNode next = null;
            public LRUNode previous = null;
            public LRUNode(K k, V v) { key = k; value = v; }
        }

        private int limit = 0;
        private Dictionary<K, LRUNode> map = new Dictionary<K, LRUNode>();
        private LRUNode head = null;
        private LRUNode tail = null;

        public LRUCache(int capacity) { limit = capacity; }

        /// <summary>
        /// Returns an array of all keys within the cache
        /// </summary>
        /// <returns></returns>
        public K[] getKeys()
        {
            K[] array = new K[map.Count];
            int i = 0;
            foreach (K key in map.Keys)
            {
                array[i] = key;
                i++;
            }
            return array;
        }

        /// <summary>
        /// Retrieves the given value at the given index in the list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public V get(int index)
        {
            if (index > map.Count) { return default(V); }
            LRUNode iter = head;
            while (iter != null && index > 0)
            {
                iter = iter.next;
                index--;
            }
            return iter.value;
        }


        /// <summary>
        /// Retrieves the key within the cache if contained.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V get(K key)
        {
            if (map.ContainsKey(key))
            {
                LRUNode node = map[key];
                remove(node);
                add(node);
                return node.value;
            }
            return default(V);
        }

        /// <summary>
        /// Adds the key value pair to the cache, returns true if key is precent in the cache
        /// and returns false if there is no mapping and the key was to be inserted
        /// false if the 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool put(K key, V value)
        {
            if (map.ContainsKey(key))
            {
                LRUNode old = map[key];
                old.value = value;
                remove(old);
                add(old);
                return true;
            }
            else
            {
                // if adding one would put it over limit, dont add
                if (map.Count + 1 > limit)
                {
                    remove(tail);
                }
                if (map.Count < limit)
                {
                    add(new LRUNode(key, value));
                }
                return false;
            }
        }

        public bool willEvict() { return map.Count + 1 > limit; }
        public bool contains(K key) { return map.ContainsKey(key); }
        public V least() { return (tail == null ? default(V) : tail.value); }
        public int size() { return map.Count; }
        // Sets the head of the cache
        private void add(LRUNode node)
        {
            if (head == null || tail == null)
            {
                head = node;
                tail = node;
                map.Add(node.key, node);
            }
            else
            {
                node.previous = null;
                node.next = head;
                head.previous = node;
                head = node;
                map.Add(node.key, node);
            }
        }
        // removes all references of the given node within
        private void remove(LRUNode node)
        {
            if (node == null) { return; }
            if (node.previous != null)
            {
                node.previous.next = node.next;
            }
            if (node.next != null)
            {
                node.next.previous = node.previous;
            }
            if (node == tail)
            {
                tail = tail.previous;
            }
            if (node == head)
            {
                head = head.next;
            }
            map.Remove(node.key);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            LRUNode iter = head;
            while (iter != null)
            {
                sb.Append("[" + iter.value.ToString() + "]");
                iter = iter.next;
            }
            return sb.ToString();
        }
    }
}
