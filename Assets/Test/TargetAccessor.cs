//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UIElements;

//namespace Yanmonet.Bindings
//{

//    public class TargetAccessor<TTarget, TValue> : INotifyValueChanged<TValue>
//    {
//        private IMemberAccessor accessor;
//        private TTarget target;


//        public TargetAccessor(TTarget target, IMemberAccessor accessor)
//        {
//            this.target = target;
//            this.accessor = accessor;
//        }

//        public TargetAccessor(TTarget target, Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
//            : this(target, new MemberAccessor<TTarget, TValue>(getter, setter))
//        {
//        }

//        public TTarget Target { get => target; set => target = value; }

//        public TValue value
//        {
//            get => (TValue)accessor.GetValue(target);

//            set
//            {
//                if (!Equals(this.value, value))
//                {
//                    accessor.SetValue(Target, value);
//                }
//            }
//        }

//        public void SetValueWithoutNotify(TValue newValue)
//        {
//            this.value = newValue;
//        }

//    }

//}
