using Microsoft.VisualStudio.TestTools.UnitTesting;
using PicPreview;

namespace PicPreviewTests
{
    [TestClass]
    public class FilePathTests
    {
        [TestMethod]
        public void TestFilePath()
        {
            FilePath path1 = @"c:\foo\bar.txt";
            FilePath path2 = @"C:\Foo\Bar.TXT";
            FilePath path3 = @"c:\foobar";
            FilePath path4 = @"";
            FilePath path5 = null;

            Assert.AreEqual(@"c:\foo\bar.txt", path1.ToString());
            Assert.AreEqual(@"C:\Foo\Bar.TXT", path2.ToString());
            Assert.AreEqual(@"c:\foobar", path3.ToString());
            Assert.AreEqual(@"", path4.ToString());
            Assert.AreEqual(null, path5.ToString());

            Assert.AreEqual(@"c:\foo", path1.Dir.ToString());
            Assert.AreEqual(@"C:\Foo", path2.Dir.ToString());
            Assert.AreEqual(@"c:\", path3.Dir.ToString());
            //Assert.AreEqual(@"", path4.Dir);
            Assert.AreEqual(null, path5.Dir);

            Assert.AreEqual(@"bar.txt", path1.FileName);
            Assert.AreEqual(@"Bar.TXT", path2.FileName);
            Assert.AreEqual(@"foobar", path3.FileName);
            Assert.AreEqual(@"", path4.FileName);
            Assert.AreEqual(null, path5.FileName);

            Assert.AreEqual(@"bar", path1.FileNameWithoutExtension);
            Assert.AreEqual(@"Bar", path2.FileNameWithoutExtension);
            Assert.AreEqual(@"foobar", path3.FileNameWithoutExtension);
            Assert.AreEqual(@"", path4.FileNameWithoutExtension);
            Assert.AreEqual(null, path5.FileNameWithoutExtension);

            Assert.AreEqual(@".txt", path1.Extension);
            Assert.AreEqual(@".TXT", path2.Extension);
            Assert.AreEqual(@"", path3.Extension);
            Assert.AreEqual(@"", path4.Extension);
            Assert.AreEqual(null, path5.Extension);

            Assert.IsFalse(path1.IsEmpty);
            Assert.IsFalse(path2.IsEmpty);
            Assert.IsFalse(path3.IsEmpty);
            Assert.IsTrue(path4.IsEmpty);
            Assert.IsTrue(path5.IsEmpty);

            Assert.IsTrue(path1.Equals(path1));
            Assert.IsTrue(path1.Equals(path2));
            Assert.IsFalse(path1.Equals(path3));
            Assert.IsFalse(path1.Equals(path4));
            Assert.IsFalse(path1.Equals(path5));

            Assert.IsTrue(path2.Equals(path1));
            Assert.IsTrue(path2.Equals(path2));
            Assert.IsFalse(path2.Equals(path3));
            Assert.IsFalse(path2.Equals(path4));
            Assert.IsFalse(path2.Equals(path5));

            Assert.IsFalse(path3.Equals(path1));
            Assert.IsFalse(path3.Equals(path2));
            Assert.IsTrue(path3.Equals(path3));
            Assert.IsFalse(path3.Equals(path4));
            Assert.IsFalse(path3.Equals(path5));

            Assert.IsFalse(path4.Equals(path1));
            Assert.IsFalse(path4.Equals(path2));
            Assert.IsFalse(path4.Equals(path3));
            Assert.IsTrue(path4.Equals(path4));
            Assert.IsFalse(path4.Equals(path5));

            Assert.IsFalse(path5.Equals(path1));
            Assert.IsFalse(path5.Equals(path2));
            Assert.IsFalse(path5.Equals(path3));
            Assert.IsFalse(path5.Equals(path4));
            Assert.IsTrue(path5.Equals(path5));

            Assert.IsTrue(path1 == path2);
            Assert.IsFalse(path1 == path3);
            Assert.IsFalse(path4 == path5);
            Assert.IsFalse(path5 == path4);
            Assert.IsTrue(path5 == null);
            Assert.IsTrue(null == path5);
        }
    }
}
