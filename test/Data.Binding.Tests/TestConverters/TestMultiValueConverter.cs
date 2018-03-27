using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LWJ.Data.Test
{
    [TestClass]
    public class TestMultiValueConverter
    {
        [TestMethod]
        public void StringFormat()
        {
            TestData data1 = new TestData();
            TestData data2 = new TestData();
            TestData target = new TestData();

            MultiBinding binding = new MultiBinding();
            binding.Add(new Binding(data1, "StringProperty"));
            binding.Add(new Binding(data2, "StringProperty"));
            binding.Target = target;
            binding.TargetPath = "StringProperty";
            binding.Converter = new StringFormatConverter();

            binding.Unbind();
            binding.ConverterParameter = null;
            binding.Bind();
            Assert.IsNull(target.StringProperty);

            binding.Unbind();
            binding.ConverterParameter = string.Empty;
            binding.Bind();
            Assert.IsNull(target.StringProperty);

            binding.Unbind();
            binding.ConverterParameter = "{0} {1}";
            data1.StringProperty = "hello";
            data2.StringProperty = "world";
            binding.Bind();
            Assert.AreEqual("hello world", target.StringProperty);
        }

        [TestMethod]
        public void StringJoin()
        {
            TestData data1 = new TestData();
            TestData data2 = new TestData();
            TestData target = new TestData();
            data1.StringProperty = "a";
            data2.StringProperty = "b";

            MultiBinding binding = new MultiBinding();
            binding.Add(new Binding(data1, "StringProperty"));
            binding.Add(new Binding(data2, "StringProperty"));
            binding.Target = target;
            binding.TargetPath = "StringProperty";
            binding.Converter = new StringJoinConverter();
            
            binding.ConverterParameter = null;
            binding.Bind();
            Assert.AreEqual("ab", target.StringProperty);
             
            binding.ConverterParameter = string.Empty;
            binding.Bind();
            Assert.AreEqual("ab", target.StringProperty);
             
            binding.ConverterParameter = ", ";
            data1.StringProperty = "12";
            data2.StringProperty = "34";
            binding.Bind();
            Assert.AreEqual("12, 34", target.StringProperty);
        }


        [TestMethod]
        public void Divide()
        {
            TestData data1 = new TestData();
            TestData data2 = new TestData();
            TestData target = new TestData();

            MultiBinding binding = new MultiBinding();
            binding.Add(new Binding(data1, "IntProperty"));
            binding.Add(new Binding(data2, "IntProperty"));
            binding.Target = target;
            binding.TargetPath = "IntProperty";
            binding.Converter = new DivideConverter();

            binding.Unbind();
            data1.IntProperty = 10;
            data2.IntProperty = 2;
            binding.Bind();
            Assert.AreEqual(5, target.IntProperty);

            binding = new MultiBinding();
            binding.Add(new Binding(data1, "FloatProperty"));
            binding.Add(new Binding(data2, "FloatProperty"));
            binding.Target = target;
            binding.TargetPath = "FloatProperty";
            binding.Converter = new DivideConverter();
            binding.Unbind();
            data1.FloatProperty = 10;
            data2.FloatProperty = 2.0f;
            binding.Bind();
            Assert.AreEqual(5.0f, target.FloatProperty);
        }

    }
}
