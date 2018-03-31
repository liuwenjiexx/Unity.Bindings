using System;

namespace LWJ.Data
{
    public interface IMultiValueConverter
    {
        object Convert(object[] values, Type targetType, object parameter);

        object[] ConvertBack(object value, Type[] targetTypes, object parameter);  
    }
}
