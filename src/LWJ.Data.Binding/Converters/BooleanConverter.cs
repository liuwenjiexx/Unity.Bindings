using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.Data
{
    [Named("Boolean")]
    public class BooleanConverter : IValueConverter, IMultiValueConverter
    {

        private bool ToBool(object value)
        {
            bool b = false;
            if (value == null)
            {
                b = false;
            }
            else if (value is bool)
            {
                b = (bool)value;
            }
            else if (value is string)
            {
                bool.TryParse(value.ToString(), out b);
            }
            else
            {
                b = value != null;
            }
            return b;
        }

        public object Convert(object value, Type targetType, object parameter)
        {
            bool b = ToBool(value);
            if (parameter.ToStringOrEmpty() == "reverse")
                b = !b;
            return b;
        }



        public object ConvertBack(object value, Type targetType, object parameter)
        {
            bool b = (bool)Convert(value, typeof(bool), parameter);
            if (targetType == typeof(string))
            {
                return b.ToString();
            }
            else if (typeof(bool) == targetType)
            {
                return b;
            }
            else if (targetType == typeof(int))
            {
                return b ? 1 : 0;
            }

            return null;
        }


        public object Convert(object[] values, Type targetType, object parameter)
        {
            bool b;
            if (values.Length == 0)
            {
                b = false;
            }
            else
            {
                b = true;
                foreach (var val in values)
                {
                    b &= ToBool(val);
                    if (!b)
                        break;
                }
            }
            return Convert(b, targetType, parameter);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter)
        {
            if (targetTypes == null)
                return null;
            object[] values = new object[targetTypes.Length];
            for (int i = 0, len = targetTypes.Length; i < len; i++)
                values[i] = ConvertBack(value, targetTypes[i], parameter);
            return values;
        }
    }
}
