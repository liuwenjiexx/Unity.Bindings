using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yanmonet.Bindings
{
    class ArrayAccessor : IAccessor
    {

        private int index;

        public ArrayAccessor(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.index = index;
        }

        public bool CanGetValue(object target)
        {
            Array array = target as Array;
            if (array != null && index < array.Length)
                return true;

            return false;
        }

        public bool CanSetValue(object target)
        {
            return CanGetValue(target);
        }

        public object GetValue(object target)
        {
            Array array = target as Array;
            if (array != null && index >= 0 && index < array.Length)
            {
                return array.GetValue(index);
            }
            throw new AccessViolationException();
        }

        public object SetValue(object target, object value)
        {
            Array array = (Array)target;
            if (array != null && index >= 0 && index < array.Length)
            {
                array.SetValue(value, index);
                return target;
            }
            throw new AccessViolationException();
        }
         
    }

}