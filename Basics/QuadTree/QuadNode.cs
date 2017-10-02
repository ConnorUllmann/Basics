using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Basics.QuadTree
{
    public class QuadNode<T>
    {

        public QuadNode(float _x, float _y, float _width, float _height, QuadTree<T> _tree = null)
        {
            init(_x, _y, _width, _height, (_tree == null && this is QuadTree<T> ? this as QuadTree<T> : _tree));
        }

        protected const int MIN_NODE_DIMENSION = 2;
        protected const int MAX_OBJECTS_BEFORE_SPLIT = 5;
        public static Queue<QuadNode<T>> pool = new Queue<QuadNode<T>>();

        protected List<int> objectIndices;
        protected List<QuadNode<T>> children;

        private float x;
        private float y;
        private float width;
        private float height;
        protected QuadTree<T> tree;

        /**
            * Returns true if this node is a leaf node (has no children and holds object indices).
            */
        protected bool isLeaf { get { return children == null || children.Count <= 0; } }

        /**
            * Initializes the variables of this node.
            * @param	_tree		the tree this node belongs to.
            * @param	_x			the x-coordinate of this node. (upper-left corner).
            * @param	_y			the y-coordinate of this node. (upper-left corner)
.		* @param	_width		the width of this node.
            * @param	_height		the height of this node.
            */
        protected void init(float _x, float _y, float _width, float _height, QuadTree<T> _tree)
        {
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            tree = _tree;
            objectIndices = null;
            children = null;
        }

        /**
            * Returns a pooled node. Use this instead of "new Node()"
            * @param	_tree		the tree this node belongs to.
            * @param	_x			the x-coordinate of this node. (upper-left corner)
            * @param	_y			the y-coordinate of this node. (upper-left corner)
            * @param	_width		the width of this node.
            * @param	_height		the height of this node.
            * @return returns a node from the pool (or created new if the pool is empty).
            */
        protected static QuadNode<T> getNode(QuadTree<T> _tree, float _x, float _y, float _width, float _height)
        {
            if (pool.Count <= 0)
                return new QuadNode<T>(_x, _y, _width, _height, _tree);

            var n = pool.Dequeue();
            n.init(_x, _y, _width, _height, _tree);
            return n;
        }

        /**
            * Puts this node into the pool. (Does NOT remove from the QuadTree).
            */
        public void addToPool()
        {
            pool.Enqueue(this);
        }

        /**
        * Splits this node into 4 nodes of equal size. Also passes held indices down to the children.
        */
        protected void split()
        {
            var w2 = width / 2f;
            var h2 = height / 2f;
            children = new List<QuadNode<T>>();
            children.Add(getNode(tree, x, y, w2, h2));
            children.Add(getNode(tree, x + w2, y, w2, h2));
            children.Add(getNode(tree, x, y + h2, w2, h2));
            children.Add(getNode(tree, x + w2, y + h2, w2, h2));
            if (objectIndices != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    for (int j = 0; j < objectIndices.Count; j++)
                    {
                        var k = objectIndices[j];
                        children[i].Insert(tree.objects[k], tree.positions[k].X, tree.positions[k].Y);
                    }
                }
                objectIndices = null;
            }
        }

        protected bool collidesPoint(Vector2 p)
        {
            return p.X >= x &&
                   p.Y >= y &&
                   p.X < x + width &&
                   p.Y < y + height;
        }
        protected bool collidesRect(float tx, float ty, float tw, float th)
        {
            return x + width > tx &&
                   y + height > ty &&
                   x < tx + tw &&
                   y < ty + th;
        }
        protected bool insideRect(float tx, float ty, float tw, float th)
        {
            return x >= tx &&
                   y >= ty &&
                   x + width < tx + tw &&
                   y + height < ty + th;
        }

        /**
            * Adds the object _o to this node (or its children, should it split) if it is within this node's bounds.
            * @param	_o		object to add.
            */
        public void Insert(T _o, float _x, float _y)
        {
            if (collidesPoint(new Vector2(_x, _y)))
            {
                if (!isLeaf)
                {
                    for (int i = 0; i < children.Count; i++)
                    {
                        children[i].Insert(_o, _x, _y);
                    }
                }
                else
                {
                    var t = tree.addObject(_o, _x, _y);
                    if (objectIndices == null)
                        objectIndices = new List<int>();
                    objectIndices.Add(t);
                    if (objectIndices.Count > MAX_OBJECTS_BEFORE_SPLIT && width / 2 >= MIN_NODE_DIMENSION)
                    {
                        split();
                    }
                }
            }
        }

        /**
            * Gathers up all unique object indexes for objects that collide with object _o's queryRectangle.
            * @param	_o		object that is checking collisions.
            * @param	_v		list of indices to add to.
            */
        public void queryPoint(Vector2 p, ref List<int> _v)
        {
            if (collidesPoint(p))
            {
                if (isLeaf)
                {
                    if (objectIndices != null)
                    {
                        for (int i = 0; i < objectIndices.Count; i++)
                            _v.Add(objectIndices[i]); //pushUniqueSorted(objectIndices[i], ref _v);
                    }
                }
                else
                {
                    for (int i = 0; i < children.Count; i++)
                        children[i].queryPoint(p, ref _v);
                }
            }
        }
        public void queryRect(float x, float y, float w, float h, ref List<int> _v)
        {
            if (collidesRect(x, y, w, h))
            {
                if (isLeaf)
                {
                    if (objectIndices != null)
                    {
                        for (int i = 0; i < objectIndices.Count; i++)
                            //if(!_v.Contains(objectIndices[i])) //Unique! Not necessarily important
                            _v.Add(objectIndices[i]);//pushUniqueSorted(objectIndices[i], ref _v);
                    }
                }
                else if (insideRect(x, y, w, h))
                {
                    GetAllObjectsInsideNode(ref _v);
                }
                else
                {
                    for (int i = 0; i < children.Count; i++)
                        children[i].queryRect(x, y, w, h, ref _v);
                }
            }
        }

        public void GetAllObjectsInsideNode(ref List<int> _v)
        {
            if (isLeaf)
            {
                if (objectIndices != null)
                {
                    for (int i = 0; i < objectIndices.Count; i++)
                        //if(!_v.Contains(objectIndices[i])) //Unique! Not necessarily important
                        _v.Add(objectIndices[i]);
                }
            }
            else
            {
                for (int i = 0; i < children.Count; i++)
                    children[i].GetAllObjectsInsideNode(ref _v);
            }
        }
    }
}