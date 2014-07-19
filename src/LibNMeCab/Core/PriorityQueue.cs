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
            this.list[0] = this.list[tailPos];
            this.list.RemoveAt(tailPos);
            if (tailPos == 0) return;
            tailPos--;
            
            int currentPos = 0;
            T current = this.list[0];
            while (true)
            {
                int leftPos = currentPos * 2 + 1;
                if (leftPos > tailPos) break;
                int rightPos = leftPos + 1;

                int chiledPos;
                T chiled;
                if (rightPos > tailPos)
                {
                    chiledPos = leftPos;
                    chiled = this.list[chiledPos];
                }
                else
                {
                    T left = this.list[leftPos];
                    T right = this.list[rightPos];
                    if (left.CompareTo(right) < 0)
                    {
                        chiledPos = leftPos;
                        chiled = left;
                    }
                    else
                    {
                        chiledPos = rightPos;
                        chiled = right;
                    }
                }

                if (current.CompareTo(chiled) < 0) break;
                this.list[currentPos] = chiled;
                this.list[chiledPos] = current;
                current = chiled;
                currentPos = chiledPos;
            }
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
