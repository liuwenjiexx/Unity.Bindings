using System;

namespace LWJ.Data
{

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
