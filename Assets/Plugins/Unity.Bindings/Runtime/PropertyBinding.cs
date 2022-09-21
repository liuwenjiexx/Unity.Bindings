using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using YMFramework;

namespace Yanmonet.Bindings
{
    class PropertyBinding : BindingBase
    {
        private IAccessor accessor;

        public PropertyBinding(object target, IAccessor targetAccessor, object source, IAccessor accessor)
            : base(target, targetAccessor)
        {
            this.Source = source;
            this.accessor = accessor;
        }



        protected virtual object GetSourceValue()
        {
            var value = accessor.GetValue(Source);
            return value;
        }

        protected virtual bool SetSourceValue(object value)
        {
            var origin = GetSourceValue();
            if (!object.Equals(origin, value))
            {
                accessor.SetValue(Source, value);
                return true;
            }
            return false;
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
                    if (!SourceSupportNotify && Source is INotifyPropertyChanged && !string.IsNullOrEmpty(PropertyName))
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

            if (SourceNotifyCallback != null)
            {
                SourceNotifyCallback(OnSourcePropertyChanged, false);
            }
            else if (Source != null)
            {
                if (Source is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)Source).PropertyChanged -= OnSourcePropertyChanged;
            }

            base.Unbind();
        }


        public override void UpdateSourceToTarget()
        {
            if (!accessor.CanGetValue(Source) || !TargetAccessor.CanSetValue(Target))
                return;
            var value = GetSourceValue();

            var converter = Converter;
            if (converter != null)
            {
                Type valueType = GetTargetValueType();
                if (valueType != null)
                {
                    value = converter.Convert(value, valueType, ConverterParameter);
                }
            }

            SetTargetValue(value);
        }

        public override void UpdateTargetToSource()
        {
            if (!accessor.CanSetValue(Source) || !TargetAccessor.CanGetValue(Target))
                return;
            var value = GetTargetValue();
            if (value == UnsetValue) return;

            var converter = Converter;
            if (converter != null)
            {
                Type valueType;
                valueType =accessor.ValueType;
                if (valueType != null)
                {
                    value = converter.ConvertBack(value, valueType, ConverterParameter);
                }
            }
            if (value == UnsetValue) return;

            SetSourceValue(value);
        }

    }

}