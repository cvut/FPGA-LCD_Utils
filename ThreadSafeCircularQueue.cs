using System;
using System.Collections.Generic;
using System.Text;

namespace LSPtools
{
    public class ThreadSafeCircularQueue<T> where T : class
    {
        private T?[] queue;
        private int ixhead = 0;
        private int ixtail = 0;
        private T? processing = null;
        private const int MINQUEUE = 4;

        public ThreadSafeCircularQueue(int sizequeue)
        {
            if (sizequeue < MINQUEUE) sizequeue = MINQUEUE;
            queue = new T[sizequeue];
            for (int i = 0; i < queue.Length; i++) queue[i] = null;
        }

        public int GetCount()
        {
            lock (this)
            {
                return ixhead < ixtail ? queue.Length + ixhead - ixtail : ixhead - ixtail;
            }
        }
        /// <summary>
        /// Prida pozadavek do fronty ke zpracovani, vraci -1 - fronta plna; n+2 - kde n je poradi ve fronte
        /// </summary>
        /// <param name="value">objekt ke zpracovani</param>
        /// <returns> -1 - fronta plna; n - kde n je poradi ve fronte</returns>
        public int EnqueueRequest(T value)
        {
            lock (this)
            {
                int ixhnew = ixhead + 1;
                if (ixhnew >= queue.Length) ixhnew = 0;
                if (ixhnew == ixtail) return -1; // queue full
                queue[ixhead = ixhnew] = value;
                return ixhead < ixtail ? queue.Length + ixhead - ixtail : ixhead - ixtail;
            }
        }
        /// <summary>
        /// Prida pozadavek do fronty ke zpracovani, pri plne fronte zrusi prvek vpredu, 
        /// vraci n+2 - kde n je poradi ve fronte
        /// </summary>
        /// <param name="value">objekt ke zpracovani</param>
        /// <returns> n - kde n je poradi ve fronte</returns>
        public int EnqueueRequestAlways(T value)
        {
            lock (this)
            {
                int ixhnew = ixhead + 1;
                if (ixhnew >= queue.Length) ixhnew = 0;
                if (ixhnew == ixtail) // full
                {
                    queue[ixtail++] = null;
                    if (ixtail >= queue.Length) ixtail = 0;
                }
                queue[ixhead = ixhnew] = value;
                return ixhead < ixtail ? queue.Length + ixhead - ixtail : ixhead - ixtail;
            }
        }

        public void ClearQueue()
        {
            lock (this)
            {
                for (int i = 0; i < ixhead; i++) queue[i] = null;
                ixhead = ixtail = 0;
            }
        }
        public bool IsNextToProcessing { get { return ixhead != ixtail; } }

        public T? NextToProcessing()
        {
            lock (this)
            {
                if (ixhead == ixtail) return processing = null;
                if (++ixtail >= queue.Length) ixtail = 0;
                processing = queue[ixtail]; queue[ixtail] = null;
                return processing;
            }

        }

        public T? PeekNext()
        {
            lock (this)
            {
                if (ixhead == ixtail) return (processing = null);
                int ixt = ixtail+1;
                if (ixt >= queue.Length) ixt = 0;
                processing = queue[ixt];
                return processing;
            }

        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int ixh = ixhead, ixt = ixtail, order = 0;

            // we prevent possibility of an endless loop by assuring 
            // that ixh and ixt are inside intervals <0,queue.Lenth>
            if (ixt >= queue.Length || ixt < 0) ixt = 0;
            if (ixh >= queue.Length || ixh < 0) ixh = 0;
            while (ixh != ixt)
            {
                sb.AppendFormat("[(0}:{1}]", order++, queue[ixt++]?.ToString());
                if (ixt >= queue.Length) ixt = 0;
            }
            return sb.Length > 0 ? sb.ToString() : "[queue is empty]";
        }
    }
}
