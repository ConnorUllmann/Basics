using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basics
{
    public class BinarySearchTree<T>
    {
        internal Node<T> root;

        public BinarySearchTree()
        {

        }

        public bool Lookup(int key)
        {
            var result = root.Lookup(key);
            root = root.Root();
            return result;
        }

        public void Insert(int key, T value)
        {
            if (root == null)
                root = new Node<T>(key, value);
            else
            {
                root.Insert(key, value);
                root = root.Root();
            }
        }

        public void Delete(int key)
        {
            root.Delete(key);
            root = root.Root();
        }
    }

    internal class Node<T>
    {
        public int key;
        public T value;
        public Node<T> parent;
        public Node<T> left;
        public Node<T> right;

        public Node(int key, T value)
        {
            this.key = key;
            this.value = value;
            this.parent = null;
            this.left = null;
            this.right = null;
        }

        public bool Lookup(int key)
        {
            if(key < this.key)
            {
                if (left == null)
                    return false;
                return left.Lookup(key);
            }
            else if(key > this.key)
            {
                if (right == null)
                    return false;
                return right.Lookup(key);
            }
            return true;
        }

        public Node<T> Insert(int key, T value)
        {
            if (key < this.key)
            {
                if (left == null)
                {
                    left = new Node<T>(key, value);
                    left.parent = this;
                    return left;
                }
                else
                    return left.Insert(key, value);
            }
            else if(key > this.key)
            {
                if (right == null)
                {
                    right = new Node<T>(key, value);
                    right.parent = this;
                    return right;
                }
                else
                    return right.Insert(key, value);
            }
            return null;
        }

        public void ReplaceSelfWithNode(Node<T> node)
        {
            if (parent != null)
            {
                if (parent.left == this)
                    parent.left = node;
                else if (parent.right == this)
                    parent.right = node;
            }
            if (node != null)
                node.parent = parent;
        }

        public void Delete(int key)
        {
            if (key < this.key)
            {
                if (left == null)
                    return;
                left.Delete(key);
            }
            else if (key > this.key)
            {
                if (right == null)
                    return;
                right.Delete(key);
            }
            else
            {
                if (left == null && right == null)
                {
                    ReplaceSelfWithNode(null);
                }
                else if (left != null && right != null)
                {
                    var predecessor = Predecessor();
                    this.key = predecessor.key;
                    this.value = predecessor.value;
                    predecessor.Delete(predecessor.key);
                }
                else if (left != null)
                {
                    ReplaceSelfWithNode(left);
                }
                else
                {
                    ReplaceSelfWithNode(right);
                }
            }
        }

        public void RotateLeft()
        {
            ReplaceSelfWithNode(right);
            right.left = this;
            parent = right;
            left = null;
            right = null;
        }

        public void RotateRight()
        {
            ReplaceSelfWithNode(left);
            left.right = this;
            parent = left;
            left = null;
            right = null;
        }

        public Node<T> Predecessor()
        {
            if (left == null)
                return null;
            return left.Rightmost();
        }

        public Node<T> Successor()
        {
            if (right == null)
                return null;
            return right.Leftmost();
        }

        public Node<T> Leftmost()
        {
            var current = this;
            while (true)
            {
                if (current.left == null)
                    return current;
                current = current.left;
            }
        }

        public Node<T> Rightmost()
        {
            var current = this;
            while (true)
            {
                if (current.right == null)
                    return current;
                current = current.right;
            }
        }

        public Node<T> Root()
        {
            var current = this;
            while(true)
            {
                if (current.parent == null)
                    return current;
                current = current.parent;
            }
        }

        public void Traverse(Action<Node<T>> callback)
        {
            if (left != null)
                left.Traverse(callback);
            callback(this);
            if (right != null)
                right.Traverse(callback);
        }

        public new string ToString()
        {
            var values = new List<int>();
            Traverse((node) => { values.Add(node.key); });
            return string.Join(", ", values.ToArray());
        }
    }
}
