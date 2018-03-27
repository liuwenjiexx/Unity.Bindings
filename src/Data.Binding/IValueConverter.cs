using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.Data
{


    public interface IValueConverter
    {

        object Convert(object value, Type targetType, object parameter);

        object ConvertBack(object value, Type targetType, object parameter);

    }

}
