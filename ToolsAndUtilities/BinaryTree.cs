using System;
using System.Collections.Generic;
using System.Text;

namespace ToolsAndUtilities
{
    /// <summary>
    ///  Example of a simple, NOT ballanced binairy (search) tree.
    ///  For a balanced tree, consider the SortedSet<> for a red-black tree.
    /// </summary>
    public class BinaryTree
    {
        private Node root = null;

        public BinaryTree()
        {
        }
        public BinaryTree(int value)
        {
            AddValue(value);
        }
        public BinaryTree(int[] values)
        {
            AddValue(values);
        }

        public void AddValue(int value)
        {
            // root = AddValueRecursive(root, value); this can cause stack overflow exceptions for large trees

            if (root == null)
            {                
                root = new Node();
                root.value = value;
                return;
            }

            Node checkNode = root;
            while (true)
            {
                if (checkNode.value == value)
                {
                    return;
                }
                else if (value < checkNode.value)
                {
                    if (checkNode.left == null)
                    {
                        checkNode.left = new Node();
                        checkNode.left.value = value;
                        return;
                    }
                    checkNode = checkNode.left;
                }
                else // if (value > checkNode.value)   if is implicit
                {
                    if (checkNode.right == null)
                    {
                        checkNode.right = new Node();
                        checkNode.right.value = value;
                        return;
                    }
                    checkNode = checkNode.right;
                }
            }
        }

        public void AddValue(int[] values)
        {
            foreach (int value in values)
            {
                AddValue(value);
            }
        }

        private Node AddValueRecursive(Node root, int v)
        {
            if (root == null)
            {
                root = new Node();
                root.value = v;
            }
            else if (v < root.value)
            {
                root.left = AddValueRecursive(root.left, v);
            }
            else if (v > root.value) // comment the if if more occurences of the same value is desired 
            {
                root.right = AddValueRecursive(root.right, v);
            }

            return root;
        }

        public bool SearchValueRecursive(int v)
        {
            return SearchValueRecursive(root, v);
        }
        private bool SearchValueRecursive(Node root, int v)
        {
            if (root != null)
            {
                if (root.value == v)
                {
                    return true;
                }
                else if (v < root.value)
                {
                    return SearchValueRecursive(root.left, v);
                }
                else // if (v > root.value)   if is implicit
                {
                    return SearchValueRecursive(root.right, v);
                }
            }
            return false;
        }

        public bool SearchValueNonRecursive(int v)
        {
            Node checkNode = root;

            while (true)
            {
                if (checkNode == null)
                {
                    return false;
                }
                else if (checkNode.value == v)
                {
                    return true;
                }
                else if (v < checkNode.value)
                {
                    checkNode = checkNode.left;
                }
                else // if (v > checkNode.value)   if is implicit
                {
                    checkNode = checkNode.right;
                }
            }
        }

        public void traverse()
        {
            traverseRecursive(root);
        }
        private void traverseRecursive(Node node)
        {
            if (node != null)
            {
                traverseRecursive(node.left);
                // Console.WriteLine("Node value: " + node.value);
                traverseRecursive(node.right);
            }
        }

        class Node
        {
            public int value;
            public Node left;
            public Node right;
        }
    }
}
