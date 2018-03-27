using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace LWJ.Data.Test
{
    public struct TestStructData : INotifyPropertyChanged
    {
        private TestData testData;
        private int intProperty;
        private string name;
         

        public TestStructData(string name)
        {
            testData = null;
            intProperty = 0;
            this.name = name;
            PropertyChanged = null;
        }

        public TestData TestData
        {
            get { return testData; }
            set
            {
                if (testData != value)
                {
                    testData = value;
                    PropertyChanged.Invoke(this, nameof(TestData));
                }
            }
        }
        public int IntProperty
        {
            get { return intProperty; }
            set
            {
                if (intProperty != value)
                {
                    intProperty = value;
                    PropertyChanged.Invoke(this, nameof(IntProperty));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
