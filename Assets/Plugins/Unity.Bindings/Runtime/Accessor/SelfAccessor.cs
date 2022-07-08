using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yanmonet.Bindings
{
    class SelfAccessor : IAccessor
    {
        public bool CanGetValue(object target) => true;

        public bool CanSetValue(object target) => false;

        public object GetValue(object target)
        {
            return target;
        }

        public void SetValue(object target, object value)
        {
            throw new AccessViolationException();
        }
    }
}
