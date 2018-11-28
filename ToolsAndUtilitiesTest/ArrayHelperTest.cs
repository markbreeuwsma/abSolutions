using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsAndUtilities;

namespace ToolsAndUtilitiesTest
{
    [TestClass]
    public class ArrayHelperTest
    {
        [TestMethod]
        public void ArrayHelperBinarySearch()
        {
            int[] numbers = { -7, -2, 1, 4, 7, 8, 100, 120 };
            Assert.IsTrue(numbers.BinarySearch(-2), "number -2 IS contained in the sorted list of integers");
            Assert.IsFalse(numbers.BinarySearch(2), "number 2 IS NOT contained in the sorted list of integers");
            Assert.IsFalse(ArrayHelper.BinarySearch(null, 2), "number 2 IS NOT contained in null (or is it?)");
        }
    }
}
