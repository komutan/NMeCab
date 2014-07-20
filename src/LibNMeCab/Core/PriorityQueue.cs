using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Core
{
    public class PriorityQueue<T>
        where T : IComparable<T>
    {
        private readonly List<T> list = new List<T>();

        public int Count
        {
            get { return this.list.Count; }
        }

        public void Clear()
        {
            this.list.Clear();
        }

#if BinaryHeapPriorityQueue

        public void Push(T item)
        {
            int currentPos = this.list.Count;
            this.list.Add(item);

            while (currentPos != 0) // has more parrent
            {
                int parentPos = (currentPos - 1) / 2;
                T parent = this.list[parentPos];

                if (parent.CompareTo(item) <= 0) break; // parent is higher or equal

                this.list[parentPos] = item;
                this.list[currentPos] = parent;

                currentPos = parentPos;
            }
        }

        public T Pop()
        {
            if (this.Count == 0) throw new InvalidOperationException("Empty");

            T ret = this.list[0];
            this.DeleteTop();
            return ret;
        }

        private void DeleteTop()
        {
            int tailPos = this.list.Count - 1;
            if (tailPos == -1) return;
            T current = this.list[tailPos];
            this.list.RemoveAt(tailPos);
            if (tailPos == 0) return;
            tailPos--;

            int currentPos = 0;
            while (true)
            {
                int chiledPos = currentPos * 2 + 1;
                if (chiledPos > tailPos) break;
                T chiled = this.list[chiledPos];

                int wrkPos = chiledPos + 1;
                if (wrkPos <= tailPos)
                {
                    T wrk = this.list[wrkPos];
                    if (chiled.CompareTo(wrk) > 0)
                    {
                        chiledPos = wrkPos;
                        chiled = wrk;
                    }
                }

                if (current.CompareTo(chiled) < 0) break;
                this.list[currentPos] = chiled;
                currentPos = chiledPos;
            }
            this.list[currentPos] = current;
        }

#else

        public int Push(T item)
        {
            int index = this.SearchIndex(this.list, item);
            this.list.Insert(index, item);
            return index;
        }

        public T Pop()
        {
            if (this.Count == 0) throw new InvalidOperationException("Empty");

            T item = this.list[0];
            this.list.RemoveAt(0);
            return item;
        }

        private int SearchIndex(List<T> list, T item)
        {
            int head = 0;
            int tail = list.Count;

            while (head < tail)
            {
                int where = (head + tail) / 2;

                if (list[where].CompareTo(item) <= 0)
                {
                    head = where + 1;
                }
                else
                {
                    tail = where;
                }
            }

            return head;
        }

#endif
    }
}
