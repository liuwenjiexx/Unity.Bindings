using System;
using System.Collections;

namespace LWJ.Data
{

    /// <summary>
    /// parameter is Format
    /// </summary>
    [Named("StringFormat")]
    public class StringFormatConverter : IMultiValueConverter, IValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter)
        {
            string format = parameter as string;
            if (string.IsNullOrEmpty(format))
                return null;

            return string.Format(format, values);
        }

        public object Convert(object value, Type targetType, object parameter)
        {
            string format = parameter as string;
            if (string.IsNullOrEmpty(format))
                return null;

            return string.Format(format, value);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
