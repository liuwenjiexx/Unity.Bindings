using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yanmonet.Bindings
{
    public interface IAccessor
    {

        bool CanGetValue(object target);

        bool CanSetValue(object target);


        bool GetValue(object target, out object value);

        void SetValue(object target, object value);
    }

    public interface IAccessor<TValue> : IAccessor
    {
        bool GetValue(object target, out TValue value);

        void SetValue(object target, TValue value);
    }

}
