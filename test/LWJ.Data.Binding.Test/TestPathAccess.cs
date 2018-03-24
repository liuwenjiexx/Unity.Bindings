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
            PropertyPath member = PropertyPath.Create(null);

            //Assert.IsNull(member.MemberName);

            //member = PathAccess.Create("");

            //Assert.IsNull(member.MemberName);

            //member = PathAccess.Create(".");

            //Assert.IsNull(member.MemberName);

        }


        [TestMethod]
        public void Path()
        {
            PropertyPath member = PropertyPath.Create("Next.Next");

            //Assert.AreEqual("Next", member.MemberName);

            //Assert.IsNull(member.MemberType);
            //Assert.IsFalse(member.IsIndexer);

            Assert.AreEqual("Next.Next", member.Path);

        }

        [TestMethod]
        public void GetValue()
        {
            TestData data1 = new TestData("1");
            TestData data2 = new TestData("2");
            TestData data3 = new TestData("3");

            PropertyPath member = PropertyPath.Create("Next.Next");
            member.Target = data1;

            object value;

            Assert.IsFalse(member.TryGetValue(out value));
            Assert.AreEqual(null, value);


            data1.Next = data2;
            data2.Next = data3;
            Assert.AreEqual(typeof(TestData), member.GetValueType());
            Assert.IsTrue(member.TryGetValue(out value));
            Assert.AreEqual(data1.Next.Next, data3);
        }

        [TestMethod]
        public void SetValue()
        {
            TestData data1 = new TestData("1");
            TestData data2 = new TestData("2");
            TestData data3 = new TestData("3");

            PropertyPath path = PropertyPath.Create("Next.Next");
            path.Target = data1;

            object value;

            Assert.IsFalse(path.TryGetValue(out value));
            Assert.AreEqual(null, value);


            Assert.IsFalse(path.TrySetValue(data3));

            data1.Next = data2;

            Assert.IsTrue(path.TrySetValue(data3));
            Assert.AreEqual(data3, data1.Next.Next);

            path.TryGetValue(out value);
            Assert.AreEqual(data3, value);

        }

        [TestMethod]
        public void ChangedCallback_Counter()
        {
            TestData data1 = new TestData("1");
            TestData data2 = new TestData("2");
            TestData data3 = new TestData("3");
            TestData data4 = new TestData("4");

            int i = 0;

            PropertyPath path = PropertyPath.Create("Next.Next.Next");
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

        [TestMethod]
        public void ChangedCallback2()
        {
            TestData data1 = new TestData("1");
            bool changed = false;

            PropertyPath path = PropertyPath.Create("IntProperty");
            path.ChangedCallback = () =>
            {
                changed = true;
            };
            changed = false;
            path.Target = data1;
            Assert.IsTrue(changed);

            changed = false;
            data1.IntProperty = 1;
            Assert.IsTrue(changed);


            changed = false;
            path.Target = null;
            Assert.IsTrue(changed);

            changed = false;
            data1.IntProperty = 2;
            Assert.IsFalse(changed);

        }


        [TestMethod]
        public void ValueType_SetTarget2()
        {
            bool changed = false;
            object value;

            TestStructData data1 = new TestStructData("struct1");
            PropertyPath path = PropertyPath.Create("IntProperty");
            data1.IntProperty = 1;
            path.ChangedCallback = () =>
            {
                changed = true;
            };

            path.Target = data1;
            Assert.IsTrue(changed);
            Assert.AreEqual(1, data1.IntProperty);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(1, value);

            changed = false;
            path.Target = null;
            Assert.IsTrue(changed);
            Assert.IsFalse(path.TryGetValue(out value));
            Assert.IsNull(value);


            changed = false;
            data1.IntProperty = 1;
            path.Target = data1;
            Assert.IsTrue(changed);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void ValueType_SetValue()
        {
            TestStructData data1 = new TestStructData("struct1");

            PropertyPath path = PropertyPath.Create("IntProperty");
            path.Target = data1;
            bool changed = false;
            object value;

            path.ChangedCallback = () =>
            {
                changed = true;
            };

            changed = false;
            data1.IntProperty = 1;
            Assert.IsFalse(changed);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(0, value);

            data1.IntProperty = 0;
            changed = false;
            Assert.IsTrue(path.TrySetValue(1));
            Assert.IsFalse(changed);
            Assert.AreEqual(0, data1.IntProperty);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void ValueType_Path()
        {
            TestData data1 = new TestData("data1");
            TestStructData data2 = new TestStructData("struct1");
            TestData data3 = new TestData("struct1");

            data2.TestData = data3;
            data1.StructData = data2;
            data3.IntProperty = 1;


            PropertyPath path = PropertyPath.Create("StructData.TestData.IntProperty");

            bool changed = false;
            object value;

            path.ChangedCallback = () =>
            {
                changed = true;
            };

            changed = false;
            path.Target = data1;
            Assert.IsTrue(changed);
            Assert.IsTrue(path.TryGetValue(out value));

            changed = false;
            data3.IntProperty = 2;
            Assert.IsFalse(changed);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(2, value);
  
        }

        [TestMethod]
        public void NoNotify_Path()
        {
            TestDataNoNotify data1 = new TestDataNoNotify("data1"); 
            TestDataNoNotify data2 = new TestDataNoNotify("data2");

            data1.Next = data2;
            data2.IntProperty = 1;

            PropertyPath path = PropertyPath.Create("Next.IntProperty");

            bool changed = false;
            object value;

            path.ChangedCallback = () =>
            {
                changed = true;
            };

            changed = false;
            path.Target = data1;
            Assert.IsTrue(changed);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(1, value);

            changed = false;
            data2.IntProperty = 2;
            Assert.IsFalse(changed);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(2, value);

            changed = false;
            data1.Next = new TestDataNoNotify();
            Assert.IsFalse(changed);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(0, value);


        }

    }
}
