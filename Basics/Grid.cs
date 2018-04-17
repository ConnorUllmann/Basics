using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Basics
{
    public class Grid<T> : IPosition
    {
        private readonly Rectangle rectangle;

        public virtual float X => 0;
        public virtual float Y => 0;
        public int Width => (int)rectangle.W;
        public int Height => (int)rectangle.H;
        protected List<List<T>> objects;

        public Grid(int _width, int _height) : this(_width, _height, (int x, int y) => default(T)) {}
        public Grid(int _width, int _height, Func<int,int,T> _tileGenerator)
        {
            rectangle = new Rectangle(X, Y, _width, _height);
            objects = new List<List<T>>();
            for (int x = 0; x < Width; x++)
            {
                objects.Add(new List<T>());
                for (int y = 0; y < Height; y++)
                    objects[x].Add(_tileGenerator.Invoke(x, y));
            }
        }
        public Grid(List<List<T>> _objects)
        {
            if (_objects == null)
                throw new ArgumentException("Two-dimensional list of objects is null");
            if (_objects.Count <= 0 || !_objects.TrueForAll(x => x.Count > 0) || !_objects.TrueForAll(x => x.Count == _objects[0].Count))
                throw new ArgumentException("Invalid dimensions for two-dimensional list of objects. Cannot create Grid.");
            rectangle = new Rectangle(X, Y, _objects.Count, _objects[0].Count);
            objects = _objects;
        }

        public void Clear()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    objects[x][y] = default;
        }

        public bool Inside(IPosition _position) => Inside(_position.X, _position.Y);
        public bool Inside(int _x, int _y) => Inside((float)_x, _y);
        public bool Inside(float _x, float _y) => _x >= 0 && _x < Width && _y >= 0 && _y < Height;

        public void Set(T o, IPosition _position) => Set(o, _position.X, _position.Y);
        public void Set(T o, float _x, float _y) => Set(o, (int)_x, (int)_y);
        public void Set(T o, int _x, int _y)
        {
            if(Inside(_x, _y))
                objects[_x][_y] = o;
        }
        public void Set(T o, Rectangle _r) => Set(o, _r.X, _r.Y, _r.W, _r.H);
        public void Set(T o, float _x, float _y, float _w, float _h)
        {
            if (!rectangle.Collides(_x, _y, _w, _h))
                return;
            int x_s = Utils.Clamp((int)Math.Floor(_x), 0, Width - 1);
            int y_s = Utils.Clamp((int)Math.Floor(_y), 0, Height - 1);
            int x_f = Utils.Clamp((int)Math.Floor(_x + _w), 1, Width);
            int y_f = Utils.Clamp((int)Math.Floor(_y + _h), 1, Height);
            for (int x = x_s; x < x_f; x++)
                for (int y = y_s; y < y_f; y++)
                    objects[x][y] = o;
        }

        public T Get(IPosition _position) => Get(_position.X, _position.Y);
        public T Get(float _x, float _y) => Get((int)_x, (int)_y);
        public T Get(int _x, int _y)
        {
            if (!Inside(_x, _y))
                return default(T);
            return objects[_x][_y];
        }
        public List<T> Get(Rectangle _r) => Get(_r.X, _r.Y, _r.W, _r.H);
        public List<T> Get(float _x, float _y, float _w, float _h)
        {
            if (_w <= 0 || _h <= 0)
                return new List<T>();
            if (!rectangle.Collides(_x, _y, _w, _h))
                return new List<T>();

            int x_s = Utils.Clamp((int)Math.Floor(_x), 0, Width-1);
            int y_s = Utils.Clamp((int)Math.Floor(_y), 0, Height-1);
            int x_f = Utils.Clamp((int)Math.Floor(_x + _w), 1, Width);
            int y_f = Utils.Clamp((int)Math.Floor(_y + _h), 1, Height);
            if (x_s > x_f || y_s > y_f)
                return new List<T>();

            var ret = new List<T>();
            for (int x = x_s; x < x_f; x++)
                for (int y = y_s; y < y_f; y++)
                    ret.Add(objects[x][y]);
            return ret;
        }

        public List<T> GetNeighborsCardinal(IPosition _position) => GetNeighborsCardinal(_position.X, _position.Y);
        public List<T> GetNeighborsCardinal(float _x, float _y) => GetNeighborsCardinal((int)_x, (int)_y);
        public virtual List<T> GetNeighborsCardinal(int _x, int _y)
        {
            var neighbors = new List<T>();
            if (Inside(_x - 1, _y)) { var o = Get(_x - 1, _y); if (o != null) neighbors.Add(o); }
            if (Inside(_x + 1, _y)) { var o = Get(_x + 1, _y); if (o != null) neighbors.Add(o); }
            if (Inside(_x, _y - 1)) { var o = Get(_x, _y - 1); if (o != null) neighbors.Add(o); }
            if (Inside(_x, _y + 1)) { var o = Get(_x, _y + 1); if (o != null) neighbors.Add(o); }
            return neighbors;
        }

        public List<T> GetNeighborsSquare(IPosition _position) => GetNeighborsSquare(_position.X, _position.Y);
        public List<T> GetNeighborsSquare(float _x, float _y) => GetNeighborsSquare((int)_x, (int)_y);
        public virtual List<T> GetNeighborsSquare(int _x, int _y)
        {
            var neighbors = new List<T>();
            for (var x = _x - 1; x <= _x + 1; x++)
                for (var y = _y - 1; y <= _y + 1; y++)
                    if ((x != _x || y != _y) && Inside(x, y)) { var o = Get(x, y); if (o != null) neighbors.Add(o); }
            return neighbors;
        }

        public virtual List<(T Object, (int X, int Y) Position)> GetNeighborsAndPositionsCardinal(int _x, int _y)
        {
            var neighbors = new List<(T, (int X, int Y))>();
            if (Inside(_x - 1, _y)) { var o = Get(_x - 1, _y); if (o != null) neighbors.Add((o, (_x - 1, _y))); }
            if (Inside(_x + 1, _y)) { var o = Get(_x + 1, _y); if (o != null) neighbors.Add((o, (_x + 1, _y))); }
            if (Inside(_x, _y - 1)) { var o = Get(_x, _y - 1); if (o != null) neighbors.Add((o, (_x, _y - 1))); }
            if (Inside(_x, _y + 1)) { var o = Get(_x, _y + 1); if (o != null) neighbors.Add((o, (_x, _y + 1))); }
            return neighbors;
        }

        public virtual List<(T Object, (int X, int Y) Position)> GetNeighborsAndPositionsSquare(int _x, int _y)
        {
            var neighbors = new List<(T, (int X, int Y))>();
            for (var x = _x - 1; x <= _x + 1; x++)
                for (var y = _y - 1; y <= _y + 1; y++)
                    if ((x != _x || y != _y) && Inside(x, y)) { var o = Get(x, y); if (o != null) neighbors.Add((o, (x, y))); }
            return neighbors;
        }

        /// <summary>
        /// Finds all surrounding grid cells which match a fillCondition starting at a given point.
        /// Note: the result set will contain the cell at the point (_x, _y) on the grid if it matches the condition.
        /// </summary>
        /// <param name="_fillCondition">condition for which all returned cells must return true</param>
        /// <param name="_x">starting x-position of flood fill</param>
        /// <param name="_y">starting y-position of flood fill</param>
        /// <returns>the neighbors of the grid position (_x, _y) which match the fill condition</returns>
        public HashSet<T> GetFloodFill(Func<T, bool> _fillCondition, int _x, int _y)
        {
            //TODO: Use scanline algorithm
            var set = new HashSet<T>();

            var firstTile = Get(_x, _y);
            if (firstTile == null || !_fillCondition(firstTile))
                return set;

            var all = new HashSet<(int X, int Y)>();
            var next = new Queue<(int X, int Y)>();

            set.Add(firstTile);
            next.Enqueue((_x, _y));
            all.Add((_x, _y));
            while (next.Count > 0)
            {
                var position = next.Dequeue();
                GetNeighborsAndPositionsCardinal(position.X, position.Y).ForEach(o =>
                {
                    if (!_fillCondition(o.Object) || all.Contains(o.Position))
                        return;
                    set.Add(o.Object);
                    next.Enqueue(o.Position);
                    all.Add(o.Position);
                });
            }
            return set;
        }
        
        /// <returns>an IEnumerable containing all objects in the grid.</returns>
        public IEnumerable<T> All()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    yield return Get(x, y);
        }

        /// <summary>
        /// Use ForEachXY instead of ForEachYX if possible; it is faster.
        /// </summary>
        /// <param name="action">action to execute for each object in the grid</param>
        public void ForEachXY(Action<T> action) => ForEachX(x => x.ForEach(xy => action(xy)));
        /// <summary>
        /// Use ForEachXY instead of ForEachYX if possible; it is faster.
        /// </summary>
        /// <param name="action">action to execute for each object in the grid</param>
        public void ForEachYX(Action<T> action) => ForEachY(x => x.ForEach(xy => action(xy)));
        public void ForEachX(Action<List<T>> action) => objects.ForEach(x => action(x));
        public void ForEachY(Action<List<T>> action)
        {
            for (var j = 0; j < Height; j++)
            {
                var list = new List<T>();
                for (var i = 0; i < Width; i++)
                    list.Add(objects[i][j]);
                action(list);
            }
        }
    }
}