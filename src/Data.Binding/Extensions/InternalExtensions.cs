using System;
using System.Collections.Generic;
using System.Reflection;

namespace LWJ.Data
{
    internal static partial class InternalExtensions
    {
        public static string ToStringOrEmpty(this object source)
        {
            string result;
            if (source == null)
            {
                result = string.Empty;
            }
            else
            {
                result = source.ToString();
                if (result == null)
                    result = string.Empty;
            }
            return result;
        }

        public static string FormatArgs(this string source, params object[] args)
        {
            return string.Format(source, args);
        }

        public static T GetCustomAttribute<T>(this ICustomAttributeProvider member, bool inherit)
            where T : Attribute
        {
            var attrs = member.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return (T)attrs[0];
            return null;
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider member, bool inherit)
            where T : Attribute
        {
            var attrs = member.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
            {
                T[] result = new T[attrs.Length];
                for (int i = 0; i < attrs.Length; i++)
                    result[i] = (T)attrs[i];
                return result;
            }
            return new T[0];
        }


        public static object GetDefaultValue(this Type type)
        {
            if (type == null) throw new NullReferenceException();
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }


        public static TValue GetOrCreateValue<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TKey, TValue> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            TValue value;
            if (!self.TryGetValue(key, out value))
            {
                value = factory(key);
                self[key] = value;
            }
            return value;
        }


        public static Type FindType(this string typeName)
        {
            Type type;
            type = Type.GetType(typeName, false);
            if (type == null)
            {
                if (typeName.IndexOf(',') < 0)
                {
                    foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        type = ass.GetType(typeName, false);
                        if (type != null)
                            break;
                    }
                }
            }
            return type;
        }

    }



}
