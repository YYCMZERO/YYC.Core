using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AntJoin.Dapper.Context.Impl
{
    ////http://lrucache.codeplex.com/
    /// <summary>
    /// 默认15分钟内，访问20次以上的数据重新移到LRU头部
    /// 未达到20次的数据将在15分钟时超期，并自动被移除
    /// 缓存项默认为200
    /// </summary>
    public class LruCache :ICache
    {
        private readonly Dictionary<object, NodeInfo> cachedNodesDictionary = new Dictionary<object, NodeInfo>();
        private readonly LinkedList<NodeInfo> lruLinkedList = new LinkedList<NodeInfo>();

        private readonly uint maxSize;
        private readonly TimeSpan slidingTimeOut;
        private readonly uint frequentlyAccess;

        private static readonly ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();

        private Timer cleanupTimer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemExpiryTimeout">数据超时时间</param>
        /// <param name="frequentlyAccess">频繁访问次数（以便添加到最常访问)</param>
        /// <param name="maxCacheSize">总缓存数据条数</param>
        /// <param name="memoryRefreshInterval">检查超时数据的间隔毫秒</param>
        public LruCache(TimeSpan? itemExpiryTimeout
            , uint maxCacheSize = uint.MaxValue
            , uint frequentlyAccess = (uint) 20
            , uint memoryRefreshInterval = (uint)60000)
        {
            this.slidingTimeOut = itemExpiryTimeout ?? TimeSpan.FromMinutes(20);
            this.maxSize = maxCacheSize;
            this.frequentlyAccess = frequentlyAccess;
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            TimerCallback tcb = this.RemoveExpiredElements;
            this.cleanupTimer = new Timer(tcb, autoEvent, 0, (int)memoryRefreshInterval);
        }

        public void Insert(object key, object cacheObject)
        {
            Insert(key, cacheObject, this.slidingTimeOut);
        }
        public void Insert(object key, object cacheObject, TimeSpan timeout)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Debug.WriteLine($"Adding a cache object with key: {key.ToString()}");
            rwl.EnterWriteLock();
            try
            {
                NodeInfo node;
                if (this.cachedNodesDictionary.TryGetValue(key, out node))//相同key，替换
                    this.Delete(node);

                this.ShrinkToSize(this.maxSize - 1);
                this.CreateNodeandAddtoList(key, cacheObject, timeout);
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        public object Get(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            object data = null;
            NodeInfo node;
            rwl.EnterReadLock();
            try
            {
                if (this.cachedNodesDictionary.TryGetValue(key, out node))
                {
                    if (node != null)
                    {
                        Debug.WriteLine(string.Format("Cache hit for key: {0}", key.ToString()));
                        node.AccessCount++;
                        data = node.Value;

                        if (node.AccessCount > this.frequentlyAccess)
                        {
                            ThreadPool.QueueUserWorkItem(this.AddBeforeFirstNode, key);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine(string.Format("Cache miss for key: {0}", key.ToString()));
                }

                return data;
            }
            finally
            {
                rwl.ExitReadLock();
            }
        }

        private void RemoveExpiredElements(object stateInfo)
        {
            rwl.EnterWriteLock();
            try
            {
                while (this.lruLinkedList.Last != null)
                {
                    NodeInfo node = this.lruLinkedList.Last.Value;
                    if (node != null && node.IsExpired)
                    {
                        this.Delete(node);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        private void CreateNodeandAddtoList(object userKey, object cacheObject,TimeSpan timeout)
        {
            NodeInfo node = new NodeInfo(userKey, cacheObject, timeout);

            node.LLNode = this.lruLinkedList.AddFirst(node);
            this.cachedNodesDictionary[userKey] = node;
        }

        private void AddBeforeFirstNode(object stateinfo)
        {
            rwl.EnterWriteLock();
            try
            {
                object key = stateinfo;
                NodeInfo nodeInfo;
                if (this.cachedNodesDictionary.TryGetValue(key, out nodeInfo))
                {
                    if (nodeInfo != null && !nodeInfo.IsExpired && nodeInfo.AccessCount > this.frequentlyAccess)
                    {
                        if (nodeInfo.LLNode != this.lruLinkedList.First)
                        {
                            this.lruLinkedList.Remove(nodeInfo.LLNode);
                            nodeInfo.LLNode = this.lruLinkedList.AddBefore(this.lruLinkedList.First, nodeInfo);

                            Debug.WriteLine(string.Format("Add to LRU First: {0}", key.ToString()));
                            nodeInfo.ResetStatistic();
                        }
                    }
                }
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        private void ShrinkToSize(uint desiredSize)
        {
            while (this.cachedNodesDictionary.Count > desiredSize)
            {
                this.RemoveLeastValuableNode();
            }
        }

        private void RemoveLeastValuableNode()
        {
            if (this.lruLinkedList.Last != null)
            {
                NodeInfo node = this.lruLinkedList.Last.Value;
                this.Delete(node);
            }
        }

        private void Delete(NodeInfo node)
        {
            Debug.WriteLine(string.Format("Evicting object from cache for key: {0}", node.Key.ToString()));
            if(this.lruLinkedList.Count>0)
                this.lruLinkedList.Remove(node.LLNode);
            this.cachedNodesDictionary.Remove(node.Key);
        }

        public bool ContainsKey(object key)
        {
            return this.cachedNodesDictionary.ContainsKey(key);
        }
        public int Count { get { return this.cachedNodesDictionary.Count; } }
        public System.Collections.ICollection Keys { get { return this.cachedNodesDictionary.Keys; } }
        public void Clear()
        {
            this.lruLinkedList.Clear();
            this.cachedNodesDictionary.Clear();
        }
        public void Remove(object key)
        {
            NodeInfo node;
            if (this.cachedNodesDictionary.TryGetValue(key, out node))
            {
                this.lruLinkedList.Remove(node);
                this.cachedNodesDictionary.Remove(key);
            }
        }
        public void RemoveAll(System.Collections.ICollection keys)
        {
            foreach (object key in keys)
                Remove(key);
        }

        ////This class represents data stored in the LinkedList Node and Dictionary
        private class NodeInfo
        {
            private readonly TimeSpan slidingTimeOut;

            internal NodeInfo(object key, object value, TimeSpan slidingtimeout)
            {
                this.Key = key;
                this.Value = value;
                this.slidingTimeOut = slidingtimeout;
                this.LastAccessOn = DateTime.Now;
            }

            internal object Key { get; private set; }

            internal object Value { get; private set; }

            internal int AccessCount { get; set; }
            internal DateTime LastAccessOn { get; set; }

            internal LinkedListNode<NodeInfo> LLNode { get; set; }
            internal void ResetStatistic()
            {
                LastAccessOn = DateTime.Now;
                AccessCount = 0;
            }
            internal bool IsExpired
            {
                get{
                    return DateTime.Now.Subtract(LastAccessOn) > slidingTimeOut;
                }
            }
        }
    }

}
