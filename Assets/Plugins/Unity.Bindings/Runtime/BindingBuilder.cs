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
        private TSource source;

        public BindingBuilder(TTarget target, TSource source)
        {
            this.target = target;
            this.source = source;
        }

        internal Action<BindingBase> Created { get; set; }

        public TTarget Target => target;

        public IAccessor TargetAccessor { get; set; }

        private string targetPropertyName { get; set; }

        public TSource Source => source;

        public IAccessor Accessor { get; set; }

        private string SourcePropertyName { get; set; }


        public string Path { get; set; }

        /// <summary>
        /// 绑定模式
        /// </summary>
        public BindingMode? Mode { get; set; }

        public BindingNotifyDelegate TargetNotifyCallback { get; set; }

        public bool? TargetNotifyValueChangedEnabled { get; set; }

        /// <summary>
        /// 源到目标是否触发事件，如：<see cref="ChangeEvent{T}"/>
        /// </summary>
        public bool? SourceToTargetNotifyEnabled { get; set; }

        public BindingNotifyDelegate SourceNotifyCallback { get; set; }

        public BindingBuilder<TTarget, TSource> OneWay()
        {
            Mode = BindingMode.OneWay;
            return this;
        }

        public BindingBuilder<TTarget, TSource> OneWayToSource()
        {
            Mode = BindingMode.OneWayToSource;
            return this;
        }

        public BindingBuilder<TTarget, TSource> TwoWay()
        {
            Mode = BindingMode.TwoWay;
            return this;
        }

        public BindingBuilder<TTarget, TSource> To(IAccessor targetAccessor)
        {
            this.TargetAccessor = targetAccessor;
            return this;
        }

        public BindingBuilder<TTarget, TSource> To<TValue>(Expression<Func<TTarget, TValue>> targetPropertySelector)
        {
            var member = BindingUtility.GetMember(targetPropertySelector);
            targetPropertyName = member.Name;
            TargetAccessor = Yanmonet.Accessor.Member(member);
            return this;
        }

        public BindingBuilder<TTarget, TSource> To<TValue>(Func<TTarget, TValue> getter, Action<TTarget, TValue> setter)
        {
            return To(new Accessor<TTarget, TValue>(getter, setter));
        }

        public BindingBuilder<TTarget, TSource> From(string path)
        {
            this.Path = path;
            Accessor = null;
            SourcePropertyName = null;
            return this;
        }


        public BindingBuilder<TTarget, TSource> From(IAccessor accessor)
        {
            Path = null;
            this.Accessor = accessor;
            return this;
        }

        public BindingBuilder<TTarget, TSource> From<TValue>(Expression<Func<TSource, TValue>> propertySelector)
        {
            var member = BindingUtility.GetMember(propertySelector);
            SourcePropertyName = member.Name;
            Accessor = Yanmonet.Accessor.Member(member);
            return From(Accessor);
        }
        public BindingBuilder<TTarget, TSource> From<TValue>(Expression<Func<TValue>> propertySelector)
        {
            var member = BindingUtility.GetMember(propertySelector);
            SourcePropertyName = member.Name;
            Accessor = Yanmonet.Accessor.Member(member);
            return From(Accessor);
        }

        public BindingBuilder<TTarget, TSource> From<TValue>(Func<TSource, TValue> getter, Action<TSource, TValue> setter)
        {
            return From(new Accessor<TSource, TValue>(getter, setter));
        }

        public BindingBuilder<TTarget, TSource> TargetPropertyName(string targetPropertyName)
        {
            this.targetPropertyName = targetPropertyName;
            return this;
        }
        public BindingBuilder<TTarget, TSource> PropertyName(string propertyName)
        {
            this.SourcePropertyName = propertyName;
            return this;
        }

        public BindingBuilder<TTarget, TSource> EnableSourceToTargetNotify()
        {
            SourceToTargetNotifyEnabled = true;
            return this;
        }

        public BindingBuilder<TTarget, TSource> DisableSourceToTargetNotify()
        {
            SourceToTargetNotifyEnabled = false;
            return this;
        }

        public BindingBuilder<TTarget, TSource> TargetNotify(BindingNotifyDelegate targetNotifyCallback)
        {
            this.TargetNotifyCallback = targetNotifyCallback;
            return this;
        }
        public BindingBuilder<TTarget, TSource> SourceNotify(BindingNotifyDelegate sourceNotifyCallback)
        {
            this.SourceNotifyCallback = sourceNotifyCallback;
            return this;
        }

        public BindingBase Build()
        {
            var targetAccessor = this.TargetAccessor;
            bool isTargetINotifyValueChangedAccessor = false;
            if (targetAccessor == null)
            {
                ///目标对象默认绑定属性
                var targetNotifyValueType = target.GetType().FindGenericTypeDefinition(typeof(INotifyValueChanged<>));
                if (targetNotifyValueType != null)
                {
                    targetAccessor = (IAccessor)GetType().GetMethod(nameof(GetINotifyValueChangedAccessor), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                          .MakeGenericMethod(targetNotifyValueType.GetGenericArguments()[0])
                          .Invoke(this, null);
                    isTargetINotifyValueChangedAccessor = true;
                }
                else
                {
                    throw new ArgumentException("Not implemented INotifyValueChanged<T>", nameof(target));
                }
            }
            else
            {
                if (targetAccessor is INotifyValueChangedAccessor)
                {
                    isTargetINotifyValueChangedAccessor = true;
                }
            }

            BindingBase bindingBase;
            if (!string.IsNullOrEmpty(Path))
            {
                Binding binding = new Binding(target, targetAccessor, source, Path);
                bindingBase = binding;
            }
            else if (Accessor != null)
            {
                PropertyBinding binding = new PropertyBinding(target, targetAccessor, source, Accessor);
                binding.PropertyName = SourcePropertyName;
                bindingBase = binding;
            }
            else
            {
                throw new Exception("Path or Accessor null");
            }

            bindingBase.TargetPropertyName = targetPropertyName;
            bindingBase.TargetNotifyValueChangedEnabled = isTargetINotifyValueChangedAccessor;

            if (Mode.HasValue)
            {
                bindingBase.Mode = Mode.Value;
            }
            else
            {
                if (isTargetINotifyValueChangedAccessor)
                    bindingBase.Mode = BindingMode.TwoWay;
            }

            if (TargetNotifyCallback != null)
                bindingBase.TargetNotifyCallback = TargetNotifyCallback;
            if (SourceNotifyCallback != null)
                bindingBase.SourceNotifyCallback = SourceNotifyCallback;
            if (TargetNotifyValueChangedEnabled.HasValue)
                bindingBase.TargetNotifyValueChangedEnabled = TargetNotifyValueChangedEnabled.Value;

            if (targetAccessor != null && targetAccessor is INotifyValueChangedAccessor)
            {
                IBindable bindable = target as IBindable;
                if (bindable != null)
                {
                    bindable.binding = bindingBase;
                }
            }

            Created?.Invoke(bindingBase);

            return bindingBase;
        }

        INotifyValueChangedAccessor<TValue> GetINotifyValueChangedAccessor<TValue>()
        {
            INotifyValueChangedAccessor<TValue> accessor;
            if (SourceToTargetNotifyEnabled.HasValue && SourceToTargetNotifyEnabled.Value)
                accessor = INotifyValueChangedAccessor<TValue>.instance;
            else
                accessor = INotifyValueChangedAccessor<TValue>.instanceWithoutNotify;
            return accessor;
        }

    }
}