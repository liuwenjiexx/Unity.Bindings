using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWJ.Data.Test
{
    [TestClass]
    public class TestPathAccess
    {


        [TestMethod]
        public void Path_Empty()
        {
            PathAccess member = PathAccess.Create(null);

            Assert.IsNull(member.MemberName);

            member = PathAccess.Create("");

            Assert.IsNull(member.MemberName);

            member = PathAccess.Create(".");

            Assert.IsNull(member.MemberName);

        }


        [TestMethod]
        public void Path()
        {
            PathAccess member = PathAccess.Create("Next.Next");

            Assert.AreEqual("Next", member.MemberName);

            //Assert.IsNull(member.MemberType);
            Assert.IsFalse(member.IsIndexer);

            Assert.AreEqual("Next.Next", member.Path);

        }

        [TestMethod]
        public void GetValue()
        {
            TestData data1 = new TestData("1");
            TestData data2 = new TestData("2");
            TestData data3 = new TestData("3");

            PathAccess member = PathAccess.Create("Next.Next");
            member.Target = data1;

            object value;

            Assert.IsFalse(member.TryGetValue(out value));
            Assert.AreEqual(null, value);


            data1.Next = data2;
            data2.Next = data3;
            Assert.AreEqual(member.GetValueType(), typeof(TestData));
            Assert.IsTrue(member.TryGetValue(out value));
            Assert.AreEqual(data1.Next.Next, data3);
        }

        [TestMethod]
        public void SetValue()
        {
            TestData data1 = new TestData("1");
            TestData data2 = new TestData("2");
            TestData data3 = new TestData("3");

            PathAccess path = PathAccess.Create("Next.Next");
            path.Target = data1;

            object value;

            Assert.IsFalse(path.TryGetValue(out value));
            Assert.AreEqual(null, value);


            Assert.IsFalse(path.TrySetValue(data3));

            data1.Next = data2;

            Assert.IsTrue(path.TrySetValue(data3));
            Assert.AreEqual(data3, data1.Next.Next);

        }

        [TestMethod]
        public void ChangedCallback()
        {
            TestData data1 = new TestData("1");
            TestData data2 = new TestData("2");
            TestData data3 = new TestData("3");
            TestData data4 = new TestData("4");

            int i = 0;

            PathAccess path = PathAccess.Create("Next.Next.Next");
            path.ChangedCallback = () =>
            {
                i++;
            };

            path.Target = data1;
            Assert.AreEqual(1, i);

            i = 0;
            data1.Next = data2;
            Assert.AreEqual(1, i);

            i = 0;
            data2.Next = data3;
            Assert.AreEqual(1, i);
            Assert.AreEqual(data3, data1.Next.Next);

            i = 0;
            data3.Next = data4;
            Assert.AreEqual(1, i);
            Assert.AreEqual(data4, data1.Next.Next.Next);

            i = 0;
            data3 = new TestData("31");
            data2.Next = data3;
            Assert.AreEqual(1, i);

            i = 0;
            data4 = new TestData("41");
            data3.Next = data4;
            Assert.AreEqual(1, i);
 
        }


    }
}
