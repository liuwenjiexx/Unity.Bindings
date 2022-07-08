using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yanmonet.Bindings
{

    class EnumerableAccessor : IAccessor
    {
        private int index;

        public EnumerableAccessor(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            this.index = index;
        }

        public bool CanGetValue(object target)
        {
            ICollection collection = target as ICollection;
            if (collection != null && index < collection.Count)
            {
                return true;
            }

            IEnumerator it = null;
            var enumerable = target as IEnumerable;
            if (enumerable != null)
            {
                it = enumerable.GetEnumerator();
            }
            else
            {
                it = target as IEnumerator;
            }
            if (it != null)
            {
                int n = 0;
                while (it.MoveNext())
                {
                    if (index >= n)
                        return true;
                    n++;
                }
            }
            return false;
        }

        public bool CanSetValue(object target)
        {
            return false;
        }

        public bool GetValue(object target, out object value)
        {
            IEnumerator it = null;
            var enumerable = target as IEnumerable;
            if (enumerable != null)
            {
                it = enumerable.GetEnumerator();
            }
            else
            {
                it = target as IEnumerator;
            }

            if (it != null)
            {
                if (GetValue(it, index, out value))
                    return true;
            }
            throw new AccessViolationException();
            return true;
        }

        public void SetValue(object target, object value)
        {
            throw new AccessViolationException();
        }

        public bool GetValue(IEnumerator it, int index, out object value)
        {
            int n = 0;
            value = null;
            bool hasValue = false;
            while (it.MoveNext())
            {
                if (n == index)
                {
                    hasValue = true;
                    value = it.Current;
                    break;
                }
                n++;
            }

            return hasValue;
        }
    }
}