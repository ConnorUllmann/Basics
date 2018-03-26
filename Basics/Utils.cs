using System;
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

        public static void ForEach<T>(this HashSet<T> _set, Action<T> _func)
        {
            foreach (var o in _set)
                _func(o);
        }

        public static void AddRange<T>(this HashSet<T> _set, IEnumerable<T> _range)
        {
            foreach (var o in _range)
                _set.Add(o);
        }

        public static T Sample<T>(this HashSet<T> _set) => _set.Count > 0 ? _set.ElementAt(RandomInt() % _set.Count) : default;
        public static T Sample<T>(this List<T> _set) => _set.Count() > 0 ? _set.ElementAt(RandomInt() % _set.Count) : default;
        public static T Sample<T>(this T[] _set) => _set.Count() > 0 ? _set.ElementAt(RandomInt() % _set.Count()) : default;

        /// <summary>
        /// Removes the IPositions from the given _set if they are outside the given _distance from the given _position
        /// </summary>
        /// <typeparam name="T">IPosition type of element in the set</typeparam>
        /// <param name="_set">set of positions</param>
        /// <param name="_position">position against which all elements of the set will be compared</param>
        /// <param name="_distance">distance from _position beyond which any elements of the set will be removed</param>
        public static void RemoveWhereOutsideRange<T>(this HashSet<T> _set, IPosition _position, float _distance) where T : IPosition
        {
            var distanceSquared = _distance * _distance;
            _set.RemoveWhere(o => o.DistanceSquared(_position) > distanceSquared);
        }

        public static T Pop<T>(this HashSet<T> _set)
        {
            if (_set == null || _set.Count == 0)
                return default;

            var o = _set.Sample();
            _set.Remove(o);
            return o;
        }

        public static bool Any<T>(this HashSet<T> _set, Func<T, bool> _func)
        {
            foreach (var o in _set)
                if (_func(o))
                    return true;
            return false;
        }

        #region Clamp / Max / Min
        public static void Clamp<T>(this List<T> list, T min, T max)
        {
            for (var i = 0; i < list.Count; i++)
                list[i] = Clamp(list[i], min, max);
        }
        public static T Clamp<T>(T value, T min, T max) => Max(Min(value, max), min);
        //These might seem superfluous, but I had trouble using Math.Min/Math.Max for Clamp without them
        public static T Max<T>(params T[] values) => values.Max();
        public static T Min<T>(params T[] values) => values.Min();
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

        public static float ManhattanDistance(this IPosition a, IPosition b) => ManhattanDistance(a.X, a.Y, b.X, b.Y);
        public static float ManhattanDistance(this IPosition a, float x1, float y1) => ManhattanDistance(a.X, a.Y, x1, y1);
        public static double ManhattanDistance(double x1, double y1, double x2, double y2) => Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        public static float ManhattanDistance(float x1, float y1, float x2, float y2) => Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        public static int ManhattanDistance(int x1, int y1, int x2, int y2) => Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        public static long ManhattanDistance(long x1, long y1, long x2, long y2) => Math.Abs(x2 - x1) + Math.Abs(y2 - y1);

        public static float Distance(this IPosition a, IPosition b) => EuclideanDistance(a.X, a.Y, b.X, b.Y);
        public static float Distance(this IPosition a, float x1, float y1) => EuclideanDistance(a.X, a.Y, x1, y1);
        public static float Distance(float x1, float y1, float x2, float y2) => EuclideanDistance(x1, y1, x2, y2);
        public static float Distance(int x1, int y1, int x2, int y2) => EuclideanDistance(x1, y1, x2, y2);
        public static float Distance(long x1, long y1, long x2, long y2) => EuclideanDistance(x1, y1, x2, y2);
        public static double Distance(double x1, double y1, double x2, double y2) => EuclideanDistance(x1, y1, x2, y2);

        public static float DistanceSquared(this IPosition a, IPosition b) => EuclideanDistanceSquared(a.X, a.Y, b.X, b.Y);
        public static float DistanceSquared(this IPosition a, float x1, float y1) => EuclideanDistanceSquared(a.X, a.Y, x1, y1);
        public static float DistanceSquared(float x1, float y1, float x2, float y2) => EuclideanDistanceSquared(x1, y1, x2, y2);
        public static float DistanceSquared(int x1, int y1, int x2, int y2) => EuclideanDistanceSquared(x1, y1, x2, y2);
        public static float DistanceSquared(long x1, long y1, long x2, long y2) => EuclideanDistanceSquared(x1, y1, x2, y2);
        public static double DistanceSquared(double x1, double y1, double x2, double y2) => EuclideanDistanceSquared(x1, y1, x2, y2);

        public static float EuclideanDistance(this IPosition a, IPosition b) => (float)EuclideanDistance((double)a.X, a.Y, b.X, b.Y);
        public static float EuclideanDistance(this IPosition a, float x1, float y1) => (float)EuclideanDistance((double)a.X, a.Y, x1, y1);
        public static float EuclideanDistance(float x1, float y1, float x2, float y2) => (float)EuclideanDistance((double)x1, y1, x2, y2);
        public static float EuclideanDistance(int x1, int y1, int x2, int y2) => (float)EuclideanDistance((double)x1, y1, x2, y2);
        public static float EuclideanDistance(long x1, long y1, long x2, long y2) => (float)EuclideanDistance((double)x1, y1, x2, y2);
        public static double EuclideanDistance(double x1, double y1, double x2, double y2) => Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));

        public static float EuclideanDistanceSquared(this IPosition a, IPosition b) => (float)EuclideanDistanceSquared((double)a.X, a.Y, b.X, b.Y);
        public static float EuclideanDistanceSquared(this IPosition a, float x1, float y1) => (float)EuclideanDistanceSquared((double)a.X, a.Y, x1, y1);
        public static float EuclideanDistanceSquared(float x1, float y1, float x2, float y2) => (float)EuclideanDistanceSquared((double)x1, y1, x2, y2);
        public static float EuclideanDistanceSquared(long x1, long y1, long x2, long y2) => (float)EuclideanDistanceSquared((double)x1, y1, x2, y2);
        public static float EuclideanDistanceSquared(int x1, int y1, int x2, int y2) => (float)EuclideanDistanceSquared((double)x1, y1, x2, y2);
        public static double EuclideanDistanceSquared(double x1, double y1, double x2, double y2) => (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);

        public static (float X, float Y) Midpoint(this IPosition a, IPosition b) => Midpoint(a.X, a.Y, b.X, b.Y);
        public static (float X, float Y) Midpoint(this IPosition a, float x1, float y1) => Midpoint(a.X, a.Y, x1, y1);
        public static (float X, float Y) Midpoint(int x1, int y1, int x2, int y2) => Midpoint(x1, y1, x2, y2);
        public static (float X, float Y) Midpoint(long x1, long y1, long x2, long y2) => Midpoint(x1, y1, x2, y2);
        public static (float X, float Y) Midpoint((float X, float Y) a, (float X, float Y) b) => Midpoint(a.X, a.Y, b.X, b.Y);
        public static (float X, float Y) Midpoint((float X, float Y) a, float x1, float y1) => Midpoint(a.X, a.Y, x1, y1);
        public static (float X, float Y) Midpoint(float x1, float y1, float x2, float y2) => ((x1 + x2) / 2, (y1 + y2) / 2);


        /// <summary>
        /// Finds the position in the group which is closest to the target position
        /// </summary>
        /// <typeparam name="T">type of element in group</typeparam>
        /// <typeparam name="T">type of target</typeparam>
        /// <param name="list">group of positions to compare</param>
        /// <param name="target">position to compare against</param>
        /// <returns></returns>
        public static T NearestTo<T, U>(this IEnumerable<T> list, U target) 
            where T : IPosition 
            where U : IPosition
        {
            float? minDistance = null;
            T minObject = default;
            foreach (var obj in list)
            {
                //Don't need to compare actual distances (which would require a Math.Sqrt call)
                var distance = obj.DistanceSquared(target);
                if (!minDistance.HasValue || distance < minDistance.Value)
                {
                    minDistance = distance;
                    minObject = obj;
                }
            }
            return minObject;
        }

        #endregion

        private static readonly float EquilateralAltitudeCoefficient = (float)(Math.Sqrt(3) / 2);
        public static float EquilateralAltitude(float _sideLength) => EquilateralAltitudeCoefficient * _sideLength;

        public static bool IsEven(this int value) => value % 2 == 0;
        public static bool IsOdd(this int value) => value % 2 != 0;

        #endregion

        public static void Repetitions(this int _count, Action _action)
        {
            for (var i = 0; i < _count; i++)
                _action();
        }

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
        public static IList<T> Shuffled<T>(this IList<T> list)
        {
            list.Shuffle();
            return list;
        }

        private static Random random = new Random();
        /// <returns>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        public static double RandomDouble() => random.NextDouble();
        public static int RandomInt() => random.Next();
        public static int RandomInt(int _max) => random.Next(0, _max);
        public static int RandomInt(int _min, int _max) => random.Next(_min, _max);
        public static int RandomSign() => RandomInt() % 2 == 0 ? 1 : -1;
        public static double RandomAngleRad() => RandomDouble() * Math.PI * 2;
        public static double RandomAngleDeg() => RandomDouble() * 360;

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] = value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> addFunc = null)
        {
            if (!dict.TryGetValue(key, out var value))
            {
                if (addFunc != null)
                    value = addFunc();
                else
                    value = default;
                dict.Add(key, value);
            }
            return value;
        }

        public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
            => dict.GetOrAdd(key, () => new TValue());

        public static List<T> Reversed<T>(this List<T> list)
        {
            list.Reverse();
            return list;
        }

        public static int Count<T>() => Enum.GetNames(typeof(T)).Length;
    }
}
