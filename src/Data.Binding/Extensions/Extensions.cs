using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LWJ.Data
{
    public static class Extensions
    {

        public static void Invoke(this PropertyChangedEventHandler source, object thisObj, string propertyName)
        {
            var propertyChanged = source;
            if (propertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                propertyChanged(thisObj, args);
            }
        }
    }
}
