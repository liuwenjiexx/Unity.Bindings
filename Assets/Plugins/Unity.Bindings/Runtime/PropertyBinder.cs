using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace Yanmonet.Bindings
{

    internal class PropertyBinder : IDisposable
    {
        private object target;
        private Type targetType;
        private string memberName;
        public Action TargetUpdatedCallback;
        public BindingNotifyDelegate targetPropertyChanged;

        private AccessMemberType accessMemberType;
        private IAccessor accessor;
        private Type memberType;
        private PropertyBinder prev;
        private PropertyBinder next;
        private PropertyBinder first;
        private PropertyBinder last;
        private bool isIndexer;
        private int? index;
        private Flags flags;
        private enum Flags
        {
            None = 0,
            AllowListenerMember = 1,
            ListenerMember = 2,
        }

        public PropertyBinder(string memberName)
            : this(memberName, false)
        {
        }

        public PropertyBinder(string memberName, bool isIndexer)
        {
            //if (memberName == null)
            //    throw new ArgumentNullException("memberName");


            this.memberName = memberName;
            this.isIndexer = isIndexer;

            if (isIndexer)
            {
                if (int.TryParse(memberName, out var n))
                    index = n;
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
                //if (isIndexer)
                //    return "[" + memberName + "]";
                //return memberName + (next != null ? "." + next.ToString() : "");
                string str = "";
                if (prev != null)
                    str = prev.ToString();
                if (isIndexer)
                {
                    str += "[" + memberName + "]";
                }
                else
                {
                    if (prev != null)
                        str += ".";
                    str += memberName;
                }
                return str;
            }
        }

        //public string MemberName { get => memberName; }
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
        //public Type MemberType { get => memberType; }
        //public int Index { get => index; }

        //public bool IsIndexer { get => isIndexer; }

        public bool CanSetValue
        {
            get
            {
                if (last != this)
                    return last.CanSetValue;
                if (target == null || accessor == null)
                    return false;
                return true;
            }
        }

        public PropertyBinder Last
        {
            get => last;
        }

        public string SelfMemberName => memberName;

        public string MemberName => Last.memberName;
        bool supportNotify;
        public bool SupportNotify => Last.supportNotify;

        private bool SetTarget(object value, Flags prevFlags = Flags.None)
        {
            if (object.Equals(target, value))
            {
                return false;
            }
            ReleaseListenerChanged();
            target = value;
            supportNotify = false;
            if (target != null)
            {
                Type oldTargetType = targetType;
                targetType = target.GetType();

                if (targetType != oldTargetType)
                {
                    accessMemberType = AccessMemberType.None;
                    memberType = null;
                    accessor = null;
                    flags = Flags.None;
                    if (!targetType.IsValueType)
                    {
                        if (first == this || (prevFlags & Flags.ListenerMember) == Flags.ListenerMember)
                        {
                            flags |= Flags.AllowListenerMember;
                        }
                    }

                    if (memberName == null)
                    {
                        accessMemberType = AccessMemberType.None;
                        memberType = targetType;
                        accessor = new ThisAccessor();
                        flags &= ~Flags.AllowListenerMember;
                    }
                    else if (!isIndexer)
                    {
                        var property = targetType.GetProperty(memberName);

                        if (property != null)
                        {
                            accessor = property.GetAccessor(); ;
                            accessMemberType = AccessMemberType.Member;
                            memberType = property.PropertyType;
                        }
                        else
                        {
                            var field = targetType.GetField(memberName);
                            if (field != null)
                            {
                                accessor = field.GetAccessor();
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
                        if (index.HasValue)
                        {
                            if (targetType.IsArray)
                            {
                                accessMemberType = AccessMemberType.Collection;
                                memberType = targetType.GetElementType();
                                accessor = Accessor.Array(index.Value);
                            }
                            else if (typeof(IList).IsAssignableFrom(targetType))
                            {
                                accessMemberType = AccessMemberType.Collection;
                                if (targetType.IsGenericType)
                                    memberType = targetType.GetGenericArguments()[0];
                                else
                                    memberType = typeof(object);
                                accessor = Accessor.List(index.Value);
                            }
                            else
                            {
                                // this[int index]
                                accessor = Accessor.Indexer(targetType, index.Value);
                            }
                             
                            if (accessor == null)
                            {
                                if (typeof(IEnumerable).IsAssignableFrom(targetType))
                                {
                                    accessMemberType = AccessMemberType.Collection;
                                    memberType = typeof(object);
                                    accessor = Accessor.Enumerable(index.Value);
                                }
                                else if (typeof(IEnumerator).IsAssignableFrom(targetType))
                                {
                                    accessMemberType = AccessMemberType.Collection;
                                    memberType = typeof(object);
                                    accessor = Accessor.Enumerable(index.Value);
                                }
                            }
                        }
                        else
                        {
                            // this[string index]
                            accessor = Accessor.Indexer(targetType, memberName);
                        }

                    }

                }

                ListenerChanged();
            }
            else
            {
                flags = Flags.None;
                accessMemberType = AccessMemberType.None;
                targetType = null;
                accessor = null;
            }


            OnMemberValueChanged();
            return true;
        }



        void ListenerChanged()
        {
            supportNotify = false;

            if (((flags & Flags.ListenerMember) != 0) || memberName == null || ((flags & Flags.AllowListenerMember) == 0))
                return;

            if (targetPropertyChanged != null)
            {
                targetPropertyChanged(Target_PropertyChanged, true);
                flags |= Flags.ListenerMember;
                supportNotify = true;
            }
            else if (target != null)
            {
                if (accessMemberType == AccessMemberType.Member)
                {
                    if (target is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged)target).PropertyChanged += Target_PropertyChanged;
                        flags |= Flags.ListenerMember;
                        supportNotify = true;
                    }
                }
                else if (accessMemberType == AccessMemberType.Collection)
                {
                    if (target is INotifyCollectionChanged)
                    {
                        ((INotifyCollectionChanged)target).CollectionChanged += Target_CollectionChanged;
                        flags |= Flags.ListenerMember;
                        supportNotify = true;
                    }
                }
            }
        }



        void ReleaseListenerChanged()
        {
            if ((flags & Flags.ListenerMember) == 0)
                return;
            supportNotify = false;
            if (targetPropertyChanged != null)
            {
                targetPropertyChanged(Target_PropertyChanged, false);
            }
            else if (target != null)
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

            flags &= ~Flags.ListenerMember;
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
                if (next.SetTarget(value, flags))
                    return;
            }

            if (first.TargetUpdatedCallback != null)
                first.TargetUpdatedCallback();
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
            if (target == null || accessor == null)
                return false;
            if (value == null)
            {
                value = GetDefaultValue(memberType);
            }

            object oldValue;
            oldValue = accessor.GetValue(target);
            if (!object.Equals(oldValue, value))
            {
                if (accessor.CanSetValue(target))
                {
                    accessor.SetValue(target, value);
                    return true;
                }
            }
            return false;
        }

        private bool TryGetMemberValue(out object value)
        {
            if (accessor == null || target == null)
            {
                value = null;
                return false;
            }
            if (accessor.CanGetValue(target))
            {
                value = accessor.GetValue(target);
                return true;
            }
            value = null;
            return false;
        }

        public Type GetValueType()
        {
            return last.memberType;
        }

        public bool TryGetTargetValue(out object targetValue)
        {
            if (next != null)
            {
                if ((flags & Flags.ListenerMember) == 0)
                {
                    object tmp;
                    if (TryGetMemberValue(out tmp))
                    {
                        next.SetTarget(tmp);
                    }
                    else
                    {
                        targetValue = null;
                        return false;
                    }
                }
                return next.TryGetTargetValue(out targetValue);
            }
            return TryGetMemberValue(out targetValue);
        }


        public bool TrySetTargetValue(object targetValue)
        {
            if (next != null)
            {
                if ((flags & Flags.ListenerMember) == 0)
                {
                    object tmp;
                    if (TryGetMemberValue(out tmp))
                    {
                        next.SetTarget(tmp);
                    }
                    else
                    {
                        return false;
                    }
                }

                return next.TrySetTargetValue(targetValue);
            }

            return TrySetMemberValue(targetValue);
        }


        public static PropertyBinder Create(string path)
        {
            PropertyBinder first = null;
            if (string.IsNullOrEmpty(path) || path == ".")
            {
                first = new PropertyBinder(null);
                first.first = first;
                first.last = first;
                return first;
            }

            string[] propertys = path.Split('.');

            PropertyBinder last = null;
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


                    PropertyBinder binder = new PropertyBinder(propName, isIndexer);
                    if (first == null)
                    {
                        first = binder;
                    }
                    else
                    {
                        last.next = binder;
                        binder.prev = last;

                    }
                    last = binder;
                }

            }
            PropertyBinder current = first;
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

            ReleaseListenerChanged();
        }

        public override string ToString()
        {
            return Path;
        }



        ~PropertyBinder()
        {
            Dispose();
        }

        public static object GetDefaultValue(Type type)
        {
            if (type == null) throw new NullReferenceException();
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }

    }
}