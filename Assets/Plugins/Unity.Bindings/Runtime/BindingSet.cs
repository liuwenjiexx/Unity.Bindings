using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    public class BindingSet
    {
        protected List<object> builders = new List<object>();
        protected List<BindingBase> bindings = new List<BindingBase>();
        public List<BindingBase> Bindings => bindings;

        public BindingMode? Mode { get; set; }

        public BindingNotifyDelegate TargetNotifyCallback { get; set; }

        public BindingNotifyDelegate SourceNotifyCallback { get; set; }

        public bool? SourceToTargetNotifyEnabled { get; set; }

        public bool IsBinding
        {
            get
            {
                foreach (var binding in bindings)
                {
                    if (!binding.IsBinding)
                        return false;
                }
                return true;
            }
        }

        public event BindingPropertyChangedEventHandler SourcePropertyChanged;

        public event BindingPropertyChangedEventHandler TargetPropertyChanged;


        public BindingBuilder<TTarget, object> Build<TTarget>(TTarget target)
        {
            return Build<TTarget, object>(target, default);
        }

        public BindingBuilder<TTarget, TNewSource> Build<TTarget, TNewSource>(TTarget target, TNewSource source)
        {
            var builder = new BindingBuilder<TTarget, TNewSource>(target, source);

            builder.TargetNotifyCallback = TargetNotifyCallback;
            builder.SourceNotifyCallback = SourceNotifyCallback;

            if (Mode.HasValue) builder.Mode = Mode;
            if (SourceToTargetNotifyEnabled.HasValue) builder.SourceToTargetNotifyEnabled = SourceToTargetNotifyEnabled;

            builder.Created = (binding) =>
            {
                bindings.Add(binding);
                builders.Remove(builder);
            };
            builders.Add(builder);
            return builder;
        }

        protected void BuildBinding()
        {
            if (builders.Count == 0)
                return;
            foreach (var builder in builders.ToArray())
            {
                var mInfo = builder.GetType().GetMethod("Build");
                mInfo.Invoke(builder, null);
            }
            builders.Clear();
        }

        protected void OnSourcePropertyChangedEventArgs(object sender, BindingPropertyChangedEventArgs e)
        {
            SourcePropertyChanged?.Invoke(sender, e);
        }

        protected void OnTargetPropertyChangedEventArgs(object sender, BindingPropertyChangedEventArgs e)
        {
            TargetPropertyChanged?.Invoke(sender, e);
        }

        public void Bind()
        {
            BuildBinding();

            foreach (var binding in bindings)
            {
                try
                {
                    if (!binding.IsBinding)
                        binding.Bind();

                    if (SourcePropertyChanged != null)
                    {
                        binding.SourcePropertyChanged -= OnSourcePropertyChangedEventArgs;
                        binding.SourcePropertyChanged += OnSourcePropertyChangedEventArgs;
                    }

                    if (TargetPropertyChanged != null)
                    {
                        binding.TargetPropertyChanged -= OnTargetPropertyChangedEventArgs;
                        binding.TargetPropertyChanged += OnTargetPropertyChangedEventArgs;
                    }

                    if (binding.TargetAccessor != null && binding.TargetAccessor is INotifyValueChangedAccessor)
                    {
                        IBindable bindable = binding.Target as IBindable;
                        if (bindable != null)
                        {
                            bindable.binding = binding;
                        }
                    }
                }
                catch
                {
                    Debug.LogError($"Bind error '{binding.PropertyName}'");
                    throw;
                }
            }
        }

        public void Unbind()
        {
            foreach (var binding in bindings)
            {
                binding.SourcePropertyChanged -= OnSourcePropertyChangedEventArgs;
                binding.TargetPropertyChanged -= OnTargetPropertyChangedEventArgs;

                if (binding.IsBinding)
                    binding.Unbind();

                if (binding.TargetAccessor != null && binding.TargetAccessor is INotifyValueChangedAccessor)
                {
                    IBindable bindable = binding.Target as IBindable;
                    if (bindable != null)
                    {
                        bindable.binding = null;
                    }
                }
            }
        }


        public void Update()
        {
            foreach (var binding in bindings)
            {
                binding.Update();
            }
        }

        public void UpdateSourceToTarget()
        {
            foreach (var binding in bindings)
            {
                if (binding.IsBinding)
                {
                    if (binding.CanUpdateSourceToTarget)
                    {
                        binding.UpdateSourceToTarget();
                    }
                }
            }
        }
        public void UpdateTargetToSource()
        {
            foreach (var binding in bindings)
            {
                if (binding.IsBinding)
                {
                    if (binding.CanUpdateTargetToSource)
                    {
                        binding.UpdateTargetToSource();
                    }
                }
            }
        }


    }

    public class BindingSet<TSource> : BindingSet
    {

        public BindingSet(TSource source)
        {
            this.Source = source;
        }

        public TSource Source { get; private set; }


        public BindingBuilder<TTarget, TSource> Build<TTarget>(TTarget target)
        {
            return Build(target, Source);
        }



        #region Binding Path

        /// <summary>
        /// 绑定属性路径
        /// </summary> 
        private BindingBase Bind(object target, IAccessor targetAccessor, string targetPropertyName, string path, BindingMode mode = BindingMode.OneWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));
            if (path == null) throw new ArgumentNullException(nameof(path));

            Binding binding = new Binding(target, targetAccessor, Source, path);
            binding.Mode = mode;
            binding.TargetPropertyName = targetPropertyName;
            bindings.Add(binding);

            return binding;
        }

        public BindingBase Bind(object target, IAccessor targetAccessor, string path, BindingMode mode = BindingMode.OneWay)
        {
            return Bind(target, targetAccessor, (string)null, path, mode);
        }
        public BindingBase Bind(object target, string targetPath, string path, BindingMode mode = BindingMode.OneWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetPath == null) throw new ArgumentNullException(nameof(targetPath));
            if (path == null) throw new ArgumentNullException(nameof(path));

            Binding binding = new Binding(target, targetPath, Source, path);
            binding.Mode = mode;
            bindings.Add(binding);
            return binding;
        }

        /// <summary>
        /// 绑定属性路径
        /// </summary>
        public BindingBase Bind<TTarget, TValue>(TTarget target, Expression<Func<TTarget, TValue>> targetPropertySelector, string path, BindingMode mode = BindingMode.OneWay)
        {
            var targetAccessor = Accessor.Member(targetPropertySelector);
            return Bind(target, targetAccessor, targetAccessor.MemberInfo.Name, path, mode);
        }


        #endregion

        #region Binding Property

        /// <summary>
        /// 绑定属性
        /// </summary>
        private BindingBase Bind(object target, IAccessor targetAccessor, string targetPropertyName, IAccessor accessor, string propertyName, BindingMode mode = BindingMode.OneWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (targetAccessor == null) throw new ArgumentNullException(nameof(targetAccessor));
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));

            var binding = new PropertyBinding(target, targetAccessor, Source, accessor);
            binding.Mode = mode;
            binding.TargetPropertyName = targetPropertyName;
            binding.PropertyName = propertyName;
            bindings.Add(binding);
            return binding;
        }


        public BindingBase Bind(object target, IAccessor targetAccessor, Accessor accessor, string propertyName, BindingMode mode = BindingMode.OneWay)
        {
            return Bind(target, targetAccessor, null, accessor, propertyName, mode);
        }

        public BindingBase Bind<TTarget, TValue>(object target, Expression<Func<TTarget, TValue>> targetPropertySelector, Expression<Func<TSource, TValue>> propertySelector, BindingMode mode = BindingMode.OneWay)
        {
            if (targetPropertySelector == null) throw new ArgumentNullException(nameof(targetPropertySelector));

            var targetAccessor = Accessor.Member(targetPropertySelector);
            var member = BindingUtility.GetMember(propertySelector);
            var accessor = Accessor.Member<TValue>(member);
            return Bind(target, targetAccessor, targetAccessor.MemberInfo.Name, accessor, accessor.MemberInfo.Name, mode);
        }

        private BindingBase Bind<TTarget, TValue>(object target, Expression<Func<TTarget, TValue>> targetPropertySelector, IAccessor<TValue> accessor, string propertyName, BindingMode mode = BindingMode.OneWay)
        {
            if (targetPropertySelector == null) throw new ArgumentNullException(nameof(targetPropertySelector));

            var targetAccessor = Accessor.Member(targetPropertySelector);
            return Bind(target, targetAccessor, targetAccessor.MemberInfo.Name, accessor, propertyName, mode);
        }


        #endregion


        #region Target INotifyValueChanged

        /// <summary>
        /// <paramref name="target"/> 绑定到 <see cref="INotifyValueChanged{T}.value"/> 属性
        /// </summary>
        public BindingBase Bind<TValue>(INotifyValueChanged<TValue> target, string path, BindingMode mode = BindingMode.TwoWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (path == null) throw new ArgumentNullException(nameof(path));

            var targetAccessor = Extensions.GetTargetAccessorWithINotifyValueChanged<TValue>(target);

            Binding binding = new Binding(target, targetAccessor, Source, path);
            binding.Mode = mode;
            binding.TargetNotifyValueChangedEnabled = true;
            bindings.Add(binding);
            return binding;
        }

        /// <summary>
        /// <paramref name="target"/> 绑定到 <see cref="INotifyValueChanged{T}.value"/> 属性
        /// </summary>
        private BindingBase Bind<TValue>(INotifyValueChanged<TValue> target, IAccessor<TValue> accessor, string propertyName, BindingMode mode = BindingMode.TwoWay)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var targetAccessor = Extensions.GetTargetAccessorWithINotifyValueChanged<TValue>(target);

            var binding = new PropertyBinding(target, targetAccessor, Source, accessor);
            binding.Mode = mode;
            binding.PropertyName = propertyName;
            binding.TargetNotifyValueChangedEnabled = true;
            bindings.Add(binding);
            return binding;
        }

        /// <summary>
        /// <paramref name="target"/> 绑定到 <see cref="INotifyValueChanged{T}.value"/> 属性
        /// </summary>
        public BindingBase Bind<TValue>(INotifyValueChanged<TValue> target, Expression<Func<TSource, TValue>> propertySelector, BindingMode mode = BindingMode.TwoWay)
        {
            if (propertySelector == null) throw new ArgumentNullException(nameof(propertySelector));
            var member = BindingUtility.GetMember(propertySelector);
            var accessor = Accessor.Member<TValue>(member);
            return Bind(target, accessor, accessor.MemberInfo.Name, mode);
        }

        #endregion


        public void CreateBinding(VisualElement root)
        {
            CreateBinding(root, null);
        }

        public void CreateBinding(VisualElement root, Func<VisualElement, bool> filter = null)
        {
            root.Query<BindableElement>().Build().ForEach(target =>
            {
                if (!string.IsNullOrEmpty(target.bindingPath) && target.binding == null)
                {
                    var type = Extensions.FindGenericTypeDefinition(target.GetType(), typeof(INotifyValueChanged<>));
                    if (type != null)
                    {
                        if (filter != null && !filter.Invoke(target))
                            return;

                        Type valueType = type.GenericTypeArguments[0];
                        var method = typeof(Extensions).GetMethod(nameof(Extensions.GetTargetAccessorWithINotifyValueChanged), BindingFlags.NonPublic | BindingFlags.Static);
                        var targetAccessor = (IAccessor)method.MakeGenericMethod(valueType).Invoke(null, new object[] { target });

                        Build(target).To(targetAccessor).From(target.bindingPath);
                        //var binding = new Binding(target, targetAccessor, source, target.bindingPath);
                        //binding.TargetNotifyValueChangedEnabled = true;
                        //binding.Bind();
                        //target.binding = binding;
                        //bindingSet.Bindings.Add(binding);
                    }
                }
            });
            BuildBinding();
        }
    }
}
