using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel;

namespace LWJ.Data
{

    public class PathAccess : IDisposable
    {
        private object target;
        private Type targetType;
        private string memberName;
        //private bool allowListenerChanged;
        public Action ChangedCallback;
        private bool isListenerChanged;
        private AccessMemberType accessMemberType;
        private IMemberAccess access;
        private Type memberType;
        private PathAccess next;
        private PathAccess first;
        private PathAccess last;
        private bool isIndexer;
        private int index;
        private bool isValueType;


        public PathAccess(string memberName)
            : this(memberName, false)
        {
        }

        public PathAccess(string memberName, bool isIndexer)
        {
            //if (memberName == null)
            //    throw new ArgumentNullException("memberName");


            this.memberName = memberName;
            this.isIndexer = isIndexer;
            if (isIndexer)
            {
                if (!int.TryParse(memberName, out index))
                    throw new ArgumentException("indexer memberName error:" + memberName);
            }
        }




        private enum AccessMemberType
        {
            None,
            Member,
            //Array,
            Collection,
            //IEnumerable,
        }


        public object Target
        {
            get { return target; }
            set
            {
                SetTarget(value);
            }
        }



        //public Type TargetType { get => targetType; }


        public string Path
        {
            get
            {
                //StringBuilder sb = new StringBuilder();
                //MemberBinder current = this;
                //while (current != null)
                //{
                //    if (isIndexer)
                //    {
                //        sb.Append('[').Append(memberName).Append(']');
                //    }
                //    else if (current.memberName != null)
                //    {
                //        if (sb.Length > 0)
                //            sb.Append('.');
                //        sb.Append(current.memberName);
                //    }
                //    current = current.next;
                //}            
                //return sb.ToString();
                if (isIndexer)
                    return "[" + memberName + "]";
                return memberName + (next != null ? "." + next.ToString() : "");
            }
        }




        public string MemberName { get => memberName; }
        /*       public bool AllowListenerChanged
               {
                   get
                   {

                       return allowListenerChanged;
                   }

                   set
                   {
                       if (allowListenerChanged != value)
                       {
                           allowListenerChanged = value;
                           if (allowListenerChanged)
                           {
                               ListenerChanged();
                           }
                           else
                           {
                               ReleaseListenerChanged();
                           }

                           if (next != null)
                               next.AllowListenerChanged = value;
                       }
                   }
               }*/
        public Type MemberType { get => memberType; }
        public int Index { get => index; }

        public bool IsIndexer { get => isIndexer; }

        public bool CanSetValue()
        {
            if (last != this)
                return last.CanSetValue();
            if (target == null || access == null)
                return false;
            return true;
        }



        private bool SetTarget(object value)
        {
            if (target == value)
            {
                return false;
            }
            ReleaseListenerChanged();
            target = value;

            if (target != null)
            {
                Type oldTargetType = target.GetType();

                if (oldTargetType != targetType)
                {
                    accessMemberType = AccessMemberType.None;
                    memberType = null;
                    access = null;

                    if (oldTargetType.IsValueType && !oldTargetType.IsPrimitive)
                        isValueType = true;
                    else
                        isValueType = false;

                    if (memberName == null)
                    {
                        accessMemberType = AccessMemberType.None;
                        memberType = targetType;
                        access = new SelfAccess();
                    }
                    else if (!isIndexer)
                    {
                        var property = oldTargetType.GetProperty(memberName);

                        if (property != null)
                        {
                            access = new PropertyAccess(property);
                            accessMemberType = AccessMemberType.Member;
                            memberType = property.PropertyType;
                        }
                        else
                        {
                            var field = oldTargetType.GetField(memberName);
                            if (field != null)
                            {
                                access = new FieldAccess(field);
                                accessMemberType = AccessMemberType.Member;
                                memberType = field.FieldType;
                            }
                            //else
                            //{
                            //    throw new Exception("Not Found Member. Type: {0}, Member: {1}".FormatArgs(dataType.FullName, memberName));
                            //}
                        }
                    }
                    else
                    {
                        if (oldTargetType.IsArray)
                        {
                            accessMemberType = AccessMemberType.Collection;
                            memberType = oldTargetType.GetElementType();
                            access = new ArrayAccess() { index = index };
                        }
                        else if (typeof(IList).IsAssignableFrom(oldTargetType))
                        {
                            accessMemberType = AccessMemberType.Collection;
                            if (oldTargetType.IsGenericType)
                                memberType = oldTargetType.GetGenericArguments()[0];
                            else
                                memberType = typeof(object);
                            access = new ListAccess() { index = index };
                        }
                        else if (typeof(IEnumerable).IsAssignableFrom(oldTargetType))
                        {
                            accessMemberType = AccessMemberType.Collection;
                            memberType = typeof(object);
                            access = new IEnumerableAccess() { index = index };
                        }
                        else if (typeof(IEnumerator).IsAssignableFrom(oldTargetType))
                        {
                            accessMemberType = AccessMemberType.Collection;
                            memberType = typeof(object);
                            access = new IEnumeratorAccess() { index = index };
                        }
                    }

                    targetType = oldTargetType;
                }

                ListenerChanged();
            }
            else
            {
                accessMemberType = AccessMemberType.None;
                targetType = null;
                access = null;
            }


            OnMemberValueChanged();
            return true;
        }



        void ListenerChanged()
        {
            //if (!allowListenerChanged)
            //    return;

            if (isListenerChanged || memberName == null)
                return;

            if (accessMemberType == AccessMemberType.Member)
            {
                if (target is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)target).PropertyChanged += Target_PropertyChanged;
                    isListenerChanged = true;
                }
            }
            else if (accessMemberType == AccessMemberType.Collection)
            {
                if (target is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)target).CollectionChanged += Target_CollectionChanged;
                    isListenerChanged = true;
                }
            }
        }



        void ReleaseListenerChanged()
        {
            if (!isListenerChanged)
                return;
            if (target != null)
            {
                if (accessMemberType == AccessMemberType.Member)
                {

                    if (target is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged)target).PropertyChanged -= Target_PropertyChanged;
                    }
                }
                else if (accessMemberType == AccessMemberType.Collection)
                {
                    if (target is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)target).CollectionChanged -= Target_CollectionChanged;
                    }
                }
            }

            isListenerChanged = false;
        }

        private void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == memberName)
            {
                OnMemberValueChanged();
            }
        }


        private void Target_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (index < 0)
                return;

            bool changed = false;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex <= index)
                        changed = true;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex <= index)
                        changed = true;
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.NewStartingIndex <= index)
                        changed = true;
                    else if (e.OldStartingIndex <= index)
                        changed = true;
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.NewStartingIndex <= index && index <= e.NewStartingIndex + e.NewItems.Count)
                        changed = true;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    changed = true;
                    break;
            }

            if (changed)
            {
                OnMemberValueChanged();
            }
        }

        void OnMemberValueChanged()
        {

            if (next != null)
            {
                object value;
                if (!TryGetMemberValue(out value))
                {
                    value = null;
                }
                if (next.SetTarget(value))
                    return;
            }


            if (first == this)
            {
                if (ChangedCallback != null)
                    ChangedCallback();
            }
            else
            {
                if (first.ChangedCallback != null)
                    first.ChangedCallback();
            }

        }


        //public MemberBinder FindSetTarget()
        //{
        //    var current = this;
        //    while (current.target != null && current.next != null)
        //    {
        //        current = current.next;
        //    }

        //    if (current.target == null || current.next != null)
        //        return null;

        //    return current;
        //}


        private bool TrySetMemberValue(object value)
        {
            if (target == null || access == null)
                return false;
            if (value == null)
            {
                value = memberType.GetDefaultValue();
            }

            return access.SetValue(target, value);
        }

        private bool TryGetMemberValue(out object value)
        {
            if (access == null || target == null)
            {
                value = null;
                return false;
            }
            return access.GetValue(target, out value);
        }

        public Type GetValueType()
        {
            return last.memberType;
        }

        public bool TryGetValue(out object value)
        {
            return last.TryGetMemberValue(out value);
        }


        public bool TrySetValue(object value)
        {
            if (next != null)
            {
                //object nextTarget;
                //if (!GetMemberValue(out nextTarget))
                //    return false;

                //next.Target = nextTarget;
                return next.TrySetValue(value);
            }

            return TrySetMemberValue(value);
        }


        public static PathAccess Create(string path)
        {
            PathAccess first = null;
            if (string.IsNullOrEmpty(path) || path == ".")
            {
                first = new PathAccess(null);
                first.first = first;
                first.last = first;
                return first;
            }

            string[] propertys = path.Split('.');

            PathAccess last = null;
            foreach (var p in propertys)
            {
                var parts = p.Trim().Split('[');


                string propName;

                for (int i = 0; i < parts.Length; i++)
                {
                    propName = parts[i].Trim();
                    if (propName.Length == 0)
                        continue;
                    bool isIndexer = false;
                    if (i > 0)
                    {
                        propName = propName.Substring(0, propName.Length - 1);
                        isIndexer = true;
                    }


                    PathAccess binder = new PathAccess(propName, isIndexer);
                    if (first == null)
                    {
                        first = binder;
                    }
                    else
                    {
                        last.next = binder;

                    }
                    last = binder;
                }

            }
            PathAccess current = first;
            while (current != null)
            {
                current.first = first;
                current.last = last;
                current = current.next;
            }

            return first;
        }


        public void Dispose()
        {
            if (next != null)
            {
                next.Dispose();
                next = null;
            }
            if (isListenerChanged)
                ReleaseListenerChanged();
        }

        public override string ToString()
        {
            if (isIndexer)
                return "[" + memberName + "]";
            return memberName + (next != null ? "." + next.ToString() : "");
        }



        ~PathAccess()
        {
            Dispose();
        }

        #region IMemberAccess

        interface IMemberAccess
        {
            bool GetValue(object target, out object value);
            bool SetValue(object target, object value);
        }

        class FieldAccess : IMemberAccess
        {
            public FieldInfo field;
            public FieldAccess(FieldInfo field)
            {
                this.field = field;
            }

            public bool GetValue(object target, out object value)
            {
                value = field.GetValue(target);
                return true;
            }

            public bool SetValue(object target, object value)
            {
                if (field.IsInitOnly)
                    return false;
                field.SetValue(target, value);
                return true;
            }
        }

        class PropertyAccess : IMemberAccess
        {
            MethodInfo getter;
            MethodInfo setter;
            public PropertyAccess(PropertyInfo property)
            {
                getter = property.GetGetMethod(false);
                setter = property.GetSetMethod(false);
            }
            public bool GetValue(object target, out object value)
            {
                if (getter == null)
                {
                    value = null;
                    return false;
                }
                value = getter.Invoke(target, null);
                return true;
            }

            public bool SetValue(object target, object value)
            {
                if (setter == null)
                    return false;
                setter.Invoke(target, new object[] { value });
                return true;
            }
        }


        class ArrayAccess : IMemberAccess
        {

            public int index;
            public bool GetValue(object target, out object value)
            {
                Array array = (Array)target;
                if (index >= 0 && index < array.Length)
                {
                    value = array.GetValue(index);
                    return true;
                }
                value = null;
                return false;
            }

            public bool SetValue(object target, object value)
            {
                Array array = (Array)target;
                if (index >= 0 && index < array.Length)
                {
                    array.SetValue(value, index);
                    return true;
                }
                return false;
            }
        }

        class CollectionAccess : IEnumeratorAccess
        {

            public override bool GetValue(object target, out object value)
            {
                ICollection array = (ICollection)target;
                if (index < 0 || index >= array.Count)
                {
                    value = null;
                    return false;
                }
                return GetValue(array.GetEnumerator(), out value);
            }

        }

        class ListAccess : IMemberAccess
        {

            public int index;
            public bool GetValue(object target, out object value)
            {
                IList list = (IList)target;
                if (index >= 0 && index < list.Count)
                {
                    value = list[index];
                    return true;
                }
                value = null;
                return false;
            }

            public bool SetValue(object target, object value)
            {
                IList list = (IList)target;
                if (index >= 0 && index < list.Count)
                {
                    list[index] = value;
                    return true;
                }
                return false;
            }
        }

        class IEnumerableAccess : IEnumeratorAccess
        {
            public override bool GetValue(object target, out object value)
            {
                if (index < 0)
                {
                    value = null;
                    return false;
                }
                IEnumerable items = (IEnumerable)target;
                return base.GetValue(items.GetEnumerator(), out value);
            }

        }

        class IEnumeratorAccess : IMemberAccess
        {
            public int index;
            public virtual bool GetValue(object target, out object value)
            {
                if (index < 0)
                {
                    value = null;
                    return false;
                }
                return GetValue((IEnumerator)target, out value);
            }

            public bool GetValue(IEnumerator it, out object value)
            {
                int n = 0;
                value = null;
                bool hasValue = false;
                while (it.MoveNext())
                {
                    if (n == index)
                    {
                        hasValue = true;
                        value = it.Current;
                        break;
                    }
                    n++;
                }

                return hasValue;
            }

            public virtual bool SetValue(object target, object value)
            {
                return false;
            }
        }

        class SelfAccess : IMemberAccess
        {
            public bool GetValue(object target, out object value)
            {
                value = target;
                return true;
            }

            public bool SetValue(object target, object value)
            {
                return false;
            }
        }

        #endregion


    }


}
