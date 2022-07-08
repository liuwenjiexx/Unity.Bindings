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
        #region Binding Path

        /// <summary>
        /// 绑定属性路径
        /// </summary>
        public static BindingBase BindPath<TValue>(this object target, IAccessor<TValue> targetAccessor, object source, string path, BindingOptions options = null)
        {

            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (path == null) throw new ArgumentNullException(nameof(path));

            Binding binding = new Binding(target, targetAccessor, source, path);
            binding.ApplyOptions(options);
            binding.Bind();

            return binding;
        }
        /// <summary>
        /// 绑定属性路径, <paramref name="target"/> 为 <see cref="INotifyValueChanged{T}"/>
        /// </summary>
        public static BindingBase BindPath<TValue>(this object target, object source, string path, BindingOptions options = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (path == null) throw new ArgumentNullException(nameof(path));

            var notify = target as INotifyValueChanged<TValue>;
            if (notify == null)
                throw new ArgumentException("target not INotifyValueChanged", nameof(target));

            IAccessor<TValue> targetAccessor;
            if (options != null)
            {
                if (options.SourceToTargetWithoutNotify.HasValue && options.SourceToTargetWithoutNotify.Value)
                    targetAccessor = NotifyValueChangedAccessor<TValue>.instanceWithoutNotify;
                else
                    targetAccessor = NotifyValueChangedAccessor<TValue>.instance;
            }
            else
            {
                targetAccessor = NotifyValueChangedAccessor<TValue>.instance;
            }

            return BindPath(target, targetAccessor, source, path, options);
        }

        /// <summary>
        /// 绑定属性路径
        /// </summary>
        public static BindingBase BindPath<TTarget, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, object source, string path, BindingOptions options = null)
        {
            IAccessor<TValue> targetAccessor = Accessor.Member(targetPropertySelector);
            return BindPath(target, targetAccessor, source, path, options);
        }


        #endregion

        #region Binding Custom Property

        /// <summary>
        /// 绑定属性
        /// </summary>
        public static BindingBase BindProperty<TSource, TValue>(this object target, IAccessor<TValue> targetAccessor, TSource source, string propertyName, IAccessor<TValue> accessor, BindingOptions options = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));

            var binding = new PropertyBinding<TValue>(target, targetAccessor, source, propertyName, accessor);
             
            binding.ApplyOptions(options);
            binding.Bind();

            return binding;
        }

        public static BindingBase BindProperty<TTarget, TSource, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, TSource source, Expression<Func<TSource, TValue>> propertySelector, BindingOptions options = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetPropertySelector == null) throw new ArgumentNullException(nameof(targetPropertySelector));
            if (source == null) throw new ArgumentNullException(nameof(source));

            var targetAccessor = Accessor.Member(targetPropertySelector);
            var member = propertySelector.FindMember();
            string propertyName = member.Name;
            var accessor = Accessor.Member<TValue>(member);
            return BindProperty(target, targetAccessor, source, propertyName, accessor, options);
        }

        public static BindingBase BindProperty<TTarget, TSource, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, TSource source, string propertyName, IAccessor<TValue> accessor, BindingOptions options = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetPropertySelector == null) throw new ArgumentNullException(nameof(targetPropertySelector));
            if (source == null) throw new ArgumentNullException(nameof(source));

            var targetAccessor = Accessor.Member(targetPropertySelector);
            return BindProperty(target, targetAccessor, source, propertyName, accessor, options);
        }

        /// <summary>
        /// 绑定属性, <paramref name="target"/> 为 <see cref="INotifyValueChanged{T}"/>
        /// </summary>
        public static BindingBase BindProperty<TSource, TValue>(this object target, TSource source, string propertyName, IAccessor<TValue> accessor, BindingOptions options = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var notify = target as INotifyValueChanged<TValue>;
            if (notify == null)
                throw new Exception("target not INotifyValueChanged");
 
            IAccessor<TValue> targetAccessor;
            if (options != null)
            {
                if (options.SourceToTargetWithoutNotify.HasValue && options.SourceToTargetWithoutNotify.Value)
                    targetAccessor = NotifyValueChangedAccessor<TValue>.instanceWithoutNotify;
                else
                    targetAccessor = NotifyValueChangedAccessor<TValue>.instance;
            }
            else
            {
                targetAccessor = NotifyValueChangedAccessor<TValue>.instance;
            }
            return BindProperty(target, targetAccessor, source, propertyName, accessor, options);
        }

        /// <summary>
        /// 绑定属性
        /// </summary>
        public static BindingBase BindProperty<TSource, TValue>(this object target, TSource source, Expression<Func<TSource, TValue>> propertySelector, BindingOptions options = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));
            var member = propertySelector.FindMember();
            string propertyName = member.Name;
            var accessor = Accessor.Member<TValue>(member);
            return BindProperty(target, source, propertyName, accessor, options);
        }


        /// <summary>
        /// 绑定静态属性
        /// </summary>
        public static BindingBase BindProperty<TValue>(this object target, IAccessor<TValue> targetAccessor, string propertyName, IAccessor<TValue> accessor, BindingOptions options = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));

            var binding = new PropertyBinding<TValue>(target, targetAccessor, null, propertyName, accessor);
            binding.ApplyOptions(options);
            binding.Bind();

            return binding;
        }
        /// <summary>
        /// 绑定静态属性, <paramref name="target"/> 为 <see cref="INotifyValueChanged{T}"/>
        /// </summary>
        public static BindingBase BindProperty<TValue>(this object target, Expression<Func<TValue>> propertySelector, BindingOptions options = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (propertySelector == null) throw new ArgumentNullException(nameof(propertySelector));

            var notify = target as INotifyValueChanged<TValue>;
            if (notify == null)
                throw new Exception("target not INotifyValueChanged");
            var member = propertySelector.FindMember();
            string propertyName = member.Name;
            IAccessor<TValue> targetAccessor;
            if (options != null)
            {
                if (options.SourceToTargetWithoutNotify.HasValue && options.SourceToTargetWithoutNotify.Value)
                    targetAccessor = NotifyValueChangedAccessor<TValue>.instanceWithoutNotify;
                else
                    targetAccessor = NotifyValueChangedAccessor<TValue>.instance;
            }
            else
            {
                targetAccessor = NotifyValueChangedAccessor<TValue>.instance;
            }
            var accessor = Accessor.Member<TValue>(member);

            return BindProperty(target, targetAccessor, propertyName, accessor, options);
        } 

        #endregion
         

        public static void BindAll(this VisualElement root)
        {
            root.Query<BindableElement>().Build().ForEach(o =>
            {
                if (o.binding != null)
                {
                    var binding = o.binding as BindingBase;
                    if (binding != null && !binding.IsBinding)
                    {
                        binding.Bind();
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

        internal static MemberInfo FindMember<TTarget, TValue>(this Expression<Func<TTarget, TValue>> selector)
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

        internal static MemberInfo FindMember<TValue>(this Expression<Func<TValue>> selector)
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

    }
}