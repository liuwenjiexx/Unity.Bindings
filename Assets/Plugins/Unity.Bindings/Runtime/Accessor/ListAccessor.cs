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

        public object GetValue(object target)
        {
            IList list = target as IList;
            if (list != null && index < list.Count)
            {
                return list[index];
            }
            throw new AccessViolationException();
        }

        public void SetValue(object target, object value)
        {
            IList list = target as IList;
            if (list != null && index < list.Count)
            {
                list[index] = value;
            }
            throw new AccessViolationException();
        }
    }

}