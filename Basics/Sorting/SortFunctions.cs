using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basics.Sorting
{
    public static class SortFunctions
    {
        public static void BubbleSort(this List<int> elements) 
        {
            for (var i = 0; i < elements.Count; i++)
            {
                for (var j = 0; j < elements.Count - 1; j++)
                {
                    if (elements[j] > elements[j + 1])
                    {
                        var temp = elements[j];
                        elements[j] = elements[j + 1];
                        elements[j + 1] = temp;
                    }
                }
            }
        }

        public static void InsertionSort(this List<int> elements)
        {
            for (var i = 0; i < elements.Count; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    if (elements[i] < elements[j])
                    {
                        var element = elements[i];
                        elements.RemoveAt(i);
                        elements.Insert(j, element);
                        break;
                    }
                }
            }
        }

        public static void HeapSort(this List<int> elements)
        {
            var heap = new Heap<int>();
            heap.AddRange(elements);
            for (var i = 0; i < elements.Count; i++)
                elements[i] = heap.Pop();
        }

        public static void QuickSort(this List<int> elements)
        {
            QuickSort_Helper(ref elements, 0, elements.Count - 1, "");
        }

        public static void MergeSort(this List<int> elements)
        {
            MergeSort_Helper(ref elements, 0, elements.Count);
        }

        #region Helper Functions
        internal static void QuickSort_Helper(ref List<int> elements, int p, int r, string s)
        {
            if (r - p <= 0)
                return;
            if (r - p == 1)
            {
                if (elements[r] < elements[p])
                {
                    var temp = elements[p];
                    elements[p] = elements[r];
                    elements[r] = temp;
                }
                else
                    return;
            }

            var pivot = elements[r];
            var less = new List<int>();
            var more = new List<int>();
            for (var i = p; i < r; i++)
            {
                if (elements[i] < pivot)
                    less.Add(i);
                else
                    more.Add(i);
            }

            var p0 = p;
            var r0 = p0 + less.Count - 1;
            var newValues = new List<int>();
            for (var i = 0; i < less.Count; i++)
            {
                newValues.Add(elements[less[i]]);
            }
            newValues.Add(pivot);
            var p1 = r0 + 2;
            var r1 = p1 + more.Count - 1;
            for (var i = 0; i < more.Count; i++)
            {
                newValues.Add(elements[more[i]]);
            }

            for (var i = 0; i < newValues.Count; i++)
            {
                elements[p + i] = newValues[i];
            }

            QuickSort_Helper(ref elements, p0, r0, s + ".");
            QuickSort_Helper(ref elements, p1, r1, s + ".");

        }

        public static void MergeSort_Helper(ref List<int> elements, int start, int length)
        {
            if (length <= 1)
                return;

            var startA = start;
            var lengthA = (int)(length / 2);
            var startB = startA + lengthA;
            var lengthB = length - lengthA;
            MergeSort_Helper(ref elements, startA, lengthA);
            MergeSort_Helper(ref elements, startB, lengthB);

            var i = startA;
            var j = startB;

            var newElements = new List<int>();

            while (true)
            {
                if (i >= startA + lengthA)
                {
                    if (j >= startB + lengthB)
                        break;
                    newElements.Add(elements[j++]);
                }
                else
                {
                    if (j >= startB + lengthB || elements[i] <= elements[j])
                        newElements.Add(elements[i++]);
                    else
                        newElements.Add(elements[j++]);
                }
            }

            for (var k = 0; k < newElements.Count; k++)
            {
                elements[startA + k] = newElements[k];
            }
        }
        #endregion
    }
}
