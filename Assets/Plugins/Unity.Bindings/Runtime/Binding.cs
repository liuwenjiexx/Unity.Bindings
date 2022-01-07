using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace UnityEngine.Bindings
{

    class Binding<TValue> : IBinding, IDisposable
    {
        private object target;
        private INotifyValueChanged<TValue> targetAccessor;
        internal INotifyValueChanged<TValue> TargetAccessor { get => targetAccessor; set => targetAccessor = value; }

        protected object source;
        private PropertyBinder sourceBinder;
        private string path;
        protected bool isSourceNotifyPropertyChanged;
        private bool isNullValue;
        private bool isFallbackValue;
        private string stringFormat;
        private object nullValue;
        private object fallbackValue;

        private bool disposed;

        private BindingMode mode = BindingMode.TwoWay;

        public object Source
        {
            get { return source; }
            set
            {
                if (source != value)
                {
                    source = value;
                    if (sourceBinder != null)
                    {
                        sourceBinder.Target = source;
                        //UpdateSourceToTarget();
                    }

                }
            }
        }
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public BindingMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        public bool IsNullValue { get => isNullValue; set => isNullValue = value; }

        public bool IsFallbackValue { get => isFallbackValue; set => isFallbackValue = value; }

        public string StringFormat { get => stringFormat; set => stringFormat = value; }

        /// <summary>
        /// if source value null then set this value
        /// </summary>
        public object NullValue { get => nullValue; set => nullValue = value; }

        /// <summary>
        /// source or target not value, set this value
        /// </summary>
        public object FallbackValue { get => fallbackValue; set => fallbackValue = value; }


        protected bool CanUpdateTargetToSource
        {
            get => mode != BindingMode.OneWay;
        }

        protected bool CanUpdateSourceToTarget
        {
            get => mode != BindingMode.OneWayToSource;
        }

        private static Dictionary<Type, string> defaultMenbers;

        public Binding(object target)
        {
            this.target = target;
            this.targetAccessor = target as INotifyValueChanged<TValue>;
        }

        public Binding(object target, object source, string path)
        {
            this.target = target;
            this.targetAccessor = target as INotifyValueChanged<TValue>;
            this.source = source;
            this.path = path;
        }

        public virtual void Bind()
        {
            Unbind();

            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                bindable.binding = this;
            }

            if (targetAccessor == null)
                throw new Exception(string.Format("{0} Null", nameof(TargetAccessor)));

            if (CanUpdateTargetToSource)
            {
                targetAccessor.RegisterValueChangedCallback(OnTargetValueChanged);
            }
             
            sourceBinder = PropertyBinder.Create(path);
            sourceBinder.Target = source;

            if (CanUpdateSourceToTarget)
            {
                sourceBinder.TargetUpdatedCallback = UpdateSourceToTarget;
            }

            if (mode == BindingMode.OneWayToSource)
                UpdateTargetToSource();
            else
                UpdateSourceToTarget();
        }


        public virtual void Unbind()
        {
            isSourceNotifyPropertyChanged = false;

            IBindable bindable = target as IBindable;
            if (bindable != null)
            {
                if (bindable.binding == this)
                    bindable.binding = null;
            }

            if (targetAccessor != null)
            {
                targetAccessor.UnregisterValueChangedCallback(OnTargetValueChanged);
            }

            if (sourceBinder != null)
            {
                sourceBinder.Dispose();
                sourceBinder = null;
            }
        }

        protected virtual void UpdateSourceToTarget()
        {
            if (mode == BindingMode.OneWayToSource)
                return;


            object value;

            IsNullValue = false;
            IsFallbackValue = false;

            if (sourceBinder.TryGetValue(out value))
            {
                if (value != null)
                {
                    //if (converter != null)
                    //    value = converter.Convert(value, targetBinder.GetValueType(), converterParameter);
                    string stringFormat = StringFormat;
                    if (!string.IsNullOrEmpty(stringFormat))
                        value = String.Format(stringFormat, value);
                }
                else
                {
                    value = NullValue;
                    IsNullValue = true;
                }
            }
            else
            {
                value = FallbackValue;
                IsFallbackValue = true;
            }

            if (!object.Equals(targetAccessor.value, value))
            {
                targetAccessor.value = (TValue)value;
            }

        }


        protected virtual void UpdateTargetToSource()
        {
            if (!(mode == BindingMode.TwoWay || mode == BindingMode.OneWayToSource))
                return;

            if (!sourceBinder.CanSetValue())
                return;

            object value;

            value = targetAccessor.value;

            //if (converter != null)
            //    value = converter.ConvertBack(value, sourceBinder.GetValueType(), converterParameter);

            if (sourceBinder.TrySetValue(value))
            {
                //if (enabledSourceUpdated)
                //{
                //    var sourceUpdated = SourceUpdated;
                //    if (sourceUpdated != null)
                //    {
                //        var args = new DataTransferEventArgs();
                //        sourceUpdated(this, args);
                //    }
                //}

            }
        }


        public virtual void PreUpdate()
        {

        }

        public virtual void Release()
        {
            if (disposed)
                return;
            Unbind();
        }

        public virtual void Update()
        {
            if (disposed)
                return;
            UpdateSourceToTarget();
        }

        public virtual void Dispose()
        {
            if (disposed)
                return;
            Unbind();
            disposed = true;
        }
        public static string GetDefaultMember(Type type)
        {
            string name;

            if (!defaultMenbers.TryGetValue(type, out name))
            {
                var members = type.GetDefaultMembers();
                if (members != null && members.Length > 0)
                {
                    name = (from m in members
                            orderby m.MemberType == MemberTypes.Property ? 0 : 1
                            orderby m.MemberType == MemberTypes.Field ? 0 : 1
                            select m).First().Name;
                }
                defaultMenbers[type] = name;
            }
            return name;
        }

        public static void SetDefaultMember(Type type, string memberName)
        {
            defaultMenbers[type] = memberName;
        }



        protected virtual void OnTargetValueChanged(ChangeEvent<TValue> e)
        {
            UpdateTargetToSource();
        }


        protected virtual TValue GetSourceValue() { throw new NotImplementedException(); }

        protected virtual void SetSourceValue(TValue value) { throw new NotImplementedException(); }

        ~Binding()
        {
            Dispose();
        }

    }


}