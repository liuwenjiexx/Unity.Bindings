using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LWJ.Data.Test
{
    [TestClass]
    public class TestBinding
    {


        [TestMethod]
        public void Bind_SourcePath_Null()
        {
            TestData target = new TestData();

            Binding binding = new Binding("abc", null, target, "StringProperty", BindingMode.OneWay);

            binding.Bind();

            Assert.AreEqual("abc", target.StringProperty);
        }

        [TestMethod]
        public void Unbind()
        {
            TestData data1 = new TestData();
            TestData target = new TestData();

            Binding binding = new Binding(data1, "StringProperty", target, "StringProperty", BindingMode.OneWay);

            binding.Bind();

            data1.StringProperty = "abc";
            Assert.AreEqual("abc", target.StringProperty);

            binding.Unbind();

            data1.StringProperty = "123";
            Assert.AreEqual("abc", target.StringProperty);

            binding.Bind();

            Assert.AreEqual("123", target.StringProperty);

            binding.Unbind();
            binding.Source = null;
            Assert.AreEqual("123", target.StringProperty);


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

        [TestMethod]
        public void StringFormat()
        {
            TestData target = new TestData();

            Binding binding = new Binding("123", null, target, "StringProperty", BindingMode.OneWay);
            binding.StringFormat = "{0}456";

            binding.Bind();

            Assert.AreEqual("123456", target.StringProperty);
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

        [TestMethod]
        public void Binding_ValueType()
        {
            TestStructData data1 = new TestStructData("struct1");

            PropertyPath path = PropertyPath.Create("IntProperty");
            bool changed = false;
            path.TargetUpdatedCallback = () =>
            {
                changed = true;
            };
            changed = false;
            path.Target = data1;
            Assert.IsTrue(changed);

            changed = false;
            path.Target = null;
            path.Target = data1;
            Assert.IsTrue(changed);

            changed = false;
            data1.IntProperty = 1;
            Assert.IsFalse(changed);

            object value;
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(0, value);

            data1.IntProperty = 0;
            Assert.IsTrue(path.TrySetValue(2));
            changed = false;
            path.TargetUpdatedCallback = () =>
            {
                changed = true;
            };
            Assert.AreEqual(0, data1.IntProperty);
            Assert.IsTrue(path.TryGetValue(out value));
            Assert.AreEqual(2, value);
        }


        [TestMethod]
        public void ValueType_GetValue()
        {

        }


        [TestMethod]
        public void Test1()
        {
            int i = 0;

            TestStructData data1 = new TestStructData();

            data1.PropertyChanged += (o, e) =>
            {
                i++;
            };
            TestStructData data2 = data1;


            i = 0;
            data1.IntProperty = 1;
            Assert.AreEqual(1, data1.IntProperty);
            Assert.AreEqual(0, data2.IntProperty);
            Assert.AreEqual(1, i);

            i = 0;
            data2.IntProperty = 2;
            Assert.AreEqual(1, data1.IntProperty);
            Assert.AreEqual(2, data2.IntProperty);
            Assert.AreEqual(1, i);
        }


        [TestMethod]
        public void TargetUpdated_OneWay()
        {
            TestData target = new TestData();

            Binding binding = new Binding("abc", null, target, "StringProperty", BindingMode.OneWay);

            int sourceChanged = 0, targetChanged = 0;
            binding.SourceUpdated += (o, e) =>
            {
                sourceChanged++;
            };
            binding.TargetUpdated += (o, e) =>
            {
                targetChanged++;
            };
            Action reset= ()=> {
                sourceChanged = 0;
                targetChanged = 0;
            };

            reset();
            binding.Bind();

            Assert.AreEqual(0, sourceChanged);
            Assert.AreEqual(1, targetChanged);

            reset();
            binding.Source = "123";
            Assert.AreEqual(0, sourceChanged);
            Assert.AreEqual(1, targetChanged);


            binding.Unbind();

            TestData data1 = new TestData();
            data1.StringProperty = "abc";

            binding.Path = "StringProperty";
            binding.Source = data1;

            reset();
            binding.Bind();
            Assert.AreEqual(0, sourceChanged);
            Assert.AreEqual(1, targetChanged);

            reset();
            data1.StringProperty = "123";
            Assert.AreEqual(0, sourceChanged);
            Assert.AreEqual(1, targetChanged);
            
            reset();
            target.StringProperty = "456";
            Assert.AreEqual(0, sourceChanged);
            Assert.AreEqual(0, targetChanged);

        }



        [TestMethod]
        public void TargetUpdated_TwoWay()
        {
            TestData target = new TestData();

            Binding binding = new Binding("abc", null, target, "StringProperty", BindingMode.TwoWay);

            int sourceChanged = 0, targetChanged = 0;
            binding.SourceUpdated += (o, e) =>
            {
                sourceChanged++;
            };
            binding.TargetUpdated += (o, e) =>
            {
                targetChanged++;
            };
            Action reset = () => {
                sourceChanged = 0;
                targetChanged = 0;
            };

            reset();
            binding.Bind();

            Assert.AreEqual(0, sourceChanged);
            Assert.AreEqual(1, targetChanged);

            reset();
            binding.Source = "123";
            Assert.AreEqual(0, sourceChanged);
            Assert.AreEqual(1, targetChanged);


            binding.Unbind();

            TestData data1 = new TestData();
            data1.StringProperty = "abc";

            binding.Path = "StringProperty";
            binding.Source = data1;

            reset();
            binding.Bind();
            Assert.AreEqual(1, sourceChanged);
            Assert.AreEqual(1, targetChanged);

            reset();
            data1.StringProperty = "123";
            Assert.AreEqual(1, sourceChanged);
            Assert.AreEqual(1, targetChanged);

            reset();
            target.StringProperty = "456";
            Assert.AreEqual(1, sourceChanged);
            Assert.AreEqual(1, targetChanged);
        }


    }


}
