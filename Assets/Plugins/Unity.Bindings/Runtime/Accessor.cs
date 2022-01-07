using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEngine.Bindings
{
    public class Accessor<TValue> : INotifyValueChanged<TValue>
    {
        private Func<TValue> getter;
        private Action<TValue> setter;

        public Accessor(Func<TValue> getter, Action<TValue> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public TValue value
        {
            get
            {
                if (getter == null)
                    throw new MemberAccessException();
                return getter();
            }

            set
            {
                if (setter == null)
                    throw new MemberAccessException();
                if (!Equals(this.value, value))
                {
                    setter(value);
                }
            }
        }

        public void SetValueWithoutNotify(TValue newValue)
        {
            this.value = newValue;
        }

    }
}
