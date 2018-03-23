using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LWJ.Data
{
    public class PriorityBinding : BindingBase
    {
        private List<BindingBase> bindings = new List<BindingBase>();
        private BindingBase current;
        private int currentIndex;
        private bool isUpdating;
        private ObservableCollection<object> sourceValues;

        public PriorityBinding()
        {
            currentIndex = -1;
            sourceValues = new ObservableCollection<object>();
            sourceValues.CollectionChanged += SourceValues_CollectionChanged;

        }

        public ReadOnlyCollection<BindingBase> Bindings
        {
            get { return bindings.AsReadOnly(); }
        }

        public override void Bind()
        {
            base.Bind();

            sourceValues.Clear();
            currentIndex = -1;
            current = null;
            foreach (var binding in bindings)
            {
                sourceValues.Add(null);
            }

            sourceValues.CollectionChanged += SourceValues_CollectionChanged_Current;
            isUpdating = true;
            int n = 0;
            foreach (var binding in bindings)
            {
                var b = binding as Binding;

                binding.TargetPath = "[" + (n++) + "]";
                binding.Target = sourceValues;
                binding.Bind();
                if (current != null)
                {
                    break;
                }
            }
            isUpdating = false;

            if (current != null)
            {
                foreach (var b in bindings)
                {
                    if (b != current)
                        b.Unbind();
                }
            }
        }

        public override void Unbind()
        {

            base.Unbind();

            sourceValues.CollectionChanged -= SourceValues_CollectionChanged_Current;
            if (current != null)
            {
                current.Unbind();
                current = null;
            }

            foreach (var item in bindings)
            {
                item.Unbind();
            }

        }

        private void SourceValues_CollectionChanged_Current(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (currentIndex == -1)
            {
                int index = e.NewStartingIndex;
                if (!bindings[index].IsFallbackValue)
                {
                    currentIndex = index;
                    current = bindings[currentIndex];

                    sourceValues.CollectionChanged -= SourceValues_CollectionChanged_Current;
                }
            }
        }

        private void SourceValues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (!isUpdating)
                UpdateSourceToTarget();
        }
         
        protected override void UpdateSourceToTarget()
        { 

            if (!targetBinder.CanSetValue())
                return;

            object value;
            IsFallbackValue = false;
            IsNullValue = false;
            if (current != null)
            {
                value = sourceValues[currentIndex];
                if (value == null)
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
            targetBinder.TrySetValue(value);
        }

        protected override void UpdateTargetToSource()
        {
            if (current == null)
                return;
            object value;

            if (!targetBinder.TryGetValue(out value))
                return;
            sourceValues[currentIndex] = value;
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
