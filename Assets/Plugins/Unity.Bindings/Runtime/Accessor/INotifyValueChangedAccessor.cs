using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{

    interface INotifyValueChangedAccessor : IAccessor { }

    class INotifyValueChangedAccessor<TValue> : IAccessor<TValue>, INotifyValueChangedAccessor
    {

        private bool withoutNotify;


        private INotifyValueChangedAccessor(bool withoutNotify)
        {
            this.withoutNotify = withoutNotify;
        }

        public static readonly INotifyValueChangedAccessor<TValue> instance = new INotifyValueChangedAccessor<TValue>(false);

        public static readonly INotifyValueChangedAccessor<TValue> instanceWithoutNotify = new INotifyValueChangedAccessor<TValue>(true);

        public bool CanGetValue(object target) => true;

        public bool CanSetValue(object target) => true;

        TValue IAccessor<TValue>.GetValue(object target)
        {
            return ((INotifyValueChanged<TValue>)target).value;
        }
        public object GetValue(object target)
        {
            return ((INotifyValueChanged<TValue>)target).value;
        }

        public object SetValue(object target, TValue value)
        {
            if (withoutNotify)
            {
                ((INotifyValueChanged<TValue>)target).SetValueWithoutNotify(value);
            }
            else
            {
                ((INotifyValueChanged<TValue>)target).value = value;
            }
            return target;
        }

        public object SetValue(object target, object value)
        {
            return SetValue(target, (TValue)value);
        }

    }
}