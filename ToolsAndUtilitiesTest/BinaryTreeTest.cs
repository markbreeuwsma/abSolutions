using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ToolsAndUtilities;

namespace ToolsAndUtilitiesTest
{
    [TestClass]
    public class BinaryTreeTest
    {
        [TestMethod]
        public void BinaryTreeSearch()
        {

            int[] numbers = { 10, 4, 5, 11, -2, 8, 19, 10, 2, -1, 100, 99, 101, 77, -20, 97, 10 };

            BinaryTree binaryTree = new BinaryTree(numbers);
            BinaryTree emptyTree = new BinaryTree();
            BinaryTree hugeTree = new BinaryTree();

            for (int i = 0; i < 100000; i++)
            {
                hugeTree.AddValue(i);
            }

            Assert.IsTrue(binaryTree.SearchValueRecursive(11), "number 11 IS contained in the binary tree");
            Assert.IsFalse(binaryTree.SearchValueRecursive(12), "number 12 IS NOT contained in the binary tree");
            Assert.IsTrue(binaryTree.SearchValueNonRecursive(11), "number 11 IS contained in the binary tree");
            Assert.IsFalse(binaryTree.SearchValueNonRecursive(12), "number 12 IS NOT contained in the binary tree");

            Assert.IsFalse(emptyTree.SearchValueRecursive(11), "number 11 IS NOT contained in the empty binary tree");
            Assert.IsFalse(emptyTree.SearchValueNonRecursive(12), "number 12 IS NOT contained in the empty binary tree");

            Assert.IsTrue(hugeTree.SearchValueRecursive(11), "number 11 IS contained in the huge binary tree");
            Assert.IsTrue(hugeTree.SearchValueNonRecursive(11), "number 11 IS contained in the huge binary tree");

            // The next statement throws a StackOverflowException, which starting with the .NET Framework version 2.0, 
            // cannot be caught by a try-catch block and the corresponding process is terminated by default, so there
            // is no way to neatly test for it. You can do it, it's just not terribly natural. It required running the
            // code under test in another process and dealing with the pain that is checking the result of that process.
            // But uncomment it to see the test terminate, which is a test in itself ;) 
            
            // Assert.ThrowsException<StackOverflowException>(() => hugeTree.SearchValueRecursive(99990), "number 99990 WILL throw an stack overflow exception for huge binary tree");

            Assert.IsTrue(hugeTree.SearchValueNonRecursive(99990), "number 99990 IS contained in the huge binary tree");
        }
    }
}
