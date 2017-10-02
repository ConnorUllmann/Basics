using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Basics.Sorting
{
    public static class SortTests
    {
        public delegate void Sort(List<int> elements);

        static void TestOrdersOfMagnitudeForSortFunction(string name, Sort sortFunction)
        {
            TestOrdersOfMagnitudeForSortFunctions(new Dictionary<string, Sort>() { [name] = sortFunction });
        }

        public static void TestOrdersOfMagnitudeForSortFunctions(Dictionary<string, Sort> sortFunctions)
        {
            var digitsStart = 10;
            var digits = 20;
            for (var i = digitsStart; i <= digits; i++)
            {
                var length = (int)Math.Pow(2, i);
                int counter = 0;
                foreach (var sortFunction in sortFunctions)
                {
                    Console.WriteLine($"[{counter}/{digits}] " + TestString(sortFunction.Key, sortFunction.Value, length, 1));
                    counter++;
                }
                Console.WriteLine(" --- ");
            }
            Console.WriteLine("Complete!");
        }

        static string TestString(string name, Sort _sortFunction, int length, int count)
        {
            return $"[n:{Utils.PrependToLength(length.ToString(), 7, " ")}] {Utils.PrependToLength(name, 13, " ")}: {AverageDuration(_sortFunction, length, count)}s";
        }

        static double AverageDuration(Sort _sortFunction, int length, int count)
        {
            long averageMilliseconds = 0;
            var stopwatch = new Stopwatch();
            for (var i = 0; i < count; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                if (!TestSort(_sortFunction, length))
                    Console.WriteLine("ERROR!");
                stopwatch.Stop();
                averageMilliseconds += stopwatch.ElapsedMilliseconds;
            }
            return averageMilliseconds / count / 1000.0;
        }

        static bool TestSort(Sort _sortFunction, int length)
        {
            var groundTruth = new List<int>();
            var elements = new List<int>();

            for (var i = 0; i < length; i++)
            {
                groundTruth.Add(i);
                elements.Add(i);
            }
            elements.Shuffle();
            _sortFunction(elements);
            
            for (var i = 0; i < groundTruth.Count; i++)
                if (elements[i] != groundTruth[i])
                    return false;
            return true;
        }
    }
}
