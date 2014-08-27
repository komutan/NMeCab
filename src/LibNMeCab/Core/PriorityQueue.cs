using System;
using System.Collections.Generic;
using System.Text;

namespace NMeCab.Core
{
    public class PriorityQueue<T>
        where T : IComparable<T>
    {
        private readonly List<T> heapList = new List<T>();

        public int Count
        {
            get { return this.heapList.Count; }
        }

        public void Clear()
        {
            this.heapList.Clear();
        }

        public void Push(T item)
        {
            if (item == null) throw new ArgumentNullException("item");

            //up heap
            int currentPos = this.heapList.Count; //tail
            this.heapList.Add(default(T));
            while (currentPos != 0)
            {
                int parentPos = (currentPos - 1) / 2;
                T parent = this.heapList[parentPos];

                if (parent.CompareTo(item) <= 0) break;

                this.heapList[currentPos] = parent; //down
                currentPos = parentPos;
            }
            this.heapList[currentPos] = item; //commit
        }

        public T Pop()
        {
            if (this.heapList.Count == 0) throw new InvalidOperationException("Empty");

            T root = this.heapList[0];

            int tailPos = this.heapList.Count - 1;
            if (tailPos != 0)
            {
                //down heap
                T tail = this.heapList[tailPos];
                int currentPos = 0;
                while (true) 
                {
                    int childPos = currentPos * 2 + 1; //left child
                    if (childPos >= tailPos) break;
                    T child = this.heapList[childPos];

                    int rChiledPos = childPos + 1; //right child
                    if (rChiledPos < tailPos)
                    {
                        T rChiled = this.heapList[rChiledPos];
                        if (child.CompareTo(rChiled) > 0)
                        {
                            child = rChiled;
                            childPos = rChiledPos;
                        }
                    }

                    if (tail.CompareTo(child) < 0) break;

                    this.heapList[currentPos] = child; //up
                    currentPos = childPos;
                }
                this.heapList[currentPos] = tail; //commit
            }
            this.heapList.RemoveAt(tailPos);

            return root;
        }
    }
}
