using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    public class BindingBuilder<TTarget, TSource>
    {

        private TTarget target;
        private IAccessor targetAccessor;
        private string targetPropertyName;
        private TSource source;
        private IAccessor accessor;
        private string path;
        private string propertyName;

        /// <summary>
        /// 绑定模式
        /// </summary>
        private BindingMode? mode;

        private BindingNotifyDelegate targetNotifyCallback;

        private bool? TargetNotifyValueChangedEnabled;

        /// <summary>
        /// 源到目标是否触发事件，如：<see cref="ChangeEvent{T}"/>
        /// </summary>
        private bool? sourceToTargetNotifyEnabled;

        private BindingNotifyDelegate sourceNotifyCallback;



        public BindingBuilder(TTarget target, TSource source)
        {
            this.target = target;
            this.source = source;
        }

        public BindingBuilder<TTarget, TSource> OneWay()
        {
            mode = BindingMode.OneWay;
            return this;
        }

        public BindingBuilder<TTarget, TSource> OneWayToSource()
        {
            mode = BindingMode.OneWayToSource;
            return this;
        }

        public BindingBuilder<TTarget, TSource> TwoWay()
        {
            mode = BindingMode.TwoWay;
            return this;
        }

        public BindingBuilder<TTarget, TSource> To(IAccessor targetAccessor)
        {
            this.targetAccessor = targetAccessor;
            return this;
        }

        public BindingBuilder<TTarget, TSource> To<TValue>(Expression<Func<TTarget, TValue>> targetPropertySelector)
        {
            var member = BindingUtility.FindMember(targetPropertySelector);
            targetPropertyName = member.Name;
            targetAccessor = Accessor.Member(member);
            return this;
        }

        public BindingBuilder<TTarget, TSource> From(string path)
        {
            this.path = path;
            accessor = null;
            propertyName = null;
            return this;
        }


        public BindingBuilder<TTarget, TSource> From(IAccessor accessor)
        {
            path = null;
            this.accessor = accessor;
            return this;
        }

        public BindingBuilder<TTarget, TSource> From<TValue>(Expression<Func<TSource, TValue>> propertySelector)
        {
            var member = BindingUtility.FindMember(propertySelector);
            propertyName = member.Name;
            accessor = Accessor.Member(member);
            return From(accessor);
        }
        public BindingBuilder<TTarget, TSource> From<TValue>(Expression<Func<TValue>> propertySelector)
        {
            var member = BindingUtility.FindMember(propertySelector);
            propertyName = member.Name;
            accessor = Accessor.Member(member);
            return From(accessor);
        }

        public BindingBuilder<TTarget, TSource> Property(string propertyName)
        {
            this.propertyName = propertyName;
            return this;
        }

        public BindingBuilder<TTarget, TSource> EnableSourceToTargetNotify()
        {
            sourceToTargetNotifyEnabled = true;
            return this;
        }

        public BindingBuilder<TTarget, TSource> DisableSourceToTargetNotify()
        {
            sourceToTargetNotifyEnabled = false;
            return this;
        }

        public BindingBuilder<TTarget, TSource> TargetNotify(BindingNotifyDelegate targetNotifyCallback)
        {
            this.targetNotifyCallback = targetNotifyCallback;
            return this;
        }
        public BindingBuilder<TTarget, TSource> SourceNotify(BindingNotifyDelegate sourceNotifyCallback)
        {
            this.sourceNotifyCallback = sourceNotifyCallback;
            return this;
        }

        public BindingBase Build()
        {
            var targetAccessor = this.targetAccessor;
            bool isINotifyValueChangedAccessor = false;
            if (targetAccessor == null)
            {
                ///目标对象默认绑定属性
                var targetNotifyValueType = target.GetType().FindGenericTypeDefinition(typeof(INotifyValueChanged<>));
                if (targetNotifyValueType != null)
                {
                    targetAccessor = (IAccessor)GetType().GetMethod(nameof(GetINotifyValueChangedAccessor), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                          .MakeGenericMethod(targetNotifyValueType.GetGenericArguments()[0])
                          .Invoke(this, null);
                    isINotifyValueChangedAccessor = true;
                }
                else
                {
                    throw new ArgumentException("Not implemented INotifyValueChanged<T>", nameof(target));
                }
            }

            BindingBase bindingBase;
            if (!string.IsNullOrEmpty(path))
            {
                Binding binding = new Binding(target, targetAccessor, source, path);
                bindingBase = binding;
            }
            else if (accessor != null)
            {
                PropertyBinding binding = new PropertyBinding(target, targetAccessor, source, accessor);
                binding.PropertyName = propertyName;
                bindingBase = binding;
            }
            else
            {
                throw new Exception("Path or Accessor null");
            }

            bindingBase.TargetPropertyName = targetPropertyName;
            bindingBase.TargetNotifyValueChangedEnabled = isINotifyValueChangedAccessor;

            if (mode.HasValue)
            {
                bindingBase.Mode = mode.Value;
            }
            else
            {
                if (isINotifyValueChangedAccessor)
                    bindingBase.Mode = BindingMode.TwoWay;
            }

            if (targetNotifyCallback != null)
                bindingBase.TargetNotifyCallback = targetNotifyCallback;
            if (sourceNotifyCallback != null)
                bindingBase.SourceNotifyCallback = sourceNotifyCallback;
            if (TargetNotifyValueChangedEnabled.HasValue)
                bindingBase.TargetNotifyValueChangedEnabled = TargetNotifyValueChangedEnabled.Value;

            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                bindable.binding = bindingBase;
            }
            return bindingBase;
        }

        INotifyValueChangedAccessor<TValue> GetINotifyValueChangedAccessor<TValue>()
        {
            INotifyValueChangedAccessor<TValue> accessor;
            if (sourceToTargetNotifyEnabled.HasValue && sourceToTargetNotifyEnabled.Value)
                accessor = INotifyValueChangedAccessor<TValue>.instance;
            else
                accessor = INotifyValueChangedAccessor<TValue>.instanceWithoutNotify;
            return accessor;
        }

    }
}