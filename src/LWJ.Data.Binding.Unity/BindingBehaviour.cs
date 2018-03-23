using UnityEngine;
using System.Collections;
using System.Reflection;
using System;
using System.Globalization;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using LWJ.Data;
using System.ComponentModel;
using Component = UnityEngine.Component;
public class ArrayPropertyAttribute : PropertyAttribute
{

}

namespace LWJ.Unity
{
    public class BindingBehaviour : MonoBehaviour
    {

        [SerializeField]
        public Entry[] items;
        [NonSerialized]
        private bool isBinding;
        private object contextData;
        private Dictionary<BindingEntry, BindingBase> bindings = new Dictionary<BindingEntry, BindingBase>();

        IDataContext dataContext;
        //private bool isFindDataContext;
        [SerializeField]
        public bool startedBinding;
        void Awake()
        {
            //if (items != null)
            //{
            //foreach (var item in items.SelectMany(o => o.bindings))
            //{
            //    if (string.IsNullOrEmpty(item.nullValue))
            //        item.nullValue = null;
            //    if (string.IsNullOrEmpty(item.stringFormat))
            //        item.stringFormat = null;
            //    if (string.IsNullOrEmpty(item.fallbackValue))
            //        item.fallbackValue = null;
            //}
            //}
        }

        // Use this for initialization
        void Start()
        {

            if (startedBinding)
                Bind();
            //  enabled = false;
        }

        public IDataContext DataContext
        {
            get { return dataContext; }
            set
            {
                if (dataContext != value)
                {
                    if (dataContext != null && dataContext is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged)dataContext).PropertyChanged -= DataContext_PropertyChanged;
                    }
                    dataContext = value;
                    if (dataContext != null && dataContext is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged)dataContext).PropertyChanged += DataContext_PropertyChanged;
                    }
                }
            }
        }



        void OnDataMemberChanged()
        {

        }



        static MemberInfo FindMember(object target, string memberName)
        {
            MemberInfo memberInfo = null;
            if (target != null && memberName != null)
            {
                Type dataType = target.GetType();
                memberInfo = dataType.GetProperty(memberName);
                if (memberInfo == null)
                    memberInfo = dataType.GetField(memberName);
            }
            return memberInfo;
        }


        public IDataContext FindDataContext()
        {
            IDataContext result = null;
            Transform p = transform;
            while (p != null)
            {
                result = p.GetComponent<IDataContext>();
                if (result != null)
                    break;
                p = p.parent;
            }
            return result;
        }




        private object GetSource(BindingEntry entry)
        {
            object source = null;

            switch (entry.sourceType)
            {
                //case SourceType.Relative:
                //    source = ResolveRelativeSource(entry.relativeSource);
                //    break;
                case SourceType.Name:
                    source = ResolveNameSource(entry.nameSource);
                    break;
                default:
                    source = ResolveSource(entry.source);
                    break;
            }

            return source;
        }
        /*
        private object GetSource(ChildBindingEntry entry)
        {
            object source = null;

            switch (entry.sourceType)
            {
                //case SourceType.Relative:
                //    source = ResolveRelativeSource(entry.relativeSource);
                //    break;
                case SourceType.Name:
                    source = ResolveNameSource(entry.nameSource);
                    break;
                default:
                    source = ResolveSource(entry.source);
                    break;
            }

            return source;
        }*/

        private IValueConverter ResolveValueConverter(string converterName)
        {
            IValueConverter converter = null;

            if (!string.IsNullOrEmpty(converterName))
            {
                //var converterType = Type.GetType(converterName);
                //if (converterType == null)
                //    throw new Exception("converter type null");
                //converter = (IValueConverter)Activator.CreateInstance(converterType);
                converter = BindingBase.GetValueConveter(converterName);
            }

            return converter;
        }

        private IMultiValueConverter ResolveMultiValueConverter(string converterName)
        {
            IMultiValueConverter converter = null;

            if (!string.IsNullOrEmpty(converterName))
            {
                //var converterType = Type.GetType(converterName);
                //if (converterType == null)
                //    throw new Exception("converter type null");
                //converter = (IMultiValueConverter)Activator.CreateInstance(converterType);
                converter = BindingBase.GetMultiValueConveter(converterName);
            }

            return converter;
        }


        private void InitBinding(BindingBase bindingBase, BindingEntry entry)
        {
            bindingBase.Target = entry.target;
            bindingBase.NullValue = string.IsNullOrEmpty(entry.nullValue) ? null : entry.nullValue;
            bindingBase.StringFormat = string.IsNullOrEmpty(entry.stringFormat) ? null : entry.stringFormat;
            bindingBase.FallbackValue = string.IsNullOrEmpty(entry.fallbackValue) ? null : entry.fallbackValue;
            bindingBase.Delay = entry.delay;

            string targetPath = entry.targetPath;
            if (string.IsNullOrEmpty(targetPath))
            {
                if (entry.target != null)
                {
                    targetPath = entry.target.GetType().GetDefaultMemberName();
                }
            }

            //multi binding
            //if (string.IsNullOrEmpty(targetPath))
            //    throw new Exception("Binding Target Path Null, {0}".FormatArgs(gameObject.name));
            bindingBase.TargetPath = targetPath;

        }


        private BindingBase ToBinding(BindingEntry entry)
        {
            var binding = new Binding()
            {
                Source = GetSource(entry),
               
                Mode = entry.mode,
                EnabledSourceUpdated = entry.enabledSourceUpdated,
                EnabledTargetUpdated = entry.enabledTargetUpdated,
                Converter = ResolveValueConverter(entry.converter),
                ConverterParameter = string.IsNullOrEmpty(entry.converterParameter) ? null : entry.converterParameter,
            };
            string path = entry.path;
            if (string.IsNullOrEmpty(path))
            {
                if (binding.Source != null)
                    path = binding.Source.GetType().GetDefaultMemberName();
            }

            if (string.IsNullOrEmpty(path))
                throw new Exception("Binding Path Null, {0}".FormatArgs(gameObject.name));

            binding.Path = path;
     
            InitBinding(binding, entry);

            return binding;
        }



        private BindingBase ToMultiBinding(BindingEntry entry)
        {
            var binding = new MultiBinding()
            {
                Mode = entry.mode,
                EnabledSourceUpdated = entry.enabledSourceUpdated,
                EnabledTargetUpdated = entry.enabledTargetUpdated,
                Converter = ResolveMultiValueConverter(entry.converter),
                ConverterParameter = string.IsNullOrEmpty(entry.converterParameter) ? null : entry.converterParameter,
            };


            InitBinding(binding, entry);

            foreach (var item in ToBindings(entry.Children))
            {
                binding.Add(item.Item2);
            }

            return binding;
        }


        private BindingBase ToPrirotyBinding(BindingEntry entry)
        {
            var binding = new PriorityBinding()
            {
            };

            InitBinding(binding, entry);

            return binding;
        }

        BindingBase ToBinding(BindingType type, BindingEntry bindingEntry)
        {
            Func<BindingEntry, BindingBase> bind;
            switch (type)
            {
                case BindingType.MultiBinding:
                    bind = ToMultiBinding;
                    break;
                case BindingType.PriorityBinding:
                    bind = ToPrirotyBinding;
                    break;
                case BindingType.Binding:
                default:
                    bind = ToBinding;
                    break;
            }
            return bind(bindingEntry);
        }

        IEnumerable<Tuple<BindingEntry, BindingBase>> ToBindings(IEnumerable<Entry> entrys)
        {
            if (items == null)
                yield break;
            foreach (var entry in entrys)
            {
                if (entry.bindings == null)
                    continue;

                foreach (var bindingEntry in entry.bindings)
                {
                    var binding = ToBinding(entry.type, bindingEntry);
                    yield return new Tuple<BindingEntry, BindingBase>(bindingEntry, binding);

                }
            }
        }


        public void Bind()
        {
            if (isBinding)
                return;
            gameObject.SendMessage("OnDataBinding", SendMessageOptions.DontRequireReceiver);
            if (dataContext != null)
                contextData = dataContext.DataContext;
            else
                contextData = null;

            foreach (var item in ToBindings(items))
            {
                var binding = item.Item2;
                var bindingEntry = item.Item1;

                binding.Bind();
                bindings[bindingEntry] = binding;
            }

            isBinding = true;
            gameObject.SendMessage("OnDataBind", SendMessageOptions.DontRequireReceiver);
        }

        public void Unbind()
        {
            if (!isBinding)
                return;

            if (bindings != null)
            {
                foreach (var bindingItem in bindings)
                {
                    bindingItem.Value.Unbind();
                }
                bindings.Clear();
            }

            isBinding = false;
            gameObject.SendMessage("OnDataUnbind", SendMessageOptions.DontRequireReceiver);
        }

        public static void Bind(GameObject target)
        {
            foreach (var binding in target.GetComponentsInChildren<BindingBehaviour>())
            {
                binding.Bind();
            }
        }

        public static void Unbind(GameObject target)
        {
            foreach (var binding in target.GetComponentsInChildren<BindingBehaviour>())
            {
                binding.Unbind();
            }
        }


        void OnDestroy()
        {

            if (isBinding)
                Unbind();
            DataContext = null;

        }

        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isBinding)
            {
                if (e.PropertyName == "DataContext")
                {
                    var oldContextData = contextData;
                    contextData = dataContext.DataContext;
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            switch (item.type)
                            {
                                case BindingType.Binding:
                                    if (item.bindings != null)
                                    {
                                        foreach (var bindingEntry in item.bindings)
                                        {
                                            if (!bindings.ContainsKey(bindingEntry))
                                                continue;

                                            Binding binding = (Binding)bindings[bindingEntry];
                                            if (bindingEntry.source == null && binding.Source == oldContextData)
                                            {
                                                binding.Source = contextData;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        object ResolveSource(Object source)
        {
            object result = source;

            if (source != null)
            {
                result = source;
            }
            else
            {
                //if (dataContext == null && !isFindDataContext)
                //{
                DataContext = FindDataContext();
                //isFindDataContext = true;
                //}

                if (dataContext != null)
                {
                    result = dataContext.DataContext;
                }

            }
            return result;
        }


        object ResolveRelativeSource(RelativeSourceEntry relativeSourceEntry)
        {
            object result = null;
            switch (relativeSourceEntry.mode)
            {
                case RelativeSourceMode.Self:
                    result = GetComponent<IDataContext>();
                    break;
                case RelativeSourceMode.TemplatedParent:
                    //?     result = GetComponentInParent<DataTemplate>();
                    break;
                case RelativeSourceMode.PreviousData:
                    break;
                case RelativeSourceMode.FindAncestor:
                default:
                    if (string.IsNullOrEmpty(relativeSourceEntry.typeName))
                        throw new Exception("relative source type name is null");
                    Type type = Type.GetType(relativeSourceEntry.typeName);
                    //if (!typeof(Component).IsAssignableFrom(type))
                    //    throw new Exception("type name not component type :" + relativeSourceEntry.typeName);

                    Component c = GetComponentInParent(type);
                    int n = relativeSourceEntry.ancestorLevel - 1;
                    while (c && n > 0)
                    {
                        c = c.transform.parent.GetComponentInParent(type);
                        n--;
                    }
                    if (n > 0)
                        throw new Exception("Binding Not Found Ancestor Data, type: {0}, ancestorLevel: {1}".FormatArgs(relativeSourceEntry.typeName, relativeSourceEntry.ancestorLevel));
                    result = c;
                    break;
            }
            return result;
        }

        object ResolveNameSource(FindNameEntry nameSourceEntry)
        {
            object result = null;
            string name = nameSourceEntry.name;
            string typeName = nameSourceEntry.typeName;

            if (string.IsNullOrEmpty(name))
                //throw new Exception("name source name null");
                return null;

            //if (string.IsNullOrEmpty(typeName))
            //    //throw new Exception("name source type name is null");
            //    return null;

            GameObject go = null;
            switch (nameSourceEntry.findMode)
            {
                case FindNameMode.Children:
                    {
                        Transform t = transform.FindByName(name);
                        if (t)
                            go = t.gameObject;
                    }
                    break;
                case FindNameMode.Global:
                    go = GameObject.Find(name);
                    break;
                case FindNameMode.Parent:
                    {
                        Transform t = transform;
                        while (t && t.name != name)
                        {
                            t = t.parent;
                        }
                        if (t)
                            go = t.gameObject;
                    }
                    break;
            }

            if (!go)
                return null;
            if (string.IsNullOrEmpty(typeName))
            {
                DataContext = go.GetComponent<IDataContext>();
                if (dataContext != null)
                    result = dataContext.DataContext;
            }
            else
            {
                if (string.Equals("GameObject", typeName, StringComparison.InvariantCultureIgnoreCase))
                    return go;
                result = go.GetComponent(typeName);
            }


            //Type type = Type.GetType(typeName);

            //if (!typeof(Component).IsAssignableFrom(type))
            //    throw new Exception("name name not component type :" + typeName);

            //result = FindNameComponentInParent(transform, name, type);

            return result;
        }

        Component FindNameComponentInParent(Transform parent, string name, Type componetType)
        {
            Component component = null;
            Transform exclude = null;

            Transform p = parent;
            while (p != null)
            {
                if (p.name == name)
                {
                    component = p.GetComponent(componetType);
                    if (component != null)
                        break;
                }

                foreach (Transform child in p)
                {
                    if (child == exclude)
                        continue;
                    component = FindNameComponentInChildren(child, name, componetType);
                    if (component != null)
                        break;
                }

                if (component != null)
                    break;

                exclude = p;
                p = p.parent;
            }

            return component;
        }

        Component FindNameComponentInChildren(Transform parent, string name, Type componetType)
        {
            Component component = null;
            if (parent.name == name)
            {
                component = parent.GetComponent(componetType);
                if (component != null)
                    return component;
            }

            foreach (Transform child in parent)
            {
                component = FindNameComponentInChildren(child, name, componetType);
                if (component != null)
                    return component;
            }
            return null;
        }
        [Serializable]
        public class Entry
        {
            [SerializeField]
            public BindingType type;


            [SerializeField]
            public BindingEntry[] bindings;
        }

        [Serializable]
        public class BindingEntry : ISerializationCallbackReceiver
        {

            public BindingType bindingType;
            public Object target;
            public string targetPath;
            public string nullValue;
            public string stringFormat;
            public int delay;
            public SourceType sourceType;
            public Object source;
            public RelativeSourceEntry relativeSource;
            public FindNameEntry nameSource;
            public string path;
            public BindingMode mode;
            public string fallbackValue;
            public string converter;
            public string converterParameter;
            public bool enabledSourceUpdated;
            public bool enabledTargetUpdated;


            [SerializeField]
            public ChildEntry[] children;
            [NonSerialized]
            [SerializeField]
            public Entry[] Children;

            public void OnBeforeSerialize()
            {

            }

            public void OnAfterDeserialize()
            {
                if (children == null)
                    Children = null;
                else
                    Children = children.Select(o => o.ToEntry()).ToArray();
            }
        }

        [Serializable]
        public class ChildEntry
        {
            [SerializeField]
            public BindingType type;

            [SerializeField]
            public ChildBindingEntry[] bindings;

            public Entry ToEntry()
            {
                Entry entry = new Entry();
                entry.type = type;
                if (bindings != null)
                    entry.bindings = bindings.Select(o => o.ToBindingEntry()).ToArray();
                return entry;
            }

            public ChildEntry(Entry entry)
            {
                type = entry.type;
                if (bindings != null)
                    bindings = entry.bindings.Select(o => new ChildBindingEntry(o)).ToArray();

            }

        }


        [Serializable]
        public class ChildBindingEntry
        {

            public BindingType bindingType;

            public string nullValue;
            public string stringFormat;
            public int delay;

            public SourceType sourceType;
            public Object source;
            public RelativeSourceEntry relativeSource;
            public FindNameEntry nameSource;
            public string path;
            public BindingMode mode;
            public string fallbackValue;
            public string converter;
            public string converterParameter;
            public bool notifyOnSourceUpdated;
            public bool notifyOnTargetUpdated;

            public BindingEntry ToBindingEntry()
            {
                BindingEntry binding = new BindingEntry();
                binding.bindingType = bindingType;

                binding.nullValue = nullValue;
                binding.stringFormat = stringFormat;
                binding.delay = delay;
                binding.sourceType = sourceType;
                binding.source = source;
                binding.relativeSource = relativeSource;
                binding.nameSource = nameSource;
                binding.path = path;
                binding.mode = mode;
                binding.fallbackValue = fallbackValue;
                binding.converter = converter;
                binding.converterParameter = converterParameter;
                binding.enabledSourceUpdated = notifyOnSourceUpdated;
                binding.enabledTargetUpdated = notifyOnTargetUpdated;
                return binding;
            }

            public ChildBindingEntry(BindingEntry binding)
            {
                bindingType = binding.bindingType;
                nullValue = binding.nullValue;
                stringFormat = binding.stringFormat;
                delay = binding.delay;
                sourceType = binding.sourceType;
                source = binding.source;
                relativeSource = binding.relativeSource;
                nameSource = binding.nameSource;
                path = binding.path;
                mode = binding.mode;
                fallbackValue = binding.fallbackValue;
                converter = binding.converter;
                converterParameter = binding.converterParameter;
                notifyOnSourceUpdated = binding.enabledSourceUpdated;
                notifyOnTargetUpdated = binding.enabledTargetUpdated;

            }

        }

        [Serializable]
        public class RelativeSourceEntry
        {

            public int ancestorLevel;

            public string typeName;

            public RelativeSourceMode mode;

        }


        [Serializable]
        public class FindNameEntry
        {
            public FindNameMode findMode;
            public string name;
            public string typeName;
        }

        public enum FindNameMode
        {
            Global = 0,
            Children,
            Parent,
        }

        public enum RelativeSourceMode
        {
            FindAncestor,
            PreviousData,
            Self,
            TemplatedParent,
        }

        public enum BindingType
        {
            Binding,
            MultiBinding,
            PriorityBinding,
        }

        public enum SourceType
        {
            Source,
            Name,
            //  Relative,
        }

    }

}