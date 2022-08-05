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
        private PropertyBinder targetBinder;
        private string targetPath;
        private object source;


        public BindingBase(object target, IAccessor targetAccessor)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));

            this.target = target;
            this.targetAccessor = targetAccessor;

        }

        public BindingBase(object target, string targetPath)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetPath == null) throw new ArgumentNullException(nameof(targetPath));
            this.target = target;
            this.targetPath = targetPath;
        }


        public bool IsBinding => isBinding;

        public object Target => target;

        public IAccessor TargetAccessor => targetAccessor;
        /// <summary>
        /// ���������ƣ���������ֵ�ı��¼�
        /// </summary>
        public string TargetPropertyName { get; set; }

        public bool TargetNotifyValueChangedEnabled { get; set; }

        public object Source
        {
            get => source;
            set => source = value;
        }
        /// <summary>
        /// ���������ƣ���������ֵ�ı��¼�
        /// </summary>
        public string PropertyName { get; set; }

        public BindingMode Mode
        {
            get; set;
        } = BindingMode.OneWay;

        protected bool TargetSupportNotify { get; set; }
        protected bool SourceSupportNotify { get; set; }



        public virtual bool CanUpdateTargetToSource
        {
            get => Mode != BindingMode.OneWay;
        }

        public virtual bool CanUpdateSourceToTarget
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
            if (targetAccessor == null)
            {
                targetBinder = PropertyBinder.Create(targetPath);
                targetBinder.Target = target;
            }

            if (CanUpdateTargetToSource)
            {
                if (!TargetSupportNotify && TargetNotifyCallback != null && !string.IsNullOrEmpty(TargetPropertyName))
                {
                    TargetNotifyCallback(OnTargetPropertyChanged, true);
                    TargetSupportNotify = true;
                }
                if (TargetNotifyValueChangedEnabled)
                {
                    if (!TargetSupportNotify && targetBinder != null && targetBinder.SupportNotify)
                    {
                        targetBinder.TargetUpdatedCallback = () =>
                        {
                            if (CanUpdateTargetToSource)
                                UpdateTargetToSource();
                        };
                        TargetSupportNotify = true;
                    }

                    if (!TargetSupportNotify)
                    {
                        var notifyType = Target.GetType().FindGenericTypeDefinition(typeof(INotifyValueChanged<>));
                        if (notifyType != null)
                        {
                            BindingUtility.RegisterValueChangedCallback(notifyType.GetGenericArguments()[0], Target, OnTargetValueChanged);
                            TargetSupportNotify = true;
                        }
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

            if (targetBinder != null)
            {
                targetBinder.TargetUpdatedCallback = null;
                targetBinder.Dispose();
                targetBinder = null;
            }

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
            object value;
            if (targetAccessor != null)
            {
                value = targetAccessor.GetValue(Target);
            }
            else
            {
                if (!targetBinder.TryGetTargetValue(out value))
                    return null;
            }
            return value;
        }

        protected virtual void SetTargetValue(object value)
        {
            if (targetAccessor != null)
            {
                targetAccessor.SetValue(Target, value);
            }
            else
            {
                targetBinder.TrySetTargetValue(value);
            }
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

        public abstract void UpdateSourceToTarget();

        public abstract void UpdateTargetToSource();

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