using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Yanmonet.Bindings
{
    public interface IMemberAccessor<TValue> : IAccessor<TValue>
    {
        MemberInfo MemberInfo { get; }
    }
}
