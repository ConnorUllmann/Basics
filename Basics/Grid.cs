using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime;

namespace Basics
{
    public class Grid<T>
    {
        public readonly int Width;
        public readonly int Height;
        protected List<List<T>> objects;

        public Grid(int _width, int _height)
        {
            Width = _width;
            Height = _height;

            objects = new List<List<T>>();
            for (int x = 0; x < Width; x++)
            {
                objects.Add(new List<T>());
                for (int y = 0; y < Height; y++)
                    objects[x].Add(default(T));
            }
        }
        public Grid(List<List<T>> _objects)
        {
            if (_objects == null)
                throw new ArgumentException("Two-dimensional list of objects is null");
            if (_objects.Count <= 0 || !_objects.TrueForAll(x => x.Count > 0))
                throw new ArgumentException("Invalid dimensions for two-dimensional list of objects. Cannot create Grid.");
            Width = _objects.Count;
            Height = _objects[0].Count;
            objects = _objects;
        }

        public void Clear()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    objects[x][y] = default(T);
        }

        public bool Inside(int _x, int _y) => Inside((float)_x, _y);
        public bool Inside(float _x, float _y) => _x >= 0 && _x < Width && _y >= 0 && _y < Height;

        public void Set(T o, float _x, float _y) => Set(o, (int)Math.Floor(_x), (int)Math.Floor(_y));
        public void Set(T o, int _x, int _y)
        {
            if(Inside(_x, _y))
                objects[_x][_y] = o;
        }
        public void Set(T o, float _x, float _y, float _w, float _h)
        {
            int x_s = Utils.Clamp((int)Math.Floor(_x), 0, Width - 1);
            int y_s = Utils.Clamp((int)Math.Floor(_y), 0, Height - 1);
            int x_f = Utils.Clamp((int)Math.Floor(_x + _w), 0, Width - 1);
            int y_f = Utils.Clamp((int)Math.Floor(_y + _h), 0, Height - 1);
            for (int x = x_s; x <= x_f; x++)
                for (int y = y_s; y <= y_f; y++)
                    objects[x][y] = o;
        }

        public T Get(float _x, float _y) => Get((int)Math.Floor(_x), (int)Math.Floor(_y));
        public T Get(int _x, int _y)
        {
            if (!Inside(_x, _y))
                return default(T);
            return objects[_x][_y];
        }
        public List<T> Get(float _x, float _y, float _w, float _h)
        {
            if (_w <= 0 || _h <= 0)
                return new List<T>();

            int x_s = (int)Math.Floor(_x);
            int y_s = (int)Math.Floor(_y);
            int x_f = (int)Math.Floor(_x + _w);
            int y_f = (int)Math.Floor(_y + _h);

            if (x_f < 0 || y_f < 0 || x_s >= Width || y_s >= Height || x_s > x_f || y_s > y_f)
                return new List<T>();

            var ret = new List<T>();
            for (int x = x_s; x <= x_f; x++)
                for (int y = y_s; y <= y_f; y++)
                    if(!ret.Contains(objects[x][y]))
                        ret.Add(objects[x][y]);
            return ret;
        }
        
        public virtual List<T> GetNeighborsCardinal(int _x, int _y)
        {
            var neighbors = new List<T>();
            if (Inside(_x - 1, _y)) { var o = Get(_x - 1, _y); if (o != null) neighbors.Add(o); }
            if (Inside(_x + 1, _y)) { var o = Get(_x + 1, _y); if (o != null) neighbors.Add(o); }
            if (Inside(_x, _y - 1)) { var o = Get(_x, _y - 1); if (o != null) neighbors.Add(o); }
            if (Inside(_x, _y + 1)) { var o = Get(_x, _y + 1); if (o != null) neighbors.Add(o); }
            return neighbors;
        }
        public virtual List<T> GetNeighborsSquare(int _x, int _y)
        {
            var neighbors = new List<T>();
            for (var x = _x - 1; x <= _x + 1; x++)
                for (var y = _y - 1; y <= _y + 1; y++)
                    if ((x != _x || y != _y) && Inside(x, y)) { var o = Get(x, y); if (o != null) neighbors.Add(o); }
            return neighbors;
        }

        public void ForEachXY(Action<T> action) => ForEachX(x => x.ForEach(xy => action(xy)));
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