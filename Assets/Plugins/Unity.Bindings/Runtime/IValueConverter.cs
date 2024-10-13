using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
namespace Unity.Bindings
{
    public interface IValueConverter
    {
        object Convert(object value, Type targetType, object parameter);

        object ConvertBack(object value, Type targetType, object parameter);
    }
}