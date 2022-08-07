using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yanmonet.Bindings
{

    class CombineAccessor : IAccessor
    {
        private IAccessor[] accessors;

        public CombineAccessor(params IAccessor[] accessors)
        {
            this.accessors = accessors;
        }


        private object GetTarget(object target)
        {
            object obj = target;
            for (int i = 0; i < accessors.Length - 1; i++)
            {
                if (!accessors[i].CanGetValue(obj))
                    return false;
                obj = accessors[i].GetValue(obj);
            }
            return obj;
        }

        public bool CanGetValue(object target)
        {
            return accessors[accessors.Length - 1].CanGetValue(GetTarget(target));
        }

        public bool CanSetValue(object target)
        {
            return accessors[accessors.Length - 1].CanSetValue(GetTarget(target));
        }

        public object GetValue(object target)
        {
            return accessors[accessors.Length - 1].GetValue(GetTarget(target));
        }

        public object SetValue(object target, object value)
        {
            return accessors[accessors.Length - 1].SetValue(GetTarget(target), value);
        }
    }


}
