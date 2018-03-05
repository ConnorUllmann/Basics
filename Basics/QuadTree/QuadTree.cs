using System.Numerics;
using System.Collections;
using System.Collections.Generic;

namespace Basics.QuadTree
{
    public class QuadTree<T>
    {
        public int MinNodeSideLength;
        public int MaxObjectsPerNode;

        private static int IndexCounter = 0;
        public Dictionary<int, T> Objects;
        public Dictionary<T, int> Indices;
        public Dictionary<int, Rectangle> Rectangles;
        private QuadNode<T> root;


        /// <summary>
        /// Instantiates a QuadTree instance
        /// </summary>
        /// <param name="_width">total width of the quadtree</param>
        /// <param name="_height">total height of the quadtree</param>
        /// <param name="_maxObjectsPerNode">number of objects a node can hold before it splits</param>
        /// <param name="_minNodeSideLength">minimum side-length of a node that can still split</param>
        public QuadTree(float _width, float _height, int _maxObjectsPerNode, int _minNodeSideLength)
            : this(0, 0, _width, _height, _maxObjectsPerNode, _minNodeSideLength) { }

        /// <summary>
        /// Instantiates a QuadTree instance
        /// </summary>
        /// <param name="_x">x-position of the quadtree</param>
        /// <param name="_y">y-position of the quadtree</param>
        /// <param name="_width">total width of the quadtree</param>
        /// <param name="_height">total height of the quadtree</param>
        /// <param name="_maxObjectsPerNode">number of objects a node can hold before it splits</param>
        /// <param name="_minNodeSideLength">minimum side-length of a node that can still split</param>
        public QuadTree(float _x, float _y, float _width, float _height, int _maxObjectsPerNode=1, int _minNodeSideLength=10)
        {
            MinNodeSideLength = _minNodeSideLength;
            MaxObjectsPerNode = _maxObjectsPerNode;
            root = new QuadNode<T>(_x, _y, _width, _height, this);
        }
        
        /// <summary>
        /// Reinitializes the tree, removing all nodes, children, rectangles, objects, indices, and objectIndices.
        /// </summary>
        public void Reset()
        {
            removeAllChildren();
            removeAllObjects();
        }

        private void removeAllObjects()
        {
            IndexCounter = 0;
            Indices = new Dictionary<T, int>();
            Objects = new Dictionary<int, T>();
            Rectangles = new Dictionary<int, Rectangle>();
        }

        private void removeAllChildren()
        {
            root.Destroy();
            root = new QuadNode<T>(root.X, root.Y, root.W, root.H, this);
        }
        
        /// <summary>
        /// Inserts the given object into the quadtree
        /// </summary>
        /// <param name="_o">object to add to the tree</param>
        /// <param name="_r">rectangle to use for collisions with the object</param>
        public void Insert(T _o, Rectangle _r) => root.Insert(_o, _r);
        
        /// <summary>
        /// Acquires an index for the given object and adds it to the tree; if the object is already there its index is returned.
        /// </summary>
        /// <param name="_o">object for which an index is needed</param>
        /// <param name="_r">rectangle to use for collisions with the object</param>
        /// <returns></returns>
        public int GetIndex(T _o, Rectangle _r) => GetIndex(_o, _r.X, _r.Y, _r.W, _r.H);
        public int GetIndex(T _o, float _x, float _y, float _w=0, float _h=0)
        {
            if (Indices.TryGetValue(_o, out var index))
                return index;
            index = Indices[_o] = IndexCounter++;
            Objects[index] = _o;
            Rectangles[index] = new Rectangle(_x, _y, _w, _h); //Have to create a new one so they're immutable
            return index;
        }

        private HashSet<T> objectsFromIndices(IEnumerable<int> _indices)
        {
            var objects = new HashSet<T>();
            foreach (var index in _indices)
                objects.Add(Objects[index]);
            return objects;
        }

        /// <summary>
        /// Queries the quadtree for collisions against a point
        /// </summary>
        /// <param name="_p">The point being tested for collisions</param>
        /// <returns>set of objects which could potentially collide with the given point</returns>
        public HashSet<T> QueryPoint(float _x, float _y)
        {
            var indices = new HashSet<int>();
            root.QueryPoint(_x, _y, indices);
            return objectsFromIndices(indices);
        }

        /// <summary>
        /// Queries the quadtree for collisions against a rectangle
        /// </summary>
        /// <param name="_r">The rectangle being tested for collisions</param>
        /// <returns>set objects which could potentially collide with the given rectangle</returns>
        public HashSet<T> QueryRect(Rectangle _r) => QueryRect(_r.X, _r.Y, _r.W, _r.H);
        public HashSet<T> QueryRect(float _x, float _y, float _w, float _h)
        {
            var indices = new HashSet<int>();
            root.QueryRect(_x, _y, _w, _h, indices);
            return objectsFromIndices(indices);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HashSet<Rectangle> GetRectangles() => root.GetRectangles();
    }
}