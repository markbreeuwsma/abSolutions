using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToolsAndUtilities;

namespace ToolsAndUtilitiesTest
{
    [TestClass]
    public class StringHelperTest
    {
        [TestMethod]
        public void StringHelperIsAnagramTest()
        {
            Assert.IsTrue("listen".IsAnagram("inlets"), "listen IS an anagram of inlets");
            Assert.IsFalse("listens".IsAnagram("inlets"), "listens IS NOT an anagram of inlets");
        }
    }
}
