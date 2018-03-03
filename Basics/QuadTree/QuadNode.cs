using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Basics.QuadTree
{
    public class QuadNode<T>
    {
        private Rectangle rectangle;
        public float X => rectangle.X;
        public float Y => rectangle.Y;
        public float W => rectangle.W;
        public float H => rectangle.H;

        public QuadNode(float _x, float _y, float _width, float _height, QuadTree<T> _tree) 
        {
            init(_x, _y, _width, _height, _tree);
        }

        private static Queue<QuadNode<T>> pool = new Queue<QuadNode<T>>();

        private QuadTree<T> tree;
        private HashSet<int> objectIndices;
        private List<QuadNode<T>> children;
        private bool hasChildren => children?.Count > 0;
        
        /// <summary>
        /// Initializes the variables of this node.
        /// </summary>
        /// <param name="_x">x-coordinate of this node (corner)</param>
        /// <param name="_y">y-coordinate of this node (corner)</param>
        /// <param name="_width">width of this node.</param>
        /// <param name="_height">height of this node.</param>
        private void init(float _x, float _y, float _width, float _height, QuadTree<T> _tree)
        {
            rectangle = new Rectangle(_x, _y, _width, _height);
            tree = _tree;
            objectIndices = new HashSet<int>();
            children = null;
        }

        /// <summary>
        /// Returns a pooled node. Use this instead of "new Node()"
        /// </summary>
        /// <param name="_tree">tree this node belongs to.</param>
        /// <param name="_x">x-coordinate of this node (corner)</param>
        /// <param name="_y">y-coordinate of this node (corner)</param>
        /// <param name="_width">width of this node.</param>
        /// <param name="_height">height of this node.</param>
        /// <returns>A node from the pool (or a new one if the pool is empty)</returns>
        private static QuadNode<T> getNode(QuadTree<T> _tree, float _x, float _y, float _width, float _height)
        {
            if (pool.Count <= 0)
                return new QuadNode<T>(_x, _y, _width, _height, _tree);

            var n = pool.Dequeue();
            n.init(_x, _y, _width, _height, _tree);
            return n;
        }
        
        /// <summary>
        /// Puts this node and its children into the pool. (Does NOT remove from the QuadTree).
        /// </summary>
        public void Destroy()
        {
            pool.Enqueue(this);
            if (hasChildren)
                children.ForEach(o => o.Destroy());
        }
        
        /// <summary>
        /// Splits this node into 4 nodes of equal size. Also passes object indices down to the new children.
        /// </summary>
        private void split()
        {
            var w2 = rectangle.W / 2f;
            var h2 = rectangle.H / 2f;
            children = new List<QuadNode<T>>
            {
                getNode(tree, rectangle.X, rectangle.Y, w2, h2),
                getNode(tree, rectangle.X + w2, rectangle.Y, w2, h2),
                getNode(tree, rectangle.X, rectangle.Y + h2, w2, h2),
                getNode(tree, rectangle.X + w2, rectangle.Y + h2, w2, h2),
            };
            foreach (var objectIndex in objectIndices)
            {
                var obj = tree.Objects[objectIndex];
                var rect = tree.Rectangles[objectIndex];
                foreach (var child in children)
                    child.Insert(obj, rect);
            }
            objectIndices = null;
        }
        
        /// <summary>
        /// Adds the object to this node (or its children, should it split) if it is within this node's bounds.
        /// </summary>
        /// <param name="_o">object to add</param>
        /// <param name="_r">rectangle to use for collisions with the object</param>
        public void Insert(T _o, Rectangle _r)
        {
            if (!rectangle.Collides(_r))
                return;

            if (hasChildren)
                children.ForEach(c => c.Insert(_o, _r));
            else
            {
                var index = tree.GetIndex(_o, _r);
                objectIndices.Add(index);
                if (objectIndices.Count > tree.MaxObjectsPerNode && rectangle.W / 2 >= tree.MinNodeSideLength)
                    split();
            }
        }
        
        /// <summary>
        /// Gathers up all unique object indexes for objects that collide with the given point
        /// </summary>
        /// <param name="_p">point that is being checked for collisions</param>
        /// <param name="_v">list of object indices to add to</param>
        public void QueryPoint(Vector2 _p, HashSet<int> _v) => QueryPoint(_p.X, _p.Y, _v);
        public void QueryPoint(float _x, float _y, HashSet<int> _v)
        {
            if (!rectangle.Collides(_x, _y))
                return;

            if (!hasChildren)
                _v.AddRange(objectIndices);
            else
                foreach(var child in children)
                    child.QueryPoint(_x, _y, _v);
        }
        
        /// <summary>
        /// Gathers up all unique object indexes for objects that collide with the given rectangle
        /// </summary>
        /// <param name="_r">rectangle that is being checked for collisions</param>
        /// <param name="_v">list of object indices to add to</param>
        public void QueryRect(Rectangle _r, HashSet<int> _v) => QueryRect(_r.X, _r.Y, _r.W, _r.H, _v);
        public void QueryRect(float _x, float _y, float _w, float _h, HashSet<int> _v)
        {
            if (!rectangle.Collides(_x, _y, _w, _h))
                return;

            if (!hasChildren)
                _v.AddRange(objectIndices);
            else
                foreach(var child in children)
                    child.QueryRect(_x, _y, _w, _h, _v);
        }

        public HashSet<Rectangle> GetRectangles()
        {
            var result = new HashSet<Rectangle>();
            GetRectangles_Helper(result);
            return result;
        }
        private void GetRectangles_Helper(HashSet<Rectangle> _rectangles)
        {
            if (hasChildren)
                children.ForEach(o => o.GetRectangles_Helper(_rectangles));
            else
                _rectangles.Add(rectangle);
        }
    }
}