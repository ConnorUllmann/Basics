using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace Basics
{
    public static class Utils
    {
        public static T? EnumFromString<T>(string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if(int.TryParse(value, out int valueInt))
                return Enum.IsDefined(typeof(T), valueInt)
                    ? (T)Enum.Parse(typeof(T), value, false)
                    : (T?)null;
            return Enum.TryParse(value, out T valueEnum) 
                ? valueEnum 
                : (T?)null;
        }

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
        public static object Sample(this Array _set) => _set.Length > 0 ? _set.GetValue(RandomInt() % _set.Length) : default;
        public static T Sample<T>(this Type _type) //Enums only
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"Type {typeof(T).FullName} must be an enum to be sampled with this function.");
            if (!_type.IsEnum)
                throw new ArgumentException($"Type {_type.FullName} must be an enum to be sampled with this function.");
            return (T)Enum.GetValues(typeof(T)).Sample();
        }
        public static T Sample<T>() //Enums only
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"Type {typeof(T).FullName} must be an enum to be sampled with this function.");
            return (T)Enum.GetValues(typeof(T)).Sample();
        }

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

        public static void EnqueueRange<T>(this Queue<T> _queue, IEnumerable<T> _range)
        {
            foreach (var o in _queue)
                _queue.Enqueue(o);
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
        
        /// <summary>
        /// Calculates the positions of all vertices for a circle at the given position with the given radius.
        /// </summary>
        /// <param name="_centerX">x-position of the center of the circle</param>
        /// <param name="_centerY">y-position of the center of the circle</param>
        /// <param name="_radius">radius of the circle</param>
        /// <returns>Returns the vertex positions that make up a circle at the given position with the given radius</returns>
        public static IEnumerable<(float X, float Y)> CircleVertices(float _centerX, float _centerY, float _radius)
        {
            var segments = 10 * Math.Sqrt(_radius);
            var theta = (float)(2 * Math.PI / segments);
            var cos = (float)Math.Cos(theta);
            var sin = (float)Math.Sin(theta);

            var x = _radius;
            var xt = 0f;
            var y = 0f;
            
            for (var i = 0; i < segments; i++)
            {
                yield return (x + _centerX, y + _centerY);

                xt = x;
                x = cos * x - sin * y;
                y = sin * xt + cos * y;
            }
        }
        public static IEnumerable<(float X, float Y)> CircleVertices((float X, float Y) _position, float _radius) => CircleVertices(_position.X, _position.Y, _radius);
        public static IEnumerable<((float X, float Y) a, (float X, float Y) b)> CircleSegments(float _centerX, float _centerY, float _radius)
        {
            var vertices = CircleVertices(_centerX, _centerY, _radius).ToList();
            for(var i = 0; i < vertices.Count; i++)
                yield return (vertices[i], vertices[(i + 1) % vertices.Count]);
        }

        private static readonly float EquilateralAltitudeCoefficient = (float)(Math.Sqrt(3) / 2);
        public static float EquilateralAltitude(float _sideLength) => EquilateralAltitudeCoefficient * _sideLength;

        public static bool IsEven(this int value) => value % 2 == 0;
        public static bool IsOdd(this int value) => value % 2 != 0;

        #endregion

        /// <summary>
        /// Given a start position, end position, and two lengths, this will find the two positions the joint could be placed in order
        /// to make the two lengths form a bridge from the start position to the end position (i.e. kinematics)
        /// </summary>
        /// <param name="startX">x-position of start of arm with length "jointLength1"</param>
        /// <param name="startY">y-position of start of arm with length "jointLength1"</param>
        /// <param name="targetX">x-position of end of arm with length "jointLength2"</param>
        /// <param name="targetY">y-position of end of arm with length "jointLength2"</param>
        /// <param name="jointLength1">length of first arm</param>
        /// <param name="jointLength2">length of second arm</param>
        /// <param name="bendDirectionLeft">direction the elbow should bend (relative to the vector from start to target)</param>
        /// <returns>the point (if it exists) where the two arms should meet to reach from 
        /// start to target bending in the specified direction</returns>
        public static (float X, float Y)? SingleJoint(float startX, float startY, float targetX, float targetY, 
            float jointLength1, float jointLength2, bool bendDirectionLeft=true)
        {
            var a = 2 * (targetX - startX);
            var b = startX*startX + startY*startY - jointLength1*jointLength1 - targetX*targetX - targetY*targetY + jointLength2*jointLength2;
            var c = 2 * (startY - targetY);

            if (c == 0)
                return null;

            // Quadratic formula
            var d = a * a / (c * c) + 1;
            var e = 2 * (a * b / (c * c) - a * startY / c - startX);
            var f = startX * startX + b * b / (c * c) - 2 * startY * b / c + startY * startY - jointLength1 * jointLength1;
            var g = e * e - 4 * d * f; 

            if (g < 0)
                return null;

            var gsq = Math.Sqrt(g);
            var x1 = (-e + gsq) / (2 * d);
            var y1 = (a * x1 + b) / c;
            var sideResult = Math.Sign((targetX - startX) * (y1 - startY) - (targetY - startY) * (x1 - startX));
            if (sideResult == (bendDirectionLeft ? 1 : -1))
                return ((float)x1, (float)y1);

            var x2 = (-e - gsq) / (2 * d);
            var y2 = (a * x2 + b) / c;
            return ((float)x2, (float)y2);
        }

        public static void Repetitions(this int _count, Action _action)
        {
            for (var i = 0; i < _count; i++)
                _action();
        }

        public static int ToInt(this bool _bool) => _bool ? 1 : 0;

        public static void Log(string message, bool newline = true, ConsoleColor _color = ConsoleColor.White)
        {
            Console.ForegroundColor = _color;
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

        /// <summary>
        /// Executes the given action and returns the amount of milliseconds it took to run.
        /// </summary>
        /// <param name="_action">action to execute and time the duration</param>
        /// <returns>milliseconds in duration for the given action</returns>
        public static long MillisecondsDuration(Action _action)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            _action();

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
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
        /// <returns>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        public static float RandomFloat() => (float)random.NextDouble();

        public static int RandomInt() => random.Next();

        /// <param name="_max">exclusive upper bound on the returned random integer</param>
        /// <returns>Returns and integer which is greater than or equal to zero and less than _max</returns>
        public static int RandomInt(int _max) => random.Next(0, _max);

        /// <param name="_min">inclusive lower bound on the returned random integer</param>
        /// <param name="_max">exclusive upper bound on the returned random integer</param>
        /// <returns>Returns an integer that is greater than or equal to _min and less than _max</returns>
        public static int RandomInt(int _min, int _max) => random.Next(_min, _max);
        public static int RandomSign() => RandomInt() % 2 == 0 ? 1 : -1;
        public static double RandomAngleRad() => RandomDouble() * Math.PI * 2;
        public static double RandomAngleDeg() => RandomDouble() * 360;

        public static (float X, float Y) Rotate(this (float X, float Y) _position, float _radians, float _centerX = 0, float _centerY = 0)
            => Rotate(_position.X, _position.Y, _radians, _centerX, _centerY);
        public static (float X, float Y) Rotate(float _vertexX, float _vertexY, float _radians, float _centerX = 0, float _centerY = 0)
        {
            var diffX = _vertexX - _centerX;
            var diffY = _vertexY - _centerY;
            var cos = Math.Cos(_radians);
            var sin = Math.Sin(_radians);
            return ((float)(diffX * cos - diffY * sin) + _centerX,
                    (float)(diffY * cos + diffX * sin) + _centerY);
        }

        public static IEnumerable<(float X, float Y)> Rotated(this IEnumerable<(float X, float Y)> _list, float _radians, float _centerX = 0, float _centerY = 0) 
            => _radians == 0
                ? _list
                : _list.Select(o => o.Rotate(_radians, _centerX, _centerY));

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue val)
        { //http://alexslover.com/Clean-foreach-loop-in-dictionaries/
            key = kvp.Key;
            val = kvp.Value;
        }

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

        public static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();

        public static void Swap<T>(ref T x, ref T y)
        {
            var t = y;
            y = x;
            x = t;
        }
    }
}
