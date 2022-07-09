using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

namespace Yanmonet.Bindings
{
    public static class Extensions
    {
        #region BindingBuilder

        public static BindingBuilder<TTarget, TSource> Bind<TTarget, TSource>(this TTarget target, TSource source)
        {
            return new BindingBuilder<TTarget, TSource>(target, source);
        }

        public static BindingBuilder<TTarget, object> Bind<TTarget>(this TTarget target)
        {
            return new BindingBuilder<TTarget, object>(target, null);
        }


        #endregion

        #region Binding Path

        /// <summary>
        /// 绑定属性路径
        /// </summary> 
        public static BindingBase Bind(this object target, IAccessor targetAccessor, string targetPropertyName, object source, string path, BindingMode mode = BindingMode.OneWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (path == null) throw new ArgumentNullException(nameof(path));

            Binding binding = new Binding(target, targetAccessor, source, path);
            binding.Mode = mode;
            binding.TargetPropertyName = targetPropertyName;

            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                bindable.binding = binding;
            }
            return binding;
        }

        /// <summary>
        /// 绑定属性路径
        /// </summary>
        public static BindingBase Bind<TTarget, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, object source, string path, BindingMode mode = BindingMode.OneWay)
        {
            var targetAccessor = Accessor.Member(targetPropertySelector);
            return Bind(target, targetAccessor, targetAccessor.MemberInfo.Name, source, path, mode);
        }

        #endregion

        #region Binding Property

        /// <summary>
        /// 绑定属性
        /// </summary>
        public static BindingBase Bind<TSource, TValue>(this object target, IAccessor<TValue> targetAccessor, string targetPropertyName, TSource source, IAccessor<TValue> accessor, string propertyName, BindingMode mode = BindingMode.OneWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));

            var binding = new PropertyBinding(target, targetAccessor, source, accessor);
            binding.Mode = mode;
            binding.TargetPropertyName = targetPropertyName;
            binding.PropertyName = propertyName;

            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                bindable.binding = binding;
            }
            return binding;
        }

        public static BindingBase Bind<TTarget, TSource, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, TSource source, Expression<Func<TSource, TValue>> propertySelector, BindingMode mode = BindingMode.OneWay)
        {
            if (targetPropertySelector == null) throw new ArgumentNullException(nameof(targetPropertySelector));

            var targetAccessor = Accessor.Member(targetPropertySelector);
            var member = BindingUtility.FindMember(propertySelector);
            var accessor = Accessor.Member<TValue>(member);
            return Bind(target, targetAccessor, targetAccessor.MemberInfo.Name, source, accessor, accessor.MemberInfo.Name, mode);
        }

        public static BindingBase Bind<TTarget, TSource, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, TSource source, IAccessor<TValue> accessor, string propertyName, BindingMode mode = BindingMode.OneWay)
        {
            if (targetPropertySelector == null) throw new ArgumentNullException(nameof(targetPropertySelector));

            var targetAccessor = Accessor.Member(targetPropertySelector);
            return Bind(target, targetAccessor, targetAccessor.MemberInfo.Name, source, accessor, propertyName, mode);
        }


        #endregion


        #region Target INotifyValueChanged

        private static IAccessor<TValue> GetTargetAccessorWithINotifyValueChanged<TValue>(object target)
        {
            var targetNotifyValue = target as INotifyValueChanged<TValue>;
            if (targetNotifyValue == null)
                throw new ArgumentException("Not implemented INotifyValueChanged<T>", nameof(target));

            IAccessor<TValue> targetAccessor;

            targetAccessor = INotifyValueChangedAccessor<TValue>.instanceWithoutNotify;

            return targetAccessor;
        }


        /// <summary>
        /// <paramref name="target"/> 绑定到 <see cref="INotifyValueChanged{T}.value"/> 属性
        /// </summary>
        public static BindingBase Bind<TValue>(this INotifyValueChanged<TValue> target, object source, string path, BindingMode mode = BindingMode.TwoWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (path == null) throw new ArgumentNullException(nameof(path));

            var targetAccessor = GetTargetAccessorWithINotifyValueChanged<TValue>(target);

            Binding binding = new Binding(target, targetAccessor, source, path);
            binding.Mode = mode;
            binding.TargetNotifyValueChangedEnabled = true;

            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                bindable.binding = binding;
            }
            return binding;
        }

        /// <summary>
        /// <paramref name="target"/> 绑定到 <see cref="INotifyValueChanged{T}.value"/> 属性
        /// </summary>
        public static BindingBase Bind<TSource, TValue>(this INotifyValueChanged<TValue> target, TSource source, IAccessor<TValue> accessor, string propertyName, BindingMode mode = BindingMode.TwoWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));

            var targetAccessor = GetTargetAccessorWithINotifyValueChanged<TValue>(target);

            var binding = new PropertyBinding(target, targetAccessor, source, accessor);
            binding.Mode = mode;
            binding.PropertyName = propertyName;
            binding.TargetNotifyValueChangedEnabled = true;

            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                bindable.binding = binding;
            }
            return binding;
        }

        /// <summary>
        /// <paramref name="target"/> 绑定到 <see cref="INotifyValueChanged{T}.value"/> 属性
        /// </summary>
        public static BindingBase Bind<TSource, TValue>(this INotifyValueChanged<TValue> target, TSource source, Expression<Func<TSource, TValue>> propertySelector, BindingMode mode = BindingMode.TwoWay)
        {
            if (propertySelector == null) throw new ArgumentNullException(nameof(propertySelector));
            var member = BindingUtility.FindMember(propertySelector);
            var accessor = Accessor.Member<TValue>(member);
            return Bind(target, source, accessor, accessor.MemberInfo.Name, mode);
        }

        #endregion


        public static void BindAll(this VisualElement root, object source = null)
        {
            root.Query<BindableElement>().Build().ForEach(target =>
            {
                if (target.binding != null)
                {
                    var binding = target.binding as BindingBase;
                    if (binding != null && !binding.IsBinding)
                    {
                        binding.Bind();
                    }
                }
                else if (!string.IsNullOrEmpty(target.bindingPath) && source != null)
                {
                    var type = FindGenericTypeDefinition(target.GetType(), typeof(INotifyValueChanged<>));
                    if (type != null)
                    {
                        Type valueType = type.GenericTypeArguments[0];
                        var method = typeof(Extensions).GetMethod(nameof(GetTargetAccessorWithINotifyValueChanged), BindingFlags.NonPublic | BindingFlags.Static);
                        var targetAccessor = (IAccessor)method.MakeGenericMethod(valueType).Invoke(null, new object[] { target });

                        var binding = new Binding(target, targetAccessor, source, target.bindingPath);
                        binding.TargetNotifyValueChangedEnabled = true;
                        binding.Bind();
                        target.binding = binding;
                    }
                }
            });
        }

        public static void UnbindAll(this VisualElement root)
        {
            root.Query<BindableElement>().Build().ForEach(o =>
            {
                if (o.binding != null)
                {
                    var binding = o.binding as BindingBase;
                    if (binding != null)
                    {
                        binding.Unbind();
                    }
                }
            });
        }

        internal static IAccessor GetAccessor(this PropertyInfo property)
        {
            return Accessor.Member(property);
        }

        internal static IAccessor GetAccessor(this FieldInfo field)
        {
            return Accessor.Member(field);
        }

        internal static Type FindGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
            Type result = null;

            if (genericTypeDefinition.IsInterface)
            {
                foreach (var t in type.GetInterfaces())
                {
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                    {
                        result = t;
                        break;
                    }
                }
            }
            else
            {
                Type t = type;
                while (t != null)
                {
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                    {
                        result = t;
                        break;
                    }
                    t = t.BaseType;
                }
            }
            return result;
        }

        #region PropertyChangedEventHandler

        public static void Invoke(this PropertyChangedEventHandler propertyChanged, object sender, string propertyName)
        {
            if (propertyChanged == null) return;
            var e = PoolPropertyChangedEventArgs.Get(propertyName);
            propertyChanged?.Invoke(sender, e);
            PoolPropertyChangedEventArgs.Release(e);
        }

        public static void Invoke<T>(this PropertyChangedEventHandler propertyChanged, object sender, string propertyName, ref T field, T newValue)
        {
            if (Equals(field, newValue)) return;
            field = newValue;
            if (propertyChanged == null) return;
            Invoke(propertyChanged, sender, propertyName);
        }

        class PoolPropertyChangedEventArgs : PropertyChangedEventArgs
        {
            private string propertyName;

            public PoolPropertyChangedEventArgs(string propertyName) : base(propertyName)
            {
                this.propertyName = propertyName;
            }

            public override string PropertyName => propertyName;


            static Queue<PoolPropertyChangedEventArgs> pool = new Queue<PoolPropertyChangedEventArgs>();
            public static PoolPropertyChangedEventArgs Get(string propertyName)
            {
                PoolPropertyChangedEventArgs item;
                if (pool.Count > 0)
                {
                    item = pool.Dequeue();
                    item.propertyName = propertyName;
                }
                else
                {
                    item = new PoolPropertyChangedEventArgs(propertyName);
                }
                return item;
            }

            public static void Release(PoolPropertyChangedEventArgs e)
            {
                pool.Enqueue(e);
            }
        }

        #endregion
    }


}