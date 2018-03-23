using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.Data
{
    [Named("StringJoin")]
    public class StringJoinConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter)
        {
            string separator = parameter as string;

            if (separator == null)
                separator = "";

            StringBuilder sb = new StringBuilder();
            for (int i = 0, len = values.Length; i < len; i++)
            {
                if (i > 0)
                    sb.Append(separator);
                object value = values[i];
                if (value != null)
                    sb.Append(value.ToString());
            }
            return sb.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter)
        {
            string str = value as string;
            if (str == null)
                return new string[targetTypes.Length];
            string separator = parameter as string;
            if (string.IsNullOrEmpty(separator))
                throw new Exception("ConverterParameter Null");
            string[] parts = str.Split(new string[] { separator }, StringSplitOptions.None);
            string[] values = new string[targetTypes.Length];
            for (int i = 0, len = targetTypes.Length; i < len && i < parts.Length; i++)
            {
                values[i] = parts[i];
            }
            return values;
        }
    }

}
