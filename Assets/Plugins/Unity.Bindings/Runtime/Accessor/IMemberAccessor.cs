using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Unity.Bindings
{
    public interface IMemberAccessor<TValue> : IAccessor<TValue>
    {
        MemberInfo MemberInfo { get; }
    }
}
