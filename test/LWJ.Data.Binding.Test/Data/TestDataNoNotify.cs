using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWJ.Data.Test
{
    class TestDataNoNotify
    {
        private string name;

        private string stringProperty;
        private int intProperty;
        private long longProperty;
        private float floatProperty;
        private double doubleProperty;
        private bool boolProperty;
        private TestDataNoNotify next;
        private TestStructData structData;

        public TestDataNoNotify()
        {

        }

        public TestDataNoNotify(string name)
        {
            this.name = name;
        }

        public string Name { get => name; set => name = value; }
        public string StringProperty { get => stringProperty; set => stringProperty = value; }
        public int IntProperty { get => intProperty; set => intProperty = value; }
        public long LongProperty { get => longProperty; set => longProperty = value; }
        public float FloatProperty { get => floatProperty; set => floatProperty = value; }
        public double DoubleProperty { get => doubleProperty; set => doubleProperty = value; }
        public bool BoolProperty { get => boolProperty; set => boolProperty = value; }
        public TestDataNoNotify Next { get => next; set => next = value; }
        public TestStructData StructData { get => structData; set => structData = value; }
        public override string ToString()
        {
            return name ?? "";
        }
    }
}
