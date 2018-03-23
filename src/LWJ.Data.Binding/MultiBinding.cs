using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LWJ.Data
{


    public class MultiBinding : BindingBase
    {

        private IMultiValueConverter converter;
        private object converterParameter;
        private BindingMode mode;
        private bool enabledSourceUpdated = true;
        private bool enabledTargetUpdated = true;

        private List<BindingBase> bindings = new List<BindingBase>();

        private ObservableCollection<object> sourceValues;


        public MultiBinding()
        {
            mode = BindingMode.OneWay;
            sourceValues = new ObservableCollection<object>();
            sourceValues.CollectionChanged += SourceValues_CollectionChanged;
        }

        public BindingMode Mode { get => mode; set => mode = value; }

        public IMultiValueConverter Converter { get => converter; set => converter = value; }

        public object ConverterParameter { get => converterParameter; set => converterParameter = value; }

        public bool EnabledSourceUpdated { get => enabledSourceUpdated; set => enabledSourceUpdated = value; }

        public bool EnabledTargetUpdated { get => enabledTargetUpdated; set => enabledTargetUpdated = value; }

        public ReadOnlyCollection<BindingBase> Bindings
        {
            get { return bindings.AsReadOnly(); }
        }

        public event EventHandler<DataTransferEventArgs> SourceUpdated;

        public event EventHandler<DataTransferEventArgs> TargetUpdated;

        public override void Bind()
        {
            base.Bind();

            sourceValues.Clear();

            isUpdating = true;
            foreach (var binding in bindings)
            {
                sourceValues.Add(null);
                var b = binding as Binding;

                binding.TargetPath = "[" + (sourceValues.Count - 1) + "]";
                binding.Target = sourceValues;
                if (b != null)
                    b.Mode = mode;

                binding.Bind();
            }
            isUpdating = false;
            if (mode == BindingMode.OneWayToSource)
                UpdateTargetToSource();
            else
                UpdateSourceToTarget();
        }

        public override void Unbind()
        {
            base.Unbind();

            foreach (var item in bindings)
            {
                item.Unbind();
            }

        }

        private bool isUpdating;
        private void SourceValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!isUpdating)
                UpdateSourceToTarget();
        }

        protected override void UpdateSourceToTarget()
        {
            if (mode == BindingMode.OneWayToSource)
                return;

            if (!targetBinder.CanSetValue())
                return;

            bool hasValue = true;

            foreach (var item in bindings)
            {
                if (item.IsFallbackValue)
                {
                    hasValue = false;
                    break;
                }
            }

            IsFallbackValue = false;
            IsNullValue = false;

            object value;

            if (hasValue)
            {
                object[] values;
                values = sourceValues.ToArray();
                if (converter != null)
                {
                    value = converter.Convert(values, targetBinder.GetValueType(), converterParameter);
                }
                else
                {
                    if (values.Length > 0)
                        value = values[0];
                    else
                        value = null;
                }
                if (value != null)
                {
                    var format = StringFormat;
                    if (!string.IsNullOrEmpty(format))
                        value = String.Format(format, value);
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

            object value;

            if (!targetBinder.TryGetValue(out value))
                return;

            Type[] types = sourceValues.Select(o => o == null ? typeof(object) : o.GetType()).ToArray();

            object[] values = null;
            if (converter != null)
                values = converter.ConvertBack(value, types, converterParameter);
            isUpdating = true;
            for (int i = 0, len = bindings.Count; i < len; i++)
            {
                if (values == null)
                    sourceValues[i] = null;
                else
                    sourceValues[i] = values[i];
            }
            isUpdating = false;

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

        public void Add(BindingBase binding)
        {
            bindings.Add(binding);
        }

        public void Remove(BindingBase binding)
        {
            bindings.Remove(binding);
        }



    }


}
