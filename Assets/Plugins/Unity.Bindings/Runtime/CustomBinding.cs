using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.ComponentModel;
using System.Reflection;

namespace UnityEngine.Bindings
{
    class CustomBinding<TValue> : IBinding, IDisposable
    {
        public object target;

        public INotifyValueChanged<TValue> targetAccessor;
        public INotifyValueChanged<TValue> TargetAccessor { get => targetAccessor; }
        public object source;
        public string propertyName;
        private bool targetSupportNotify;
        private bool sourceSupportNotify;
        private bool disposed;

        public string PropertyName { get => propertyName; }
        public BindingMode mode;

        protected bool CanGetSourceValue
        {
            get => false;
        }

        protected bool CanUpdateTargetToSource
        {
            get => mode != BindingMode.OneWay;
        }

        protected bool CanUpdateSourceToTarget
        {
            get => mode != BindingMode.OneWayToSource;
        }

        internal void Bind()
        {
            sourceSupportNotify = false;
            targetSupportNotify = false;

            if (CanUpdateSourceToTarget)
            {
                if (source != null)
                {
                    if (source is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged)source).PropertyChanged += OnSourcePropertyChanged;
                        sourceSupportNotify = true;
                    }
                }
            }

            if (CanUpdateTargetToSource)
            {
                if (targetAccessor == target)
                {
                    targetAccessor.RegisterValueChangedCallback(OnTargetValueChanged);
                    targetSupportNotify = true;
                }
            }


            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                bindable.binding = this;
            }

            if (mode == BindingMode.OneWayToSource)
            {
                UpdateTargetToSource();
            }
            else
            {
                UpdateSourceToTarget();
            }
        }

        public void Unbind()
        {
            if (targetAccessor != null)
            {
                targetAccessor.UnregisterValueChangedCallback(OnTargetValueChanged); 
            }
            if (source != null)
            {
                if (source is INotifyPropertyChanged)
                    ((INotifyPropertyChanged)source).PropertyChanged -= OnSourcePropertyChanged; 
            }

            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                if (bindable.binding == this)
                {
                    bindable.binding = null;
                }
            }

        }
        private void OnTargetValueChanged(ChangeEvent<TValue> e)
        {
            if (CanUpdateTargetToSource)
            {
                SetSourceValue(e.newValue);
            }
        }
        private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == propertyName)
            {

                if (CanUpdateSourceToTarget)
                {
                    TValue value = GetSourceValue();
                    targetAccessor.value = value;
                }
            }
        }

        public void PreUpdate()
        {

        }
        public void Release()
        {
            if (disposed)
                return;
             
        }

        public void Update()
        {
            if (disposed)
                return;
            if (CanUpdateSourceToTarget && !sourceSupportNotify)
            {
                UpdateSourceToTarget();
            }

            if (CanUpdateTargetToSource && !targetSupportNotify)
            {
                UpdateTargetToSource();
            }
        }

        protected virtual void UpdateSourceToTarget()
        {
            var value = GetSourceValue();
            if (!object.Equals(targetAccessor.value, value))
            {
                targetAccessor.value = value;
            }
        }

        protected virtual void UpdateTargetToSource()
        {
            if (target != targetAccessor)
            {
                var value = targetAccessor.value;
                if (!object.Equals(GetSourceValue(), value))
                {
                    SetSourceValue(value);
                }
            }
        }

        protected virtual TValue GetSourceValue() { throw new NotImplementedException(); }

        protected virtual void SetSourceValue(TValue value) { throw new NotImplementedException(); }

        public void Dispose()
        {
            if (disposed)
                return;
            Unbind();
            disposed = true;
            propertyName = null;

        }

        internal class DelegateAccessor : IPropertyAccessor
        {
            private Action<string, TValue> setter;
            private Func<string, TValue> getter;

            public DelegateAccessor(Action<string, TValue> setter, Func<string, TValue> getter)
            {
                this.setter = setter;
                this.getter = getter;
            }



            public bool CanReadProperty(string propertyName)
            {
                return getter != null;
            }

            public bool CanWriteProperty(string propertyName)
            {
                return setter != null;
            }

            public object GetPropertyValue(string propertyName)
            {
                return getter(propertyName);
            }

            public void SetPropertyValue(string propertyName, object value)
            {
                setter(propertyName, (TValue)value);
            }
        }


        internal class AccessorPropertyBinding : CustomBinding<TValue>
        {
            public IPropertyAccessor accessor;



            protected override TValue GetSourceValue()
            {
                return (TValue)accessor.GetPropertyValue(propertyName);
            }

            protected override void SetSourceValue(TValue value)
            {
                accessor.SetPropertyValue(propertyName, value);
            }
        }
        internal class AccessorPropertyBinding2 : CustomBinding<TValue>
        {
            public INotifyValueChanged<TValue> accessor;

            protected override TValue GetSourceValue()
            {
                return accessor.value;
            }

            protected override void SetSourceValue(TValue value)
            {
                accessor.value = value;
            }
        }
        class PropertyAccessor : INotifyValueChanged<TValue>
        {
            private PropertyInfo property;
            private MethodInfo setter;
            private MethodInfo getter;
            public object Target;


            public PropertyInfo Property
            {
                get
                {
                    return property;
                }
                set
                {
                    property = value;
                    setter = null;
                    getter = null;
                    if (property != null)
                    {
                        getter = property.GetGetMethod(true);
                        setter = property.GetSetMethod(true);
                    }
                }
            }

            public TValue value { get => (TValue)getter.Invoke(Target, null); set => setter.Invoke(Target, new object[] { value }); }

            public void SetValueWithoutNotify(TValue newValue)
            {
                this.value = newValue;
            }
        }
        class FieldAccessor : INotifyValueChanged<TValue>
        {
            private FieldInfo field;
            public object Target;

            public FieldInfo Field { get => field; set => field = value; }
            public TValue value { get => (TValue)field.GetValue(Target); set => field.SetValue(Target, value); }


            public void SetValueWithoutNotify(TValue newValue)
            {
                this.value = newValue;
            }
        }



    }
    internal interface IPropertyAccessor
    {
        bool CanReadProperty(string propertyName);

        bool CanWriteProperty(string propertyName);

        object GetPropertyValue(string propertyName);

        void SetPropertyValue(string propertyName, object value);
    }


}