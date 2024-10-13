using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Bindings;

namespace Unity.Bindings
{
    class ThisAccessor : IAccessor
    {

        public Type ValueType => null;

        public bool CanGetValue(object target) => true;

        public bool CanSetValue(object target) => false;
         
        public object GetValue(object target)
        {
            return target;
        }

        public object SetValue(object target, object value)
        {
            throw new AccessViolationException();
        }
    }
}
