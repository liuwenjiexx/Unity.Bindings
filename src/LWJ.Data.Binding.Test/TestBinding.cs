using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LWJ.Data.Test
{
    [TestClass]
    public class TestBinding
    {


        [TestMethod]
        public void Bind()
        {

            TestData target = new TestData();

            string text = "hello";

            Binding binding = new Binding(text, ".", target, "StringProperty", BindingMode.OneWay);

            Assert.IsNull(target.StringProperty);
            binding.Bind();

            Assert.AreEqual(text, target.StringProperty);
        }

        [TestMethod]
        public void Unbind()
        {

            TestData target = new TestData();

            string value = "hello";

            Binding binding = new Binding(value, ".", target, "StringProperty", BindingMode.OneWay);

            Assert.IsNull(target.StringProperty);

            binding.Bind();

            Assert.AreEqual(value, target.StringProperty);

            binding.Unbind();

            value = "world";
            binding.Source = value;

            Assert.AreEqual("hello", target.StringProperty);
        }



        [TestMethod]
        public void Set_Source()
        {

            TestData target = new TestData();

            string value = "hello";

            Binding binding = new Binding(value, ".", target, "StringProperty", BindingMode.OneWay);

            Assert.IsNull(target.StringProperty);

            binding.Bind();

            Assert.AreEqual(value, target.StringProperty);

            value = "world";
            binding.Source = value;

            Assert.AreEqual(value, target.StringProperty);
        }

        [TestMethod]
        public void Mode_OneWay()
        {
            TestData source = new TestData();
            TestData target = new TestData();

            Binding binding = new Binding(source, "IntProperty", target, "IntProperty", BindingMode.OneWay);
            binding.Bind();

            Assert.AreEqual(0, source.IntProperty, "source");
            Assert.AreEqual(0, target.IntProperty, "target");

            source.IntProperty = 1;
            Assert.AreEqual(1, source.IntProperty, "source");
            Assert.AreEqual(1, target.IntProperty, "target");

            target.IntProperty = 2;
            Assert.AreEqual(1, source.IntProperty, "source");
            Assert.AreEqual(2, target.IntProperty, "target");

        }

        [TestMethod]
        public void Mode_OneWayToSource()
        {
            TestData source = new TestData();
            TestData target = new TestData();

            Binding binding = new Binding(source, "IntProperty", target, "IntProperty", BindingMode.OneWayToSource);
            binding.Bind();

            Assert.AreEqual(0, source.IntProperty, "source");
            Assert.AreEqual(0, target.IntProperty, "target");

            source.IntProperty = 1;
            Assert.AreEqual(1, source.IntProperty, "source");
            Assert.AreEqual(0, target.IntProperty, "target");

            target.IntProperty = 2;
            Assert.AreEqual(2, source.IntProperty, "source");
            Assert.AreEqual(2, target.IntProperty, "target");
        }

        [TestMethod]
        public void Mode_TwoWay()
        {
            TestData source = new TestData();
            TestData target = new TestData();

            Binding binding = new Binding(source, "IntProperty", target, "IntProperty", BindingMode.TwoWay);
            binding.Bind();

            Assert.AreEqual(0, source.IntProperty, "source");
            Assert.AreEqual(0, target.IntProperty, "target");

            source.IntProperty = 1;
            Assert.AreEqual(1, source.IntProperty, "source");
            Assert.AreEqual(1, target.IntProperty, "target");

            target.IntProperty = 2;
            Assert.AreEqual(2, source.IntProperty, "source");
            Assert.AreEqual(2, target.IntProperty, "target");
        }


        private void TestPropertyType<T>(T value, string propertyName)
        {
            TestData target = new TestData();


            Binding binding = new Binding(value, ".", target, propertyName, BindingMode.OneWay);

            var pInfo = target.GetType().GetProperty(propertyName);

            binding.Bind();
            Assert.AreEqual(value, pInfo.GetValue(target));
        }


        [TestMethod]
        public void Property_Types()
        {

            TestPropertyType("hello", "StringProperty");
            TestPropertyType(1, "IntProperty");
            TestPropertyType(1L, "LongProperty");
            TestPropertyType(1.1f, "FloatProperty");
            TestPropertyType(1.1d, "DoubleProperty");
        }
  

    }


}
