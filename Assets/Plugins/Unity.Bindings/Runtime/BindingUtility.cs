using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine.UIElements;


namespace Yanmonet.Bindings
{
    public static class BindingUtility
    {


        #region Lambda

        internal static MemberInfo GetMember<TTarget, TValue>(Expression<Func<TTarget, TValue>> selector)
        {
            MemberExpression memberExpr = null;

            if (selector.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = selector.Body as MemberExpression;
            }
            if (memberExpr != null)
                return memberExpr.Member;
            return null;
        }

        internal static MemberInfo GetMember<TValue>(Expression<Func<TValue>> selector)
        {
            MemberExpression memberExpr = null;

            if (selector.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = selector.Body as MemberExpression;
            }
            if (memberExpr != null)
                return memberExpr.Member;
            return null;
        }


        #endregion


        #region RegisterValueChangedCallback

        private static MethodInfo registerValueChangedCallbackMethod;
        private static MethodInfo unregisterValueChangedCallbackMethod;

        private static Dictionary<Type, (Action<object, EventCallback<IChangeEvent>>, Action<object, EventCallback<IChangeEvent>>)> cachedRegisterValueChangedCallbacks;

        private static (Action<object, EventCallback<IChangeEvent>>, Action<object, EventCallback<IChangeEvent>>) GetRegisterValueChangedCallback(Type valueType)
        {
            if (cachedRegisterValueChangedCallbacks == null)
                cachedRegisterValueChangedCallbacks = new Dictionary<Type, (Action<object, EventCallback<IChangeEvent>>, Action<object, EventCallback<IChangeEvent>>)>();
            if (!cachedRegisterValueChangedCallbacks.TryGetValue(valueType, out var item))
            {
                if (registerValueChangedCallbackMethod == null)
                {
                    registerValueChangedCallbackMethod = typeof(INotifyValueChangedExtensions).GetMethod("RegisterValueChangedCallback");
                    unregisterValueChangedCallbackMethod = typeof(INotifyValueChangedExtensions).GetMethod("UnregisterValueChangedCallback");
                }

                Action<object, EventCallback<IChangeEvent>> registerValueChangedCallback = null;
                Action<object, EventCallback<IChangeEvent>> unregisterValueChangedCallback = null;


                var registerValueChangedCallbackMethod2 = registerValueChangedCallbackMethod.MakeGenericMethod(valueType);
                var unregisterValueChangedCallbackMethod2 = unregisterValueChangedCallbackMethod.MakeGenericMethod(valueType);

                try
                {
                    var paramTarget = Expression.Parameter(typeof(object));
                    var paramEvent = Expression.Parameter(typeof(EventCallback<IChangeEvent>));
                    Expression body;
                    var targetType = typeof(INotifyValueChanged<>).MakeGenericType(valueType);
                    var targetExpr = Expression.Convert(paramTarget, targetType);
                    body = Expression.Call(registerValueChangedCallbackMethod2, targetExpr, paramEvent);
                    registerValueChangedCallback = Expression.Lambda<Action<object, EventCallback<IChangeEvent>>>(body, paramTarget, paramEvent).Compile();

                    body = Expression.Call(unregisterValueChangedCallbackMethod2, targetExpr, paramEvent);
                    unregisterValueChangedCallback = Expression.Lambda<Action<object, EventCallback<IChangeEvent>>>(body, paramTarget, paramEvent).Compile();
                }
                catch (Exception ex)
                {
                }

                if (registerValueChangedCallback == null)
                {
                    registerValueChangedCallback = (target, e) =>
                    {
                        registerValueChangedCallbackMethod2.Invoke(null, new object[] { target, e });
                    };
                    unregisterValueChangedCallback = (target, e) =>
                    {
                        unregisterValueChangedCallbackMethod2.Invoke(null, new object[] { target, e });
                    };
                }

                item = new(registerValueChangedCallback, unregisterValueChangedCallback);
                cachedRegisterValueChangedCallbacks[valueType] = item;
            }
            return item;
        }

        internal static void RegisterValueChangedCallback(Type valueType, object target, EventCallback<IChangeEvent> e)
        {
            var item = GetRegisterValueChangedCallback(valueType);
            item.Item1(target, e);
        }

        internal static void UnregisterValueChangedCallback(Type valueType, object target, EventCallback<IChangeEvent> e)
        {
            var item = GetRegisterValueChangedCallback(valueType);
            item.Item2(target, e);
        }

        #endregion


    }
}