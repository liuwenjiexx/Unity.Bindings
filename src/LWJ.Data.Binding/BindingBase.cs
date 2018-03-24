using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

            if (string.IsNullOrEmpty(targetPath))
                throw new Exception("TargetPath Empty");

            targetBinder = PropertyPath.Create(targetPath);
            targetBinder.Target = target;
            targetBinder.ChangedCallback = UpdateTargetToSource;
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

        static Dictionary<string, Tuple<Type, IValueConverter>> converters;
        static Dictionary<string, Tuple<Type, IMultiValueConverter>> multiConverters;

        class ConverterItem
        {
            public Type type;
            public IValueConverter valueConverter;
            public IMultiValueConverter multiValueConverter;
        }
        static Dictionary<string, ConverterItem> cachedConverters;


        static BindingBase()
        {
            Init();
        }

        static void Init()
        {
            cachedConverters = new Dictionary<string, ConverterItem>(StringComparer.InvariantCultureIgnoreCase);
            converters = new Dictionary<string, Tuple<Type, IValueConverter>>();
            multiConverters = new Dictionary<string, Tuple<Type, IMultiValueConverter>>();

            //ImportAttribute.Import<BindingBase>();

            AddValueConverter("Boolean", typeof(BooleanConverter));
            AddValueConverter("StringFormat", typeof(StringFormatConverter));

            AddMultiValueConverter("Boolean", typeof(BooleanConverter));
            AddMultiValueConverter("Divide", typeof(DivideConverter));
            AddMultiValueConverter("StringJoin", typeof(StringJoinConverter));
            AddMultiValueConverter("StringFormat", typeof(StringFormatConverter));
            
        }

        public static void AddValueConverter(string name, Type valueConverterType)
        {
            if (!typeof(IValueConverter).IsAssignableFrom(valueConverterType))            
                throw new ArgumentException("Not Implemented Interface [IValueConverter]", nameof(valueConverterType));
             
            converters[name] = new Tuple<Type, IValueConverter>(valueConverterType, null);
        }

        public static void AddMultiValueConverter(string name, Type multiValueConverterType)
        {
            if (!typeof(IMultiValueConverter).IsAssignableFrom(multiValueConverterType))
                throw new ArgumentException("Not Implemented Interface [IMultiValueConverter]", nameof(multiValueConverterType));

            converters[name] = new Tuple<Type, IValueConverter>(multiValueConverterType, null);
        }


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
        public static string[] GetValueConveterNames()
        {
            return converters.Keys.ToArray();
        }

        public static string[] GetMultiValueConveterNames()
        {
            return multiConverters.Keys.ToArray();
        }
        public static IValueConverter GetValueConveter(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            IValueConverter result = null;
            Tuple<Type, IValueConverter> item;
            if (converters.TryGetValue(name, out item))
            {
                result = item.Item2;
                if (result == null)
                {
                    result = (IValueConverter)Activator.CreateInstance(item.Item1);
                    converters[name] = new Tuple<Type, IValueConverter>(item.Item1, result);
                }
            }
            else
            {
                throw new Exception("Not IValueConverter {0}".FormatArgs(name));
            }
            return result;
        }

        public static IMultiValueConverter GetMultiValueConveter(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            IMultiValueConverter result = null;
            Tuple<Type, IMultiValueConverter> item;
            if (multiConverters.TryGetValue(name, out item))
            {
                result = item.Item2;
                if (result == null)
                {
                    result = (IMultiValueConverter)Activator.CreateInstance(item.Item1);
                    multiConverters[name] = new Tuple<Type, IMultiValueConverter>(item.Item1, result);
                }
            }
            else
            {
                throw new Exception("Not IMultiValueConverter {0}".FormatArgs(name));
            }
            return result;
        }

        #endregion
    }
}
