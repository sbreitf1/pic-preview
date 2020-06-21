using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPreview;
using System;

namespace PicPreviewTests
{
    [TestClass]
    public class OrderedMapTests
    {
        [TestMethod]
        public void TestTypicalUsage()
        {
            OrderedMap<string> map = new OrderedMap<string>();
            Assert.AreEqual(0, map.Count);
            Assert.AreEqual(0, map.Add("foo"));
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual(1, map.Add("bar"));
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual("foo", map.Get(0));
            Assert.AreEqual("bar", map.Get(1));
            int index;
            Assert.IsTrue(map.TryGetIndex("foo", out index));
            Assert.AreEqual(0, index);
            Assert.IsTrue(map.TryGetIndex("bar", out index));
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void TestAddRange()
        {
            OrderedMap<string> map = new OrderedMap<string>();
            Assert.AreEqual(0, map.Count);
            map.AddRange(new string[] { "foo", "bar" });
            Assert.AreEqual(2, map.Count);
            Assert.AreEqual("foo", map.Get(0));
            Assert.AreEqual("bar", map.Get(1));
            int index;
            Assert.IsTrue(map.TryGetIndex("foo", out index));
            Assert.AreEqual(0, index);
            Assert.IsTrue(map.TryGetIndex("bar", out index));
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void TestClear()
        {
            OrderedMap<string> map = new OrderedMap<string>();
            Assert.AreEqual(0, map.Count);
            map.Add("foo");
            Assert.AreEqual(1, map.Count);
            map.Clear();
            Assert.AreEqual(0, map.Count);
            map.Add("bar");
            int index;
            Assert.IsFalse(map.TryGetIndex("foo", out index));
            Assert.IsTrue(map.TryGetIndex("bar", out index));
            Assert.AreEqual(0, index);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void TestGetOutOfRange()
        {
            OrderedMap<string> map = new OrderedMap<string>();
            map.Get(0);
        }
    }
}
