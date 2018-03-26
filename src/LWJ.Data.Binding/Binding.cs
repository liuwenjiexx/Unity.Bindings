using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.Data
{

    public class Binding : BindingBase
    {
        private object source;
        private string path;
        private BindingMode mode;
        private PropertyPath sourceBinder;
        private IValueConverter converter;
        private object converterParameter;
        private bool enabledSourceUpdated = true;
        private bool enabledTargetUpdated = true;

        //private bool isBindToSource;
        //private bool isBindToTarget;

        public Binding()
        {

        }


        public Binding(object source, string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            this.path = path;
            this.source = source;
            mode = BindingMode.OneWay;
        }


        public Binding(object source, string path, object target, string targetPath)
            : this(source, path, target, targetPath, BindingMode.OneWay)
        {

        }
        public Binding(object source, string path, object target, string targetPath, BindingMode mode)
        {
            //if (path == null)
            //    throw new ArgumentNullException("path");
            if (targetPath == null)
                throw new ArgumentNullException("targetPath");
            this.mode = mode;
            this.source = source;
            this.path = path;
            this.TargetPath = targetPath;
            this.Target = target;

        }

        /// <summary>
        /// source target
        /// </summary>
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
        /// <summary>
        /// source path, ( null, "", "." ) : <see cref="Source"/>
        /// </summary>
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

        public IValueConverter Converter { get => converter; set => converter = value; }
        public object ConverterParameter { get => converterParameter; set => converterParameter = value; }
        public bool EnabledSourceUpdated { get => enabledSourceUpdated; set => enabledSourceUpdated = value; }
        public bool EnabledTargetUpdated { get => enabledTargetUpdated; set => enabledTargetUpdated = value; }


        public event EventHandler<DataTransferEventArgs> SourceUpdated;
        public event EventHandler<DataTransferEventArgs> TargetUpdated;


        protected override void UpdateSourceToTarget()
        {
            if (mode == BindingMode.OneWayToSource)
                return;

            if (!targetBinder.CanSetValue())
                return;

            object value;

            IsNullValue = false;
            IsFallbackValue = false;

            if (sourceBinder.TryGetValue(out value))
            {
                if (value != null)
                {
                    if (converter != null)
                        value = converter.Convert(value, targetBinder.GetValueType(), converterParameter);
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

            if (targetBinder.TrySetValue(value))
            {
                if (enabledTargetUpdated)
                {
                    var targetUpdated = TargetUpdated;
                    if (targetUpdated != null)
                    {
                        var args = new DataTransferEventArgs();
                        targetUpdated(this, args);
                    }
                }
            }

        }

        protected override void UpdateTargetToSource()
        {
            if (!(mode == BindingMode.TwoWay || mode == BindingMode.OneWayToSource))
                return;

            if (!sourceBinder.CanSetValue())
                return;

            object value;

            if (!targetBinder.TryGetValue(out value))
                return;

            if (converter != null)
                value = converter.ConvertBack(value, sourceBinder.GetValueType(), converterParameter);

            if (sourceBinder.TrySetValue(value))
            {
                if (enabledSourceUpdated)
                {
                    var sourceUpdated = SourceUpdated;
                    if (sourceUpdated != null)
                    {
                        var args = new DataTransferEventArgs();
                        sourceUpdated(this, args);
                    }
                }

            }


        }




        public override void Bind()
        {
            base.Bind();

            //if (string.IsNullOrEmpty(path))
            //    throw new Exception("Path Empty");


            //if (mode == BindingMode.OneTime || mode == BindingMode.OneWay || mode == BindingMode.TwoWay)
            //    isBindToTarget = true;
            //else
            //    isBindToTarget = false;

            //if (mode == BindingMode.TwoWay || mode == BindingMode.OneWayToSource)
            //    isBindToSource = true;
            //else
            //    isBindToSource = false;

            //targetBinder.AllowListenerChanged = isBindToSource;

            sourceBinder = PropertyPath.Create(path);

            //sourceBinder.AllowListenerChanged = isBindToTarget;
            sourceBinder.Target = source;
            sourceBinder.TargetUpdatedCallback = UpdateSourceToTarget;


            if (mode == BindingMode.OneWayToSource)
                UpdateTargetToSource();
            else
                UpdateSourceToTarget();
        }

        public override void Unbind()
        {
            base.Unbind();

            if (sourceBinder != null)
            {
                sourceBinder.Dispose();
                sourceBinder = null;
            }
        }


    }

}
