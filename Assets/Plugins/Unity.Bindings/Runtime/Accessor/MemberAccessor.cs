using System;
using System.Linq.Expressions;
using System.Reflection;
using YMFramework;

namespace Yanmonet.Bindings
{

    public class MemberAccessor<TTarget, TValue> : Accessor<TTarget, TValue>, IMemberAccessor<TValue>
    {
        private MemberInfo memberInfo;

        public MemberAccessor(Expression<Func<TTarget, TValue>> memberSelector, Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
            : this(BindingUtility.GetMember(memberSelector), getter, setter)
        {
        }

        public MemberAccessor(MemberInfo memberInfo, Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
        : base(getter, setter)
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));

            this.memberInfo = memberInfo;
        }
        public MemberAccessor(MemberInfo memberInfo, Func<TTarget, TValue> getter, Func<TTarget, TValue, TTarget> setter)
        : base(getter, setter)
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));

            this.memberInfo = memberInfo;
        }

        public MemberAccessor(MemberInfo memberInfo, Func<TTarget, TValue> getter)
        : base(getter, null)
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));

            this.memberInfo = memberInfo;
        }

        public MemberInfo MemberInfo { get => memberInfo; }
    }
}
