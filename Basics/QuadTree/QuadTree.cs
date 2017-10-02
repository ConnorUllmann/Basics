using System.Numerics;
using System.Collections;
using System.Collections.Generic;

namespace Basics.QuadTree
{
    public class QuadTree<T> : QuadNode<T>
    {
        private static int IndexCounter = 0;
        public Dictionary<int, T> objects;
        public Dictionary<T, int> indices;
        public Dictionary<int, Vector2> positions;

        public QuadTree(float _width, float _height) : base(0, 0, _width, _height) { }
        public QuadTree(float _x, float _y, float _width, float _height) : base(_x, _y, _width, _height) { }

        /**
         * Reinitializes the tree, removing all nodes, children, objects, indices, and objectIndices.
         */
        public void Reset()
        {
            IndexCounter = 0;

            objects = new Dictionary<int, T>();
            indices = new Dictionary<T, int>();
            positions = new Dictionary<int, Vector2>();
            if (objectIndices != null)
            {
                objectIndices.Clear();
            }
            if (!isLeaf)
            {
                while (children.Count > 0)
                {
                    int ind = children.Count - 1;
                    children[ind].addToPool();
                    children.RemoveAt(ind);
                }
            }
        }

        /**
            * Removes _o from the tree.
            * @param	_o		the object to remove from the tree.
            */
        /*public void removeObject(GameObject _o)
        {
            objects.Remove(indices[_o]);
            indices.Remove(_o);
        }*/

        /**
            * Acquires an index for the given object and adds it to the tree; if the object is already there its index is returned.
            * @param	_o		the object to add to the tree.
            * @return returns the index of the object within the "objects" list.
            */
        public int addObject(T _o, float _x, float _y)
        {
            if (indices.ContainsKey(_o))
                return indices[_o];
            indices[_o] = IndexCounter++;
            objects[indices[_o]] = _o;
            positions[indices[_o]] = new Vector2(_x, _y);
            return indices[_o];
        }


        public List<T> QueryPoint(Vector2 p)
        {
            var t_v = new List<int>();
            queryPoint(p, ref t_v);
            var t_vr = new List<T>();
            for (int i = 0; i < t_v.Count; i++)
                t_vr.Add(objects[t_v[i]]);
            return t_vr;
        }
        public List<T> QueryRect(float x, float y, float w, float h)
        {
            var t_v = new List<int>();
            queryRect(x, y, w, h, ref t_v);
            var unique = new Dictionary<int, bool>();
            var t_vr = new List<T>();
            for (int i = 0; i < t_v.Count; i++)
            {
                if (unique.ContainsKey(t_v[i]))
                    continue;
                unique[t_v[i]] = true;
                t_vr.Add(objects[t_v[i]]);
            }
            return t_vr;
        }

    }
}