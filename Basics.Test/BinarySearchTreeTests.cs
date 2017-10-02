using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Basics;

namespace Basics.Test
{
    public class BinarySearchTreeTests
    {
        [Fact]
        public void BinarySearchTree_Insert()
        {
            var tree = new BinarySearchTree<int>();
            tree.Insert(8, 8);
            tree.Insert(1, 1);
            tree.Insert(3, 3);
            tree.Insert(5, 5);

            var root = tree.root.left;
            Assert.Equal(8, root.Root().value);
            Assert.Equal(1, root.value);
            Assert.Equal(3, root.right.value);
            Assert.Equal(5, root.right.right.value);
        }

        [Fact]
        public void BinarySearchTree_RotateLeft()
        {
            var tree = new BinarySearchTree<int>();
            tree.Insert(8, 8);
            tree.Insert(1, 1);
            tree.Insert(3, 3);
            tree.Insert(5, 5);

            var root = tree.root.left;
            root.RotateLeft();
            root = tree.root.left;
            Assert.Equal(8, root.Root().value);
            Assert.Equal(3, root.value);
            Assert.Equal(1, root.left.value);
            Assert.Equal(5, root.right.value);
        }
    }
}
