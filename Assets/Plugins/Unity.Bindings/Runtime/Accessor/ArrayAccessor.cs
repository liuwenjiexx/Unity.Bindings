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

        public bool GetValue(object target, out object value)
        {
            if (!CanGetValue(target))
                throw new AccessViolationException();
            Array array = target as Array;
            if (array != null)
            {
                value = array.GetValue(index);
                return true;
            }
      
            value = null;
            return true;
        }

        public void SetValue(object target, object value)
        {
            if (!CanSetValue(target))
                throw new AccessViolationException();
            Array array = (Array)target;
            if (array != null)
            {
                array.SetValue(value, index);
            } 
        }
    }

}