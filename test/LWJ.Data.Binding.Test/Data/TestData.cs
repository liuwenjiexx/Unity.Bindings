using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWJ.Data.Test
{
    public class TestData : INotifyPropertyChanged
    {
        private string name;

        private string stringProperty;
        private int intProperty;
        private long longProperty;
        private float floatProperty;
        private double doubleProperty;
        private TestData next;
        

        public TestData() {
        }

        public TestData(string name)
        {
            this.name = name;
        }

        public string StringProperty
        {
            get
            {
                return stringProperty;
            }
            set
            {
                if (stringProperty != value)
                {
                    stringProperty = value;
                    PropertyChanged.Invoke(this, nameof(StringProperty));
                }
            }
        }

        public int IntProperty
        {
            get
            {
                return intProperty;
            }
            set
            {
                if (intProperty != value)
                {
                    intProperty = value;
                    PropertyChanged.Invoke(this, nameof(IntProperty));
                }
            }
        }

        public long LongProperty
        {
            get
            {
                return longProperty;
            }
            set
            {
                if (longProperty != value)
                {
                    longProperty = value;
                    PropertyChanged.Invoke(this, nameof(LongProperty));
                }
            }
        }

        public float FloatProperty
        {
            get
            {
                return floatProperty;
            }
            set
            {
                if (floatProperty != value)
                {
                    floatProperty = value;
                    PropertyChanged.Invoke(this, nameof(FloatProperty));
                }
            }
        }

        public double DoubleProperty
        {
            get
            {
                return doubleProperty;
            }
            set
            {
                if (doubleProperty != value)
                {
                    doubleProperty = value;
                    PropertyChanged.Invoke(this, nameof(DoubleProperty));
                }
            }
        }
        public TestData Next
        {
            get
            {
                return next;
            }
            set
            {
                if (next != value)
                {
                    next = value;
                    PropertyChanged.Invoke(this, nameof(Next));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return name ?? "";
        }
    }
}
