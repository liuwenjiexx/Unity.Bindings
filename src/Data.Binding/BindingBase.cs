using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LWJ.Data
{
    public abstract class BindingBase : IDisposable
    {
        private object target;
        private string targetPath;
        internal PropertyPath targetBinder;
        private bool isNullValue;
        private bool isFallbackValue;
        private string stringFormat;
        private object nullValue;
        private object fallbackValue;
        private int delay;

        private static Dictionary<Type, string> defaultMenbers;
        private static Dictionary<string, ConverterInfo> cachedConverters;
        private static Dictionary<Type, ConverterInfo> cachedTypeConverters;

        static BindingBase()
        {
            Init();
        }

        public object Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    //if (targetBinder != null)
                    //{
                    //    targetBinder.Target = target;
                    //    UpdateSourceToTarget();
                    //}
                }
            }
        }

        public string TargetPath { get => targetPath; set => targetPath = value; }

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

        public int Delay { get => delay; set => delay = value; }

        public virtual void Bind()
        {
            Unbind();

            if (target == null)
                throw new Exception(string.Format("{0} Null", nameof(Target)));

            string targetPath = this.targetPath;

            if (string.IsNullOrEmpty(targetPath))
            {
                if (target != null)
                    targetPath = GetDefaultMember(target.GetType());
                // throw new Exception("TargetPath Empty"+targetPath);
            }


            targetBinder = PropertyPath.Create(targetPath);
            targetBinder.Target = target;
            targetBinder.TargetUpdatedCallback = UpdateTargetToSource;
        }

        public virtual void Unbind()
        {

            if (targetBinder != null)
            {
                targetBinder.Dispose();
                targetBinder = null;
            }

        }

        protected abstract void UpdateSourceToTarget();


        protected abstract void UpdateTargetToSource();


        public virtual void Dispose()
        {
            Unbind();
        }

        ~BindingBase()
        {
            Dispose();
        }

        /*
     //   [Import("69225e0c-f0a5-4103-bc11-608062ed47fa",IsCollection =true)]
        private static Dictionary<Type, string> cachedDefaultMembers;

        public static string GetDefaultMember(Type type)
        {
            string defaultMember = null;

            if (cachedDefaultMembers == null)
            {
                ImportAttribute.ImportStatic(typeof(BindingBase)); 
            }
            if (!cachedDefaultMembers.TryGetValue(type, out defaultMember))
            {
                var defaultMemberAttr = type.GetCustomAttribute<DefaultMemberAttribute>(true);
                if (defaultMemberAttr != null)
                {
                    defaultMember = defaultMemberAttr.MemberName;
                }
                cachedDefaultMembers[type] = defaultMember;
            }
            return defaultMember;
        }
        */


        #region Converters
             

        class ConverterInfo
        {
            public string Name;
            public IValueConverter valueConverter;
            public IMultiValueConverter multiValueConverter;
            public Type valueConverterType;
            public Type multiValueConverterType;
        }

        static void Init()
        {
            cachedConverters = new Dictionary<string, ConverterInfo>();
            cachedTypeConverters = new Dictionary<Type, ConverterInfo>();
            //converters = new Dictionary<string, Tuple<Type, IValueConverter>>();
            //multiConverters = new Dictionary<string, Tuple<Type, IMultiValueConverter>>();
            defaultMenbers = new Dictionary<Type, string>();
            //ImportAttribute.Import<BindingBase>();

            //AddValueConverter("Boolean", typeof(BooleanConverter));
            //AddValueConverter("StringFormat", typeof(StringFormatConverter));

            //AddMultiValueConverter("Boolean", typeof(BooleanConverter));
            //AddMultiValueConverter("Divide", typeof(DivideConverter));
            //AddMultiValueConverter("StringJoin", typeof(StringJoinConverter));
            //AddMultiValueConverter("StringFormat", typeof(StringFormatConverter));


            foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                InitAssembly(ass);
            }
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            InitAssembly(args.LoadedAssembly);
        }

        public static void InitAssembly(Assembly assembly)
        {
            Type converterAttrType = typeof(ConverterAttribute);

            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsDefined(converterAttrType, true))
                    continue;
                foreach (ConverterAttribute converterAttr in type.GetCustomAttributes(converterAttrType, true))
                {
                    if (!string.IsNullOrEmpty(converterAttr.ConverterName))
                    {
                        ConverterInfo cInfo = new ConverterInfo();
                        cInfo.Name = converterAttr.ConverterName;

                        if (typeof(IValueConverter).IsAssignableFrom(type))
                        {
                            cInfo.valueConverterType = type;
                        }
                        if (typeof(IMultiValueConverter).IsAssignableFrom(type))
                        {
                            cInfo.multiValueConverterType = type;
                        }
                        cachedConverters[converterAttr.ConverterName] = cInfo;
                    }
                }
            }
        }



        //public static void AddValueConverter(string name, Type valueConverterType)
        //{
        //    if (!typeof(IValueConverter).IsAssignableFrom(valueConverterType))
        //        throw new ArgumentException(string.Format("Not Implemented Interface [{0}]", nameof(IValueConverter)), nameof(valueConverterType));

        //    converters[name] = new Tuple<Type, IValueConverter>(valueConverterType, null);
        //}

        //public static void AddMultiValueConverter(string name, Type multiValueConverterType)
        //{
        //    if (!typeof(IMultiValueConverter).IsAssignableFrom(multiValueConverterType))
        //        throw new ArgumentException(string.Format("Not Implemented Interface [{0}]", nameof(IMultiValueConverter)), nameof(multiValueConverterType));

        //    multiConverters[name] = new Tuple<Type, IMultiValueConverter>(multiValueConverterType, null);
        //}
        //public static void AddValueConverter(Type type, Type valueConverterType)
        //{
        //    if (type == null)
        //        throw new ArgumentNullException(nameof(type));
        //    if (valueConverterType == null)
        //        throw new ArgumentNullException(nameof(valueConverterType));
        //    if (!typeof(IValueConverter).IsAssignableFrom(valueConverterType))
        //        throw new ArgumentException(string.Format("Not Implemented Interface [{0}]", nameof(IValueConverter)), nameof(valueConverterType));
        //    ConverterInfo cInfo;
        //    cInfo = cachedTypeConverters.GetOrCreateValue(type, (k) => new ConverterInfo());
        //    cInfo.valueConverterType = valueConverterType;
        //}

        //public static void AddMultiValueConverter(Type type, Type multiValueConverterType)
        //{
        //    if (type == null)
        //        throw new ArgumentNullException(nameof(type));
        //    if (multiValueConverterType == null)
        //        throw new ArgumentNullException(nameof(multiValueConverterType));
        //    if (!typeof(IMultiValueConverter).IsAssignableFrom(multiValueConverterType))
        //        throw new ArgumentException(string.Format("Not Implemented Interface [{0}]", nameof(IMultiValueConverter)), nameof(multiValueConverterType));
        //    ConverterInfo cInfo;
        //    cInfo = cachedTypeConverters.GetOrCreateValue(type, (k) => new ConverterInfo());
        //    cInfo.multiValueConverterType = multiValueConverterType;
        //}

        /*
        [Import(typeof(IValueConverter), Multiple = true)]
        static void ImportValueConverter(IEnumerable items)
        {
            string name;
            foreach (var item in items)
            {
                if (item is Type)
                {
                    Type type = (Type)item;
                    var attr = type.GetCustomAttribute<NamedAttribute>(true);
                    if (attr != null)
                    {
                        name = attr.Name;
                    }
                    else
                    {
                        name = type.Name;
                        if (name.EndsWith("Converter"))
                            name = name.Substring(0, name.Length - 9);
                    }

                    converters[name] = new Tuple<Type, IValueConverter>(type, null);
                }
            }
        }
        [Import(typeof(IMultiValueConverter), Multiple = true)]
        static void ImportMultiValueConverter(IEnumerable items)
        {
            string name;
            foreach (var item in items)
            {
                if (item is Type)
                {
                    Type type = (Type)item;
                    var attr = type.GetCustomAttribute<NamedAttribute>(true);
                    if (attr != null)
                    {
                        name = attr.Name;
                    }
                    else
                    {
                        name = type.Name;
                        if (name.EndsWith("Converter"))
                            name = name.Substring(0, name.Length - 9);
                    }

                    multiConverters[name] = new Tuple<Type, IMultiValueConverter>(type, null);
                }
            }
        }
        */
        public static IEnumerable<string> GetValueConveterNames()
        {
            return from o in cachedConverters.Values
                   where o.valueConverterType != null
                   select o.Name;
        }

        public static IEnumerable<string> GetMultiValueConveterNames()
        {
            return from o in cachedConverters.Values
                   where o.multiValueConverterType != null
                   select o.Name;
        }
        public static IValueConverter GetValueConveter(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            IValueConverter result = null;
            ConverterInfo cInfo;
            if (!cachedConverters.TryGetValue(name, out cInfo))
            {
                Type type = name.FindType();
                if (type != null)
                {
                    if (typeof(IValueConverter).IsAssignableFrom(type))
                    {
                        cInfo = new ConverterInfo();
                        cInfo.valueConverterType = type;
                        cachedConverters[name] = cInfo;
                    }
                }

            }

            if (cInfo != null)
            {
                if (cInfo.valueConverter == null && cInfo.valueConverterType != null)
                {
                    cInfo.valueConverter = (IValueConverter)Activator.CreateInstance(cInfo.valueConverterType);
                }
                result = cInfo.valueConverter;
            }

            if (result == null)
            {
                throw new Exception("Not {0} {1}".FormatArgs(nameof(IValueConverter), name));
            }

            return result;
        }

        public static IMultiValueConverter GetMultiValueConveter(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            IMultiValueConverter result = null;
            ConverterInfo cInfo;
            if (!cachedConverters.TryGetValue(name, out cInfo))
            {
                Type type = name.FindType();
                if (type != null)
                {
                    if (typeof(IMultiValueConverter).IsAssignableFrom(type))
                    {
                        cInfo = new ConverterInfo();
                        cInfo.multiValueConverterType = type;
                        cachedConverters[name] = cInfo;
                    }
                }

            }

            if (cInfo != null)
            {
                if (cInfo.multiValueConverter == null && cInfo.multiValueConverterType != null)
                {
                    cInfo.multiValueConverter = (IMultiValueConverter)Activator.CreateInstance(cInfo.multiValueConverterType);
                }
                result = cInfo.multiValueConverter;
            }

            if (result == null)
            {
                throw new Exception("Not {0} {1}".FormatArgs(nameof(IMultiValueConverter), name));
            }
            return result;
        }

        //public static IValueConverter GetValueConveter(Type type)
        //{
        //    if (type == null) throw new ArgumentNullException(nameof(type));
        //    IValueConverter result = null;
        //    ConverterInfo cInfo;
        //    if (!cachedTypeConverters.TryGetValue(type, out cInfo))
        //        throw new Exception("Not {0} {1}".FormatArgs(nameof(IValueConverter), type));

        //    if (cInfo.valueConverter == null && cInfo.valueConverterType != null)
        //    {
        //        cInfo.valueConverter = (IValueConverter)Activator.CreateInstance(cInfo.valueConverterType);
        //    }
        //    result = cInfo.valueConverter;

        //    return result;
        //}

        //public static IMultiValueConverter GetMultiValueConveter(Type type)
        //{
        //    if (type == null) throw new ArgumentNullException(nameof(type));
        //    IMultiValueConverter result = null;
        //    ConverterInfo cInfo;
        //    if (!cachedTypeConverters.TryGetValue(type, out cInfo))
        //        throw new Exception("Not {0} {1}".FormatArgs(nameof(IMultiValueConverter), type));

        //    if (cInfo.multiValueConverter == null && cInfo.multiValueConverterType != null)
        //    {
        //        cInfo.multiValueConverter = (IMultiValueConverter)Activator.CreateInstance(cInfo.multiValueConverterType);
        //    }
        //    result = cInfo.multiValueConverter;
        //    return result;
        //}

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

        #endregion
    }
}
