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
    }
}
