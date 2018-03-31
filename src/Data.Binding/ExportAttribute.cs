using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LWJ.Data
{
    //[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    //internal sealed class ExportAttribute : Attribute
    //{
    //    public ExportAttribute(object key, object value)
    //    {
    //        this.Key = key;
    //        this.Value = value;
    //    }

    //    public object Key { get; set; }

    //    public int Priority { get; set; }

    //    public object Value { get; set; }

    //    public IEnumerable<object> GetValues(object key)
    //    {

    //    }

    //    public static IEnumerable<object> GetValues(Assembly assembly)
    //    {
    //        Type exportType;
    //        exportType = assembly.GetType("ExportAttribute", false);
    //    }

    //}


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ConverterAttribute : Attribute
    {
        public ConverterAttribute(string converterName)
        {
            this.ConverterName = converterName;
        }

        //public ConverterAttribute(Type converterType)
        //{
        //    this.ConverterType = converterType;
        //}

        public string ConverterName { get; set; }
      //  public string ConverterType { get; set; }
    }


}
