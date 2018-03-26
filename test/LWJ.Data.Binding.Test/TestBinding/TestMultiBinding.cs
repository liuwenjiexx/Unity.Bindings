using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LWJ.Data.Test
{
    [TestClass]
    public class TestMultiBinding
    {

        [TestMethod]
        public void Bind()
        {
            TestData data1 = new TestData();
            TestData data2 = new TestData();
            TestData data3 = new TestData();

            data1.StringProperty = "abc";
            data2.StringProperty = "123";

            MultiBinding mb = new MultiBinding();
            mb.Add(new Binding(data1, "StringProperty"));
            mb.Add(new Binding(data2, "StringProperty"));

            mb.Converter = new StringFormatConverter();
            mb.ConverterParameter = "{0} {1}";

            mb.Target = data3;
            mb.TargetPath = "StringProperty";

            mb.Bind();
            
            Assert.AreEqual("abc 123", data3.StringProperty);

        }

        [TestMethod]
        public void StringFormat()
        {
            TestData data1 = new TestData();
            TestData data2 = new TestData();
            TestData data3 = new TestData();

            data1.StringProperty = "abc";
            data2.StringProperty = "123";

            MultiBinding mb = new MultiBinding();
            mb.Add(new Binding(data1, "StringProperty"));
            mb.Add(new Binding(data2, "StringProperty"));

            mb.Converter = new StringFormatConverter();
            mb.ConverterParameter = "{0} {1}";

            mb.Target = data3;
            mb.TargetPath = "StringProperty";
            mb.StringFormat = "[{0}]";
            mb.Bind();

            Assert.AreEqual("[abc 123]", data3.StringProperty);

        }

    }
}
