using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    class NotifyValueChangedAccessor<TValue> : IAccessor<TValue>
    {

        private bool withoutNotify;


        private NotifyValueChangedAccessor(bool withoutNotify)
        {
            this.withoutNotify = withoutNotify;
        }

        public static readonly NotifyValueChangedAccessor<TValue> instance = new NotifyValueChangedAccessor<TValue>(false);

        public static readonly NotifyValueChangedAccessor<TValue> instanceWithoutNotify = new NotifyValueChangedAccessor<TValue>(true);

        public bool CanGetValue(object target) => true;

        public bool CanSetValue(object target) => true;

        public bool GetValue(object target, out TValue value)
        {
            value = ((INotifyValueChanged<TValue>)target).value;
            return true;
        }
        public bool GetValue(object target, out object value)
        {
            value = ((INotifyValueChanged<TValue>)target).value;
            return true;
        }

        public void SetValue(object target, TValue value)
        {
            if (withoutNotify)
            {
                ((INotifyValueChanged<TValue>)target).SetValueWithoutNotify(value);
            }
            else
            {
                ((INotifyValueChanged<TValue>)target).value = value;
            }
        }

        public void SetValue(object target, object value)
        {
            SetValue(target, (TValue)value);
        }

    }
}