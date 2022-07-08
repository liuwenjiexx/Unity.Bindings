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

        public bool GetValue(object target, out object value)
        {
            value = target;
            return true;
        }

        public void SetValue(object target, object value)
        {
            throw new AccessViolationException();
        }
    }
}
