using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEngine.Bindings
{
    public static class Extensions
    {

        public static IDisposable Bind<TValue>(this VisualElement target, object source, string path, BindingMode mode = BindingMode.TwoWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var targetAccessor = target as INotifyValueChanged<TValue>;
            if (targetAccessor == null)
                throw new Exception("target not INotifyValueChanged<T>");
            return Bind(target, targetAccessor, source, path, mode: mode);
        }

        public static IDisposable Bind<TValue>(this VisualElement target, INotifyValueChanged<TValue> targetAccessor, object source, string path, BindingMode mode = BindingMode.TwoWay)
        {

            if (target == null) throw new ArgumentNullException(nameof(target));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (path == null) throw new ArgumentNullException(nameof(path));


            Binding<TValue> binding = new Binding<TValue>(target, source, path);
            binding.TargetAccessor = targetAccessor;
            binding.Bind();

            return binding;
        }



        public static IDisposable Bind<TValue>(this VisualElement target, object source, string propertyName, INotifyValueChanged<TValue> accessor, BindingMode mode = BindingMode.TwoWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var targetAccessor = target as INotifyValueChanged<TValue>;
            if (targetAccessor == null)
                throw new Exception("target not INotifyValueChanged<T>");
            return Bind(target, targetAccessor, source, propertyName, accessor, mode);
        }

        public static IDisposable Bind<TValue>(this VisualElement target, INotifyValueChanged<TValue> targetAccessor, object source, string propertyName, INotifyValueChanged<TValue> accessor, BindingMode mode = BindingMode.TwoWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));


            var binding = new CustomBinding<TValue>.AccessorPropertyBinding2();
            binding.mode = mode;
            binding.target = target;
            binding.targetAccessor = targetAccessor;
            binding.source = source;
            binding.propertyName = propertyName;
            binding.accessor = accessor;
            binding.Bind();

            return binding;
        }
          
 

    }
}