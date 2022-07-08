using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Yanmonet.Bindings
{
    class ListAccessor : IAccessor
    {

        private int index;

        public ListAccessor(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.index = index;
        }

        public bool CanGetValue(object target)
        {
            IList list = target as IList;
            if (list != null && index < list.Count)
            {
                return true;
            }
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

            IList list = target as IList;
            if (list != null && index < list.Count)
            {
                value = list[index];
                return true;
            }
            value = null;
            return true;
        }

        public void SetValue(object target, object value)
        {
            if (!CanSetValue(target))
                throw new AccessViolationException();

            IList list = target as IList;
            if (list != null && index < list.Count)
            {
                list[index] = value;
            }
        }
    }

}