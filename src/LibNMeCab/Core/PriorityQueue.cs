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

        public void Push(T item)
        {
            int currentPos = this.list.Count;
            this.list.Add(item); // dummy

            while (currentPos != 0)
            {
                int parentPos = (currentPos - 1) / 2;
                T parent = this.list[parentPos];

                if (parent.CompareTo(item) <= 0) break;
                this.list[currentPos] = parent;
                currentPos = parentPos;
            }
            this.list[currentPos] = item;
        }

        public T Pop()
        {
            if (this.Count == 0) throw new InvalidOperationException("Empty");

            T ret = this.list[0];
            this.DeleteRoot();
            return ret;
        }

        private void DeleteRoot()
        {
            int tailPos = this.list.Count - 1;
            T current = this.list[tailPos];
            this.list.RemoveAt(tailPos);
            if (tailPos == 0) return; // empty
            tailPos--;

            int currentPos = 0;
            while (true)
            {
                int childPos = currentPos * 2 + 1; // left child
                if (childPos > tailPos) break;
                T child = this.list[childPos];

                int wrkPos = childPos + 1; // right child
                if (wrkPos <= tailPos)
                {
                    T wrk = this.list[wrkPos];
                    if (child.CompareTo(wrk) > 0)
                    {
                        childPos = wrkPos;
                        child = wrk;
                    }
                }

                if (current.CompareTo(child) < 0) break;
                this.list[currentPos] = child;
                currentPos = childPos;
            }
            this.list[currentPos] = current;
        }
    }
}
