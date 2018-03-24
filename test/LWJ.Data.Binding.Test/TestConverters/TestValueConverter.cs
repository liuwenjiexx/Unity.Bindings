using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWJ.Data.Test
{
    [TestClass]
    public class TestValueConverter
    {

        [TestMethod]
        public void Boolean()
        {
            TestData target = new TestData();



            Binding binding = new Binding(null, ".", target, "BoolProperty", BindingMode.OneWay);
            binding.Converter = new BooleanConverter();

            binding.Bind();

            binding.Source = true;
            Assert.IsTrue(target.BoolProperty);

            binding.Source = false;
            Assert.IsFalse(target.BoolProperty);

            binding.Source = "true";
            Assert.IsTrue(target.BoolProperty);

            binding.Source = "false";
            Assert.IsFalse(target.BoolProperty);

            binding.Source = "True";
            Assert.IsTrue(target.BoolProperty);

            binding.Source = "False";
            Assert.IsFalse(target.BoolProperty);

            binding.ConverterParameter = "reverse";
            binding.Source = true;
            Assert.IsFalse(target.BoolProperty);

            binding.Source = false;
            Assert.IsTrue(target.BoolProperty);



        }

        [TestMethod]
        public void StringFormat()
        {
            TestData target = new TestData();

            Binding binding = new Binding(null, ".", target, "StringProperty", BindingMode.OneWay);
            binding.Converter = new StringFormatConverter();

            binding.Bind();

            binding.ConverterParameter = null;
            binding.Source = "a";
            Assert.IsNull(target.StringProperty);

            binding.ConverterParameter = string.Empty;
            binding.Source = "b";
            Assert.IsNull(target.StringProperty);

            binding.ConverterParameter = "{0}";
            binding.Source = "c";
            Assert.AreEqual("c", target.StringProperty);

            binding.ConverterParameter = "1{0}3";
            binding.Source = "2";
            Assert.AreEqual("123", target.StringProperty);
        }


     
    }
}
