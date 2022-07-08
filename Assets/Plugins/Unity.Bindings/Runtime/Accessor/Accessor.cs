using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{

    public class Accessor : IAccessor
    {
        private Func<object, object> getter;
        private Action<object, object> setter;

        private static Dictionary<MemberInfo, IAccessor> cachedAccessors;

        public Accessor(Func<object, object> getter, Action<object, object> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public bool CanGetValue(object target) => getter != null;

        public bool CanSetValue(object target) => setter != null;

        public virtual bool GetValue(object target, out object value)
        {
            if (getter == null)
                throw new MemberAccessException();
            if (getter == null)
            {
                value = null;
                return false;
            }
            value = getter(target);
            return true;
        }


        public virtual void SetValue(object target, object value)
        {
            if (setter == null)
                throw new MemberAccessException();
            setter(target, value);
        }


        public static IAccessor Member(MemberInfo propertyOrField)
        {
            IAccessor accessor;

            if (propertyOrField == null)
                throw new ArgumentNullException(nameof(propertyOrField));

            if (cachedAccessors == null)
                cachedAccessors = new Dictionary<MemberInfo, IAccessor>();

            if (!cachedAccessors.TryGetValue(propertyOrField, out accessor))
            {

                ParameterExpression targetExpr = null;
                bool canWrite = false;
                MemberExpression memberExpr = null;
                Type targetType = null, valueType = null;

                var pInfo = propertyOrField as PropertyInfo;
                if (pInfo != null)
                {
                    targetType = pInfo.DeclaringType;
                    valueType = pInfo.PropertyType;
                    targetExpr = Expression.Parameter(targetType);
                    if (pInfo.GetGetMethod().IsStatic)
                        memberExpr = Expression.Property(null, pInfo);
                    else
                        memberExpr = Expression.Property(targetExpr, pInfo);
                    if (pInfo.CanWrite)
                        canWrite = true;
                }
                else
                {
                    var fInfo = propertyOrField as FieldInfo;
                    if (fInfo != null)
                    {
                        targetType = fInfo.DeclaringType;
                        valueType = fInfo.FieldType;
                        targetExpr = Expression.Parameter(targetType);
                        if (fInfo.IsStatic)
                            memberExpr = Expression.Field(null, fInfo);
                        else
                            memberExpr = Expression.Field(targetExpr, fInfo);
                        if (!fInfo.IsInitOnly)
                            canWrite = true;
                    }
                }


                Delegate getter = null, setter = null;

                if (canWrite)
                {

                    var valueExpr = Expression.Parameter(valueType);
                    var setterBody = Expression.Assign(memberExpr, valueExpr);
                    setter = Expression.Lambda(typeof(Action<,>).MakeGenericType(targetType, valueType), setterBody, targetExpr, valueExpr)
                        .Compile();
                }

                getter = Expression.Lambda(typeof(Func<,>).MakeGenericType(targetType, valueType), memberExpr, targetExpr)
                    .Compile();

                accessor = (IAccessor)Activator.CreateInstance(typeof(MemberAccessor<,>).MakeGenericType(targetType, valueType), getter, setter);
                cachedAccessors[propertyOrField] = accessor;
            }

            return accessor;
        }

        public static IAccessor<TValue> Member<TValue>(MemberInfo propertyOrField)
        {
            return (IAccessor<TValue>)Member(propertyOrField);
        }

        public static IAccessor<TValue> Member<TTarget, TValue>(Expression<Func<TTarget, TValue>> propertySelector)
        {
            MemberInfo member = propertySelector.FindMember();
            if (member == null)
                throw new ArgumentException(nameof(propertySelector));
            return Member<TValue>(member);
        }

        public static IAccessor<TValue> Member<TValue>(Expression<Func<TValue>> propertySelector)
        {
            MemberInfo member = propertySelector.FindMember();
            if (member == null)
                throw new ArgumentException(nameof(propertySelector));
            return Member<TValue>(member);
        }

        public static IAccessor Array(int index)
        {
            return new ArrayAccessor(index);
        }

        public static IAccessor List(int index)
        {
            return new ListAccessor(index);
        }

        public static IAccessor Enumerable(int index)
        {
            return new EnumerableAccessor(index);
        }
    }


    public class MemberAccessor<TTarget, TValue> : IAccessor<TValue>
    {
        private Func<TTarget, TValue> getter;
        private Action<TTarget, TValue> setter;

        public MemberAccessor(Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }

        public bool CanGetValue(object target) => getter != null;

        public bool CanSetValue(object target) => setter != null;

        public bool GetValue(object target, out object value)
        {
            if (getter == null)
            {
                value = default;
                return false;
            }
            value = getter((TTarget)target);
            return true;
        }

        public bool GetValue(object target, out TValue value)
        {
            if (getter == null)
            {
                value = default;
                return false;
            }
            value = getter((TTarget)target);
            return true;
        }

        public void SetValue(object target, TValue value)
        {
            if (setter == null)
                throw new MemberAccessException();
            setter((TTarget)target, value);
        }

        public void SetValue(object target, object value)
        {
            SetValue((TTarget)target, (TValue)value);
        }

    }

}
