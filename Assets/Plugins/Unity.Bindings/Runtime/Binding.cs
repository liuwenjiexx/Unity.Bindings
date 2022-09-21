using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using YMFramework;

namespace Yanmonet.Bindings
{

    class Binding : BindingBase
    {


        private PropertyBinder binder;
        private string path;
        protected bool isSourceNotifyPropertyChanged;
        private bool isNullValue;
        private bool isFallbackValue;
        private string stringFormat;
        private object nullValue;
        private object fallbackValue;


        public Binding(object target, IAccessor targetAccessor)
            : base(target, targetAccessor)
        {
        }

        public Binding(object target, IAccessor targetAccessor, object source, string path)
            : base(target, targetAccessor)
        {
            this.Source = source;
            this.path = path;
        }
        public Binding(object target, string targetPath)
            : base(target, targetPath)
        {
        }

        public Binding(object target, string targetPath, object source, string path)
            : base(target, targetPath)
        {
            this.Source = source;
            this.path = path;
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
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


        private static Dictionary<Type, string> defaultMenbers;


        public override void Bind()
        {
            if (IsBinding)
                return;

            binder = PropertyBinder.Create(path);
            binder.Target = Source;
            PropertyName = binder.MemberName;

            base.Bind();

            if (CanUpdateSourceToTarget)
            {
                if (!SourceSupportNotify && binder.SupportNotify)
                {
                    binder.TargetUpdatedCallback = () =>
                    {
                        if (CanUpdateSourceToTarget)
                            UpdateSourceToTarget();
                    };
                    SourceSupportNotify = true;
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
            isSourceNotifyPropertyChanged = false;


            if (binder != null)
            {
                if (SourceNotifyCallback != null)
                {
                    SourceNotifyCallback(OnSourcePropertyChanged, false);
                }
                binder.TargetUpdatedCallback = null;

                binder.Dispose();
                binder = null;
            }
            base.Unbind();
        }

        public override void UpdateSourceToTarget()
        {

            object value;

            IsNullValue = false;
            IsFallbackValue = false;

            if (binder.TryGetTargetValue(out value))
            {
                var converter = Converter;
                if (converter != null)
                {
                    Type valueType = GetTargetValueType();
                    if (valueType != null)
                    {
                        value = converter.Convert(value, valueType, ConverterParameter);
                    }
                }

                if (value != null)
                {
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


            SetTargetValue(value);

        }



        public override void UpdateTargetToSource()
        {
            if (!binder.CanSetValue) return;

            object value;

            value = GetTargetValue();
            if (value == UnsetValue) return;

            var converter = Converter;
            if (converter != null)
            {
                Type valueType;
                valueType = binder.GetLast()?.ValueType;
                if (valueType != null)
                {
                    value = converter.ConvertBack(value, valueType, ConverterParameter);
                }
            }
            if (value == UnsetValue) return;

            if (binder.TrySetTargetValue(value))
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
                OnSourcePropertyChanged(path);
            }
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


    }



}