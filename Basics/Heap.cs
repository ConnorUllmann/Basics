using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basics
{
    class Heap<T> where T: IComparable
    {
        private bool min;
        private List<T> elements;

        public Heap(bool min = true)
        {
            this.min = min;
            elements = new List<T>();
        }

        public new string ToString() => string.Join(" ", elements);

        public bool Contains(T element) => elements.Contains(element);

        public void Add(T element)
        {
            elements.Add(element);
            Fix();
        }
        public void AddRange(IEnumerable<T> element)
        {
            elements.AddRange(element);
            Fix();
        }

        public void DeleteAt(int index)
        {
            var last = elements.Count - 1;
            elements[index] = elements[last];
            elements.RemoveAt(last);
            Fix();
        }

        public bool Empty() => elements.Count <= 0;

        public T Pop()
        {
            if (!Empty())
            {
                var element = elements[0];
                DeleteAt(0);
                return element;
            }
            return default(T);
        }

        public T Top => elements.Count > 0 ? elements.First() : default;

        public void Fix()
        {
            for (var i = elements.Count - 1; i > 0; i--)
            {
                int parentIndex = Math.Max(0, (i + 1) / 2 - 1);
                var result = elements[parentIndex].CompareTo(elements[i]);
                if ((min ? result : -result) > 0)
                {
                    var temp = elements[parentIndex];
                    elements[parentIndex] = elements[i];
                    elements[i] = temp;
                }
            }
        }
    }
}
