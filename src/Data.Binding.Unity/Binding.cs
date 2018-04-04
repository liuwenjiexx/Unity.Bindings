using LWJ.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

public class ArrayPropertyAttribute : PropertyAttribute
{

}

namespace LWJ.Unity
{

    public class Binding : MonoBehaviour
    {
        [SerializeField]
        private List<Entry> bindings;

        //[SerializeField]
        //public Entry1[] items;
        [NonSerialized]
        private bool isBinding;
        private object contextData;
        //private Dictionary<BindingEntry, BindingBase> bindings2 = new Dictionary<BindingEntry, BindingBase>();

        //IDataContext dataContext;
        //private bool isFindDataContext;
        [SerializeField]
        public bool startedBinding;

        public List<Entry> Bindings
        {
            get
            {
                if (bindings == null)
                    bindings = new List<Entry>();
                return bindings;
            }
        }

        static Binding()
        {
            #region UnityEngine.UI

            BindingBase.SetDefaultMember(typeof(Text), "text");
            BindingBase.SetDefaultMember(typeof(Image), "sprite");
            BindingBase.SetDefaultMember(typeof(RawImage), "texture");

            #endregion

            //BindingBase.AddValueConverter("Sprite", typeof(Texture2DConverter));
            //BindingBase.AddValueConverter("Texture2D", typeof(Texture2DConverter));

        }

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
            enabled = false;
        }

        void Update()
        {
            this.enabled = false;
        }

        /*
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
        }*/





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



        BindingBase ToBinding(BindingType type, Entry entry)
        {
            BindingBase bindingBase;
            switch (type)
            {
                case BindingType.MultiBinding:
                    {
                        var binding = new MultiBinding()
                        {
                            Mode = entry.mode,
                            EnabledSourceUpdated = entry.enabledSourceUpdated,
                            EnabledTargetUpdated = entry.enabledTargetUpdated,
                            Converter = ResolveMultiValueConverter(entry.converter),
                            ConverterParameter = string.IsNullOrEmpty(entry.converterParameter) ? null : entry.converterParameter,
                        };

                        //foreach (var item in ToBindings(entry.Children))
                        //{
                        //    binding.Add(item.Item2);
                        //}
                        foreach (var item in entry.Bindings)
                        {
                            binding.Add(ToBinding(item.bindingType, item));
                        }
                        bindingBase = binding;
                    }
                    break;
                case BindingType.PriorityBinding:
                    {
                        var binding = new PriorityBinding()
                        {
                        };


                        bindingBase = binding;
                    }
                    break;
                case BindingType.Binding:
                default:
                    {
                        Data.Binding binding = new Data.Binding()
                        {
                            Source = ResolveSource(entry),
                            Mode = entry.mode,
                            EnabledSourceUpdated = entry.enabledSourceUpdated,
                            EnabledTargetUpdated = entry.enabledTargetUpdated,
                            Converter = ResolveValueConverter(entry.converter),
                            ConverterParameter = string.IsNullOrEmpty(entry.converterParameter) ? null : entry.converterParameter,
                        };
                        //string path = entry.path;
                        //if (string.IsNullOrEmpty(path))
                        //{
                        //    if (binding.Source != null)
                        //        path = binding.Source.GetType().GetDefaultMemberName();
                        //}

                        //if (string.IsNullOrEmpty(path))
                        //    throw new Exception("Binding Path Null, {0}".FormatArgs(gameObject.name));

                        binding.Path = entry.path;
                        bindingBase = binding;
                        Debug.Log("src:" + binding.Source + "," + binding.Path);
                    }
                    break;
            }

            bindingBase.Target = entry.target;
            bindingBase.NullValue = string.IsNullOrEmpty(entry.nullValue) ? null : entry.nullValue;
            bindingBase.StringFormat = string.IsNullOrEmpty(entry.stringFormat) ? null : entry.stringFormat;
            bindingBase.FallbackValue = string.IsNullOrEmpty(entry.fallbackValue) ? null : entry.fallbackValue;
            bindingBase.Delay = entry.delay;

            //string targetPath = entry.targetPath;
            //if (string.IsNullOrEmpty(targetPath))
            //{
            //    if (entry.target != null)
            //    {
            //        targetPath = entry.target.GetType().GetDefaultMemberName();
            //    }
            //}

            //multi binding
            //if (string.IsNullOrEmpty(targetPath))
            //    throw new Exception("Binding Target Path Null, {0}".FormatArgs(gameObject.name));
            bindingBase.TargetPath = entry.targetPath;

            return bindingBase;
        }

        //IEnumerable<Tuple<BindingEntry, BindingBase>> ToBindings(IEnumerable<Entry1> entrys)
        //{
        //    if (items == null)
        //        yield break;
        //    foreach (var entry in entrys)
        //    {
        //        if (entry.bindings == null)
        //            continue;

        //        foreach (var bindingEntry in entry.bindings)
        //        {
        //            var binding = ToBinding(entry.type, bindingEntry);
        //            yield return new Tuple<BindingEntry, BindingBase>(bindingEntry, binding);

        //        }
        //    }
        //}

        [ContextMenu("Bind")]
        public void Bind()
        {
            if (isBinding)
                return;
            //gameObject.SendMessage("OnDataBinding", SendMessageOptions.DontRequireReceiver);
            //if (dataContext != null)
            //    contextData = dataContext.DataContext;
            //else
            //    contextData = null;

            if (bindings != null)
            {
                BindingBase binding;
                foreach (var item in bindings)
                {
                    binding = ToBinding(item.bindingType, item);
                    binding.Bind();
                    item.binding = binding;
                }
            }
            isBinding = true;
            //gameObject.SendMessage("OnDataBind", SendMessageOptions.DontRequireReceiver);
        }
        [ContextMenu("Unbind")]
        public void Unbind()
        {
            if (!isBinding)
                return;

            if (bindings != null)
            {
                BindingBase binding;
                foreach (var entry in bindings)
                {
                    binding = entry.binding;
                    if (binding != null)
                    {
                        binding.Unbind();
                        entry.binding = null;
                    }
                }
            }

            isBinding = false;
            //gameObject.SendMessage("OnDataUnbind", SendMessageOptions.DontRequireReceiver);
        }

        public static void Bind(GameObject target)
        {
            foreach (var binding in target.GetComponentsInChildren<Binding>())
            {
                binding.Bind();
            }
        }

        public static void Unbind(GameObject target)
        {
            foreach (var binding in target.GetComponentsInChildren<Binding>())
            {
                binding.Unbind();
            }
        }


        void OnDestroy()
        {

            if (isBinding)
                Unbind();
        }

        //private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (isBinding)
        //    {
        //        if (e.PropertyName == "DataContext")
        //        {
        //            var oldContextData = contextData;
        //            contextData = dataContext.DataContext;
        //            if (items != null)
        //            {
        //                foreach (var item in items)
        //                {
        //                    switch (item.type)
        //                    {
        //                        case BindingType.Binding:
        //                            if (item.bindings != null)
        //                            {
        //                                foreach (var bindingEntry in item.bindings)
        //                                {
        //                                    if (!bindings.ContainsKey(bindingEntry))
        //                                        continue;

        //                                    Binding binding = (Binding)bindings[bindingEntry];
        //                                    if (bindingEntry.source == null && binding.Source == oldContextData)
        //                                    {
        //                                        binding.Source = contextData;
        //                                    }
        //                                }
        //                            }
        //                            break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public static bool IsNull(object obj)
        {
            return obj == null || ReferenceEquals(obj, null) || obj.Equals(null);
        }

        protected object ResolveSource(Entry entry)
        {
            object result;
            result = entry.source;
            if (result.IsNull())
            {

                if (!string.IsNullOrEmpty(entry.sourceName))
                {
                    result = ResolveNameSource(entry);
                }
                else if (!string.IsNullOrEmpty(entry.sourceType))
                {
                    result = ResolveTypeSource(entry);
                }

            }

            if (result != null)
            {
                IDataContext ctx = result as IDataContext;
                if (ctx != null && ctx.DataContext != null)
                {
                    result = ctx.DataContext;
                }
            }
            return result;
        }

        protected bool ResolveDataContextSource(Entry entry, out Object result)
        {


            int level = entry.ancestorLevel;
            Transform t;
            IDataContext ctx = null;

            result = null;
            t = transform;

            while (t != null)
            {
                ctx = t.GetComponentInParent<IDataContext>();

                if (ctx == null)
                    break;

                Component c = ctx as Component;
                if (ctx.DataContext != null)
                {
                    if (--level < 0)
                    {
                        result = c;
                        break;
                    }
                }
                t = c.transform.parent;
            }

            if (result != null)
                return true;

            return false;
        }

        protected Object ResolveNameSource(Entry entry)
        {
            Object result = null;
            string sourceName = entry.sourceName;
            if (string.IsNullOrEmpty(sourceName))
                return null;

            int level = entry.ancestorLevel;
            Transform t;
            t = transform;

            while (t != null)
            {
                if (string.Equals(t.name, sourceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (--level < 0)
                    {
                        if (string.IsNullOrEmpty(entry.sourceType))
                        {
                            result = t.GetComponents<IDataContext>().Where(o => o.DataContext != null).FirstOrDefault() as Object;
                        }
                        else
                        {
                            if (entry.sourceType == "GameObject")
                                result = t.gameObject;
                            else
                                result = t.GetComponent(entry.sourceType);
                        }

                        break;
                    }
                }
                t = t.parent;
            }
            return result;
        }


        protected Object ResolveTypeSource(Entry entry)
        {
            Object result = null;
            if (string.IsNullOrEmpty(entry.sourceType))
                return null;

            int level = entry.ancestorLevel;
            Transform t = transform;

            if (entry.sourceType == "GameObject")
            {
                while (t != null)
                {
                    if (--level < 0)
                    {
                        result = t.GetComponents<IDataContext>().Where(o => o.DataContext != null).FirstOrDefault() as Object;
                        if (result == null)
                            result = t.gameObject;
                        break;
                    }
                    t = t.parent;
                }
            }
            else
            {
                while (t != null)
                {
                    Object c = t.GetComponent(entry.sourceType);
                    if (c)
                    {
                        if (--level < 0)
                        {
                            result = c;
                            break;
                        }
                    }
                    t = t.parent;
                }
            }

            return result;
        }

        /*
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
        }*//*

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
        }*/

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
        //[Serializable]
        //public class Entry1
        //{
        //    [SerializeField]
        //    public BindingType type;


        //    [SerializeField]
        //    public List<BindingEntry> bindings;
        //}

        [Serializable]
        public class Entry : ISerializationCallbackReceiver
        {

            public BindingType bindingType = BindingType.Binding;
            [ComponentPopup]
            public Object target;
            [MemberPopup("target", MemberPopupFlags.Field| MemberPopupFlags.Property)]
            public string targetPath;
            public string nullValue;
            public string stringFormat;
            public int delay;
            //public SourceType sourceType;
            [ComponentPopup]
            public Object source;
      
            [SerializeField]
            public string sourceName;
            [SerializeField]
            public string sourceType;
            [SerializeField]
            public int ancestorLevel;

            //public RelativeSourceEntry relativeSource;
            public FindNameEntry nameSource;
            [MemberPopup("source", MemberPopupFlags.Field | MemberPopupFlags.Property)]
            public string path;
            public BindingMode mode;
            public string fallbackValue;
            public string converter;
            public string converterParameter;
            public bool enabledSourceUpdated;
            public bool enabledTargetUpdated;

            internal BindingBase binding;

            //[SerializeField]
            //public ChildEntry[] children;
            //[NonSerialized]
            //[SerializeField]
            //public Entry[] Children;

            [SerializeField]
            private List<Entry> bindings;
            public List<Entry> Bindings
            {
                get
                {
                    if (bindings == null)
                        bindings = new List<Entry>();
                    return bindings;
                }
            }

            public Entry AddBinding(Entry entry)
            {
                if (!Bindings.Contains(entry))
                    Bindings.Add(entry);
                return this;
            }

            public void OnBeforeSerialize()
            {

            }

            public void OnAfterDeserialize()
            {
                //if (children == null)
                //    Children = null;
                //else
                //    Children = children.Select(o => o.ToEntry()).ToArray();
            }
        }

        //[Serializable]
        //public class ChildEntry
        //{
        //    [SerializeField]
        //    public BindingType type;

        //    [SerializeField]
        //    public List<ChildBindingEntry> bindings;

        //    public Entry ToEntry()
        //    {
        //        Entry entry = new Entry();
        //        entry.type = type;
        //        if (bindings != null)
        //            entry.bindings = bindings.Select(o => o.ToBindingEntry()).ToList();
        //        return entry;
        //    }

        //    public ChildEntry(Entry entry)
        //    {
        //        type = entry.type;
        //        if (bindings != null)
        //            bindings = entry.bindings.Select(o => new ChildBindingEntry(o)).ToList();

        //    }

        //}


        //[Serializable]
        //public class ChildBindingEntry
        //{

        //    public BindingType bindingType;

        //    public string nullValue;
        //    public string stringFormat;
        //    public int delay;

        //    public string sourceType;
        //    public Object source;
        //    public RelativeSourceEntry relativeSource;
        //    public FindNameEntry nameSource;
        //    public string path;
        //    public BindingMode mode;
        //    public string fallbackValue;
        //    public string converter;
        //    public string converterParameter;
        //    public bool notifyOnSourceUpdated;
        //    public bool notifyOnTargetUpdated;

        //    public BindingEntry ToBindingEntry()
        //    {
        //        BindingEntry binding = new BindingEntry();
        //        binding.bindingType = bindingType;

        //        binding.nullValue = nullValue;
        //        binding.stringFormat = stringFormat;
        //        binding.delay = delay;
        //        binding.sourceType = sourceType;
        //        binding.source = source;
        //        binding.relativeSource = relativeSource;
        //        binding.nameSource = nameSource;
        //        binding.path = path;
        //        binding.mode = mode;
        //        binding.fallbackValue = fallbackValue;
        //        binding.converter = converter;
        //        binding.converterParameter = converterParameter;
        //        binding.enabledSourceUpdated = notifyOnSourceUpdated;
        //        binding.enabledTargetUpdated = notifyOnTargetUpdated;
        //        return binding;
        //    }

        //    public ChildBindingEntry(BindingEntry binding)
        //    {
        //        bindingType = binding.bindingType;
        //        nullValue = binding.nullValue;
        //        stringFormat = binding.stringFormat;
        //        delay = binding.delay;
        //        sourceType = binding.sourceType;
        //        source = binding.source;
        //        relativeSource = binding.relativeSource;
        //        nameSource = binding.nameSource;
        //        path = binding.path;
        //        mode = binding.mode;
        //        fallbackValue = binding.fallbackValue;
        //        converter = binding.converter;
        //        converterParameter = binding.converterParameter;
        //        notifyOnSourceUpdated = binding.enabledSourceUpdated;
        //        notifyOnTargetUpdated = binding.enabledTargetUpdated;

        //    }

        //}

        //[Serializable]
        //public class RelativeSourceEntry
        //{

        //    public int ancestorLevel;

        //    public string typeName;

        //    public RelativeSourceMode mode;

        //}


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

        public void AddBinding(Entry entry)
        {
            //Entry1 entry = null;
            //if (items != null)
            //{
            //    entry = items.Where(o => o.type == bindingEntry.bindingType).FirstOrDefault();
            //}
            //if (entry == null)
            //{
            //    entry = new Entry1() { type = bindingEntry.bindingType, bindings = new List<BindingEntry>() };
            //    if (items == null)
            //    {
            //        items = new Entry1[] { entry };
            //    }
            //    else
            //    {
            //        var tmp = new Entry1[items.Length + 1];
            //        Array.Copy(items, tmp, items.Length);
            //        items = tmp;
            //        items[items.Length - 1] = entry;
            //    }
            ////}
            //if (!entry.bindings.Contains(bindingEntry))
            //    entry.bindings.Add(bindingEntry);
            if (!Bindings.Contains(entry))
            {
                Bindings.Add(entry);
            }
        }

    }

}