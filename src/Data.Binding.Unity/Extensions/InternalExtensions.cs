using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using UnityEngine;

namespace LWJ.Unity
{
    internal static partial class InternalExtensions
    {
        public static bool IsNull(this object obj)
        {
            if (obj is UnityEngine.Object)
                return !(UnityEngine.Object)obj;
            return obj == null;
        }

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
        public static void Invoke(this PropertyChangedEventHandler source, object thisObj, string propertyName)
        {
            var propertyChanged = source;
            if (propertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                propertyChanged(thisObj, args);
            }
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type == null) throw new NullReferenceException();
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

        public static string GetDefaultMemberName(this Type type)
        {
            var attrMember = type.GetCustomAttribute<DefaultMemberAttribute>(true);
            if (attrMember == null)
                return null;
            return attrMember.MemberName;
        }
        public static Transform FindByName(this Transform t, string name)
        {
            Transform result = null;
            foreach (Transform child in t)
            {
                if (child.name == name)
                {
                    result = child;
                    break;
                }
                result = FindByName(child, name);
                if (result != null)
                    break;
            }

            return result;
        }

        public static object GetValueUnity(this PropertyInfo source, object obj/*, object[] index*/)
        {
            if (source == null) throw new NullReferenceException();

            var getter = source.GetGetMethod(true);
            if (getter == null) throw new MemberAccessException("Property Not Get Method");

            object value;
            value = getter.Invoke(obj, null);
            return value;
        }

        public static void SetValueUnity(this PropertyInfo source, object obj, object value/*, object[] index*/)
        {
            if (source == null) throw new NullReferenceException();

            var setter = source.GetSetMethod(true);
            if (setter == null) throw new MemberAccessException("Property Not Set Method");

            setter.Invoke(obj, new object[] { value });
        }

    }



    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NamedAttribute : Attribute
    {
        public NamedAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
