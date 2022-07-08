using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Yanmonet.Bindings
{
    class PropertyBinding<TValue> : BindingBase
    {
        private string propertyName;
        private IAccessor<TValue> accessor;

        public PropertyBinding(object target, IAccessor<TValue> targetAccessor, object source, string propertyName, IAccessor<TValue> accessor)
            : base(target, targetAccessor)
        {
            this.Source = source;
            this.propertyName = propertyName;
            this.accessor = accessor;
        }

        /// <summary>
        /// 绑定属性名称，监听属性值改变事件
        /// </summary>
        protected override string PropertyName { get => propertyName; }

        protected virtual TValue GetSourceValue()
        {
            var value = accessor.GetValue(Source);
            return value;
        }

        protected virtual void SetSourceValue(TValue value)
        {
            accessor.SetValue(Source, value);
        }

        public override void Bind()
        {
            if (IsBinding)
                return;

            base.Bind();

            if (CanUpdateSourceToTarget)
            {
                if (Source != null)
                {
                    if (!SourceSupportNotify && Source is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged)Source).PropertyChanged += OnSourcePropertyChanged;
                        SourceSupportNotify = true;
                    }
                }
            }


            if (Mode == BindingMode.OneWayToSource)
            {
                if (CanUpdateTargetToSource)
                    UpdateTargetToSource();
            }
            else
            {
                if (CanUpdateSourceToTarget)
                    UpdateSourceToTarget();
            }
        }

        public override void Unbind()
        {

            if (SourceNotify != null)
            {
                SourceNotify(OnSourcePropertyChanged, false);
            }
            else if (Source != null)
            {
                if (Source is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)Source).PropertyChanged -= OnSourcePropertyChanged;
            }

            base.Unbind();
        }


        protected override void UpdateSourceToTarget()
        {
            if (!accessor.CanGetValue(Source) || !TargetAccessor.CanSetValue(Target))
                return;
            var value = GetSourceValue();
            if (!object.Equals(GetTargetValue(), value))
            {
                SetTargetValue(value);
            }
        }

        protected override void UpdateTargetToSource()
        {
            if (!accessor.CanSetValue(Source) || !TargetAccessor.CanGetValue(Target))
                return;
            var value = GetTargetValue();
            if (!object.Equals(GetSourceValue(), value))
            {
                SetSourceValue((TValue)value);
            }
        }

    }

}