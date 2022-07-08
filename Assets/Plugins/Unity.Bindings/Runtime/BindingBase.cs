using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    public abstract class BindingBase : IBinding
    {
        private bool isBinding;
        private object target;
        private IAccessor targetAccessor;
        private object source;

        private static MethodInfo registerValueChangedCallbackMethod;
        private static MethodInfo unregisterValueChangedCallbackMethod;

        public BindingBase(object target, IAccessor targetAccessor)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null)
                throw new ArgumentNullException(nameof(targetAccessor));
            this.target = target;
            this.targetAccessor = targetAccessor;
        }


        public bool IsBinding => isBinding;

        public object Target => target;

        protected IAccessor TargetAccessor => targetAccessor;

        public string TargetPropertyName { get; set; }

        public bool TargetNotifyValueChangedEnabled { get; set; } = true;

        public object Source
        {
            get => source;
            set => source = value;
        }

        protected virtual string PropertyName { get => null; }

        public BindingMode Mode
        {
            get; set;
        } = BindingMode.TwoWay;

        protected bool TargetSupportNotify { get; set; }
        protected bool SourceSupportNotify { get; set; }



        protected virtual bool CanUpdateTargetToSource
        {
            get => Mode != BindingMode.OneWay;
        }

        protected virtual bool CanUpdateSourceToTarget
        {
            get => Mode != BindingMode.OneWayToSource;
        }

        public SetPropertyChangedDelegate TargetPropertyChanged { get; set; }

        public SetPropertyChangedDelegate SourcePropertyChanged { get; set; }

        public void ApplyOptions(BindingOptions options)
        {
            if (options == null)
                return;
            if (options.Mode.HasValue)
                Mode = options.Mode.Value;
            if (options.TargetPropertyChanged != null)
                TargetPropertyChanged = options.TargetPropertyChanged;
            if (options.SourcePropertyChanged != null)
                SourcePropertyChanged = options.SourcePropertyChanged;
            if (options.TargetNotifyValueChangedEnabled.HasValue)
                TargetNotifyValueChangedEnabled = options.TargetNotifyValueChangedEnabled.Value;
        }

        public virtual void Bind()
        {
            if (isBinding)
            {
                return;
            }
            isBinding = true;

            TargetSupportNotify = false;
            SourceSupportNotify = false;



            if (CanUpdateTargetToSource)
            {
                if (!TargetSupportNotify && TargetPropertyChanged != null)
                {
                    TargetPropertyChanged(OnTargetPropertyChanged, true);
                    TargetSupportNotify = true;
                }

                if (!TargetSupportNotify && TargetNotifyValueChangedEnabled)
                {
                    var notifyType = Target.GetType().FindGenericTypeDefinition(typeof(INotifyValueChanged<>));
                    if (notifyType != null)
                    {
                        if (registerValueChangedCallbackMethod == null)
                        {
                            registerValueChangedCallbackMethod = typeof(INotifyValueChangedExtensions).GetMethod("RegisterValueChangedCallback");
                        }
                        registerValueChangedCallbackMethod.MakeGenericMethod(notifyType.GetGenericArguments()[0])
                            .Invoke(null, new object[] { Target, new EventCallback<IChangeEvent>(OnTargetValueChanged) });
                        TargetSupportNotify = true;
                    }
                }
            }


            if (CanUpdateSourceToTarget)
            {
                if (!SourceSupportNotify && SourcePropertyChanged != null)
                {
                    SourcePropertyChanged(OnSourcePropertyChanged, true);
                    SourceSupportNotify = true;
                }
            }

            IBindable bindable = Target as IBindable;
            if (bindable != null)
            {
                bindable.binding = this;
            }
        }

        public virtual void Unbind()
        {
            if (!isBinding)
                return;
            isBinding = false;

            if (TargetPropertyChanged != null)
            {
                TargetPropertyChanged(OnTargetPropertyChanged, false);
            }
            else
            {
                var notifyType = Target.GetType().FindGenericTypeDefinition(typeof(INotifyValueChanged<>));
                if (notifyType != null)
                {
                    if (unregisterValueChangedCallbackMethod == null)
                    {
                        unregisterValueChangedCallbackMethod = typeof(INotifyValueChangedExtensions).GetMethod("UnregisterValueChangedCallback");
                    }
                    unregisterValueChangedCallbackMethod.MakeGenericMethod(notifyType.GetGenericArguments()[0])
                        .Invoke(null, new object[] { Target, new EventCallback<IChangeEvent>(OnTargetValueChanged) });
                }

            }
        }

        protected virtual object GetTargetValue()
        {
            targetAccessor.GetValue(Target, out var value);
            return value;
        }

        protected virtual void SetTargetValue(object value)
        {
            targetAccessor.SetValue(Target, value);
        }

        public virtual void PreUpdate()
        {

        }

        public virtual void Release()
        {
            if (!IsBinding)
                return;
            Unbind();
        }

        public virtual void Update()
        {
            if (!IsBinding)
                return;
            if (CanUpdateSourceToTarget && !SourceSupportNotify)
            {
                UpdateSourceToTarget();
            }

            if (CanUpdateTargetToSource && !TargetSupportNotify)
            {
                UpdateTargetToSource();
            }
        }

        protected abstract void UpdateSourceToTarget();

        protected abstract void UpdateTargetToSource();

        protected void OnTargetValueChanged(IChangeEvent e)
        {
            if (CanUpdateTargetToSource)
            {
                UpdateTargetToSource();
            }
        }


        protected void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TargetPropertyName)
            {
                if (CanUpdateTargetToSource)
                {
                    UpdateTargetToSource();
                }
            }
        }

        protected void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
            {
                if (CanUpdateSourceToTarget)
                {
                    UpdateSourceToTarget();
                }
            }
        }

    }

    public delegate void SetPropertyChangedDelegate(PropertyChangedEventHandler handler, bool add);

}