﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace Basics
{
    public static class Utils
    {

        public static bool SequenceEqual<T>(this List<List<T>> a, List<List<T>> b)
        {
            if (b == null || a.Count != b.Count)
                return false;

            for (var i = 0; i < a.Count; i++)
            {
                if (a[i] == null && b[i] == null)
                    continue;
                if (a[i] == null || b[i] == null)
                    return false;
                if (a[i].Count != b[i].Count)
                    return false;
            }

            for (var i = 0; i < a.Count; i++)
            {
                if (a[i] == null) //Don't need to check b because the last loop makes sure they are equal
                    continue;
                if (!a[i].SequenceEqual(b[i]))
                    return false;
            }

            return true;
        }
        
        public static List<T> ExtractBatch<T>(this List<T> list, int count)
        {
            if (count < 0)
                throw new ArgumentException($"Batch size {count} is invalid.");
            if (list == null || list.Count <= 0 || count == 0)
                return new List<T>();
            count = Math.Min(list.Count, count);
            var batch = list.GetRange(0, count);
            list.RemoveRange(0, count);
            return batch;
        }

        public static List<List<T>> Batchify<T>(this List<T> list, int count)
        {
            if (count <= 0)
                throw new ArgumentException($"Batch size {count} is invalid.");
            if (list == null || list.Count <= 0)
                return new List<List<T>>();
            var batches = new List<List<T>>();
            for(var i = 0; i < list.Count; i+=count)
                batches.Add(list.GetRange(i, Min(list.Count - i, count)));
            return batches;
        }

        public static void ForEach<T>(this HashSet<T> set, Action<T> func)
        {
            foreach (var o in set)
                func(o);
        }
        

        #region Clamp / Max / Min
        public static void Clamp<T>(this List<T> list, T min, T max)
        {
            for (var i = 0; i < list.Count; i++)
                list[i] = Clamp(list[i], min, max);
        }
        public static T Clamp<T>(T value, T min, T max) => Max(Min(value, max), min);
        public static T Max<T>(params T[] values) => values.ToList().Max();
        public static T Min<T>(params T[] values) => values.ToList().Min();
        #endregion

        #region Math

        #region Constants
        public const double Pi = Math.PI;
        public const double Tau = 2 * Pi;
        public const double DegToRad = Pi / 180;
        public const double RadToDeg = 180 / Pi;
        #endregion

        #region AngleDifference
        public static float AngleDifferenceDegrees(float from, float to) => (float)AngleDifferenceDegrees((double)from, (double)to);
        public static double AngleDifferenceDegrees(double from, double to) => RadToDeg * AngleDifferenceRadians(from * DegToRad, to * DegToRad);
        public static float AngleDifferenceRadians(float from, float to) => (float)AngleDifferenceRadians((double)from, (double)to);
        public static double AngleDifferenceRadians(double from, double to)
        {
            var diff = to - from;
            while (diff > Pi) { diff -= Tau; }
            while (diff <= -Pi) { diff += Tau; }
            return diff;
        }
        #endregion

        #region Distance
        public static double ManhattanDistance(double x1, double y1, double x2, double y2) => Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        public static float ManhattanDistance(float x1, float y1, float x2, float y2) => Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        public static int ManhattanDistance(int x1, int y1, int x2, int y2) => Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        public static long ManhattanDistance(long x1, long y1, long x2, long y2) => Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        public static float Distance(Vector2 start, Vector2 finish) => EuclideanDistance(start, finish);
        public static float Distance(float x1, float y1, float x2, float y2) => EuclideanDistance(x1, y1, x2, y2);
        public static float Distance(int x1, int y1, int x2, int y2) => EuclideanDistance(x1, y1, x2, y2);
        public static float Distance(long x1, long y1, long x2, long y2) => EuclideanDistance(x1, y1, x2, y2);
        public static double Distance(double x1, double y1, double x2, double y2) => EuclideanDistance(x1, y1, x2, y2);
        public static float EuclideanDistance(Vector2 start, Vector2 finish) => (float)EuclideanDistance(start.X, start.Y, finish.X, finish.Y);
        public static float EuclideanDistance(float x1, float y1, float x2, float y2) => (float)EuclideanDistance((double)x1, y1, x2, y2);
        public static float EuclideanDistance(int x1, int y1, int x2, int y2) => (float)EuclideanDistance((double)x1, y1, x2, y2);
        public static float EuclideanDistance(long x1, long y1, long x2, long y2) => (float)EuclideanDistance((double)x1, y1, x2, y2);
        public static double EuclideanDistance(double x1, double y1, double x2, double y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        
        #endregion

        public static float EquilateralAltitude(float _sideLength) => (float)(Math.Sqrt(3) / 2 * _sideLength);

        public static bool IsEven(this int value) => value % 2 == 0;
        public static bool IsOdd(this int value) => value % 2 != 0;

        #endregion

        public static void Log(string message, bool newline = true, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Out.Write(message + (newline ? "\n" : ""));
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static string PrependToLength(string baseString, int length, string prefix="0")
        {
            if (baseString == null)
                throw new ArgumentNullException("Base string cannot be null");
            if (prefix == null)
                throw new ArgumentNullException("Prefix string cannot be null");
            if (prefix == "")
                throw new ArgumentException("Prefix string cannot be empty");
            var additionalPrefixes = length - baseString.Length;
            var str = new StringBuilder();
            int i = 0;
            while(i++ < additionalPrefixes)
                str.Append(prefix);
            str.Append(baseString);
            return str.ToString();
        }
        
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


        private static Random random = new Random();
        public static double RandomDouble() => random.NextDouble();
        public static int RandomInt() => random.Next();
        public static int RandomInt(int _min, int _max) => random.Next(_min, _max);


        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] = value;
        }
    }
}
