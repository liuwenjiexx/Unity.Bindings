using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.Data
{
    public interface IMultiValueConverter
    {
        object Convert(object[] values, Type targetType, object parameter);

        object[] ConvertBack(object value, Type[] targetTypes, object parameter);


    }
}
