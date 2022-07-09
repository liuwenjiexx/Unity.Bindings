using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    public abstract class BindingBase : IBinding
    {
        private bool isBinding;
        private object target;
        private IAccessor targetAccessor;
        private object source;


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
        /// <summary>
        /// 绑定属性名称，监听属性值改变事件
        /// </summary>
        public string TargetPropertyName { get; set; }

        public bool TargetNotifyValueChangedEnabled { get; set; }

        public object Source
        {
            get => source;
            set => source = value;
        }
        /// <summary>
        /// 绑定属性名称，监听属性值改变事件
        /// </summary>
        public string PropertyName { get; set; }

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

        public BindingNotifyDelegate TargetNotifyCallback { get; set; }

        public BindingNotifyDelegate SourceNotifyCallback { get; set; }
         
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
                if (!TargetSupportNotify && TargetNotifyCallback != null && !string.IsNullOrEmpty(TargetPropertyName))
                {
                    TargetNotifyCallback(OnTargetPropertyChanged, true);
                    TargetSupportNotify = true;
                }

                if (!TargetSupportNotify && TargetNotifyValueChangedEnabled)
                {
                    var notifyType = Target.GetType().FindGenericTypeDefinition(typeof(INotifyValueChanged<>));
                    if (notifyType != null)
                    {
                        BindingUtility.RegisterValueChangedCallback(notifyType.GetGenericArguments()[0], Target, OnTargetValueChanged);
                        TargetSupportNotify = true;
                    }
                }
            }


            if (CanUpdateSourceToTarget)
            {
                if (!SourceSupportNotify && SourceNotifyCallback != null && !string.IsNullOrEmpty(PropertyName))
                {
                    SourceNotifyCallback(OnSourcePropertyChanged, true);
                    SourceSupportNotify = true;
                }
            }

        }



        public virtual void Unbind()
        {
            if (!isBinding)
                return;
            isBinding = false;

            if (TargetNotifyCallback != null)
            {
                TargetNotifyCallback(OnTargetPropertyChanged, false);
            }
            else
            {
                var notifyType = Target.GetType().FindGenericTypeDefinition(typeof(INotifyValueChanged<>));
                if (notifyType != null)
                {
                    BindingUtility.UnregisterValueChangedCallback(notifyType.GetGenericArguments()[0], Target, OnTargetValueChanged);
                }

            }
        }

        protected virtual object GetTargetValue()
        {
            var value = targetAccessor.GetValue(Target);
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

    public delegate void BindingNotifyDelegate(PropertyChangedEventHandler handler, bool add);

}