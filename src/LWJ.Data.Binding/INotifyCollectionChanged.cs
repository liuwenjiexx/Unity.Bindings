/**************************************************************
 *  Filename:    NotifyCollectionChangedEventArgs.cs
 *  Copyright:  © 2017 WenJie Liu. All rights reserved.
 *  Description: LWJ ClassFile
 *  @author:     WenJie Liu
 *  @version     2017/2/27
 **************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace LWJ.Data
{
    public interface INotifyCollectionChanged
    {

        event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;
    }


    public enum NotifyCollectionChangedAction
    {
        Add = 1,
        Move,
        Remove,
        Replace,
        Reset,
    }

    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event EventHandler<NotifyCollectionChangedEventArgs> CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection(IEnumerable<T> collection)
        {
            AddRange(collection);
        }

        public ObservableCollection()
        {

        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        protected override void ClearItems()
        {
            if (Count > 0)
            {
                base.ClearItems();
                if (CollectionChanged != null)
                {
                    var args = NotifyCollectionChangedEventArgs.Remove(new List<T>(Items), 0);
                    CollectionChanged(this, args);
                }
                PropertyChanged.Invoke(this, "Count");
            }
        }
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            if (CollectionChanged != null)
            {
                var args = NotifyCollectionChangedEventArgs.Add(new List<T>(new T[] { item }), index);
                CollectionChanged(this, args);
            }
            PropertyChanged.Invoke(this, "Count");
        }
        protected override void RemoveItem(int index)
        {

            var old = Items[index];
            base.RemoveItem(index);
            if (CollectionChanged != null)
            {
                var args = NotifyCollectionChangedEventArgs.Remove(new List<T>(new T[] { old }), index);
                CollectionChanged(this, args);
            }
            PropertyChanged.Invoke(this, "Count");
        }
        protected override void SetItem(int index, T item)
        {
            var old = Items[index];
            base.SetItem(index, item);
            if (CollectionChanged != null)
            {
                var args = NotifyCollectionChangedEventArgs.Replace(new List<T>(new T[] { item }), new List<T>(new T[] { old }), index);
                CollectionChanged(this, args);
            }
        }
    }

    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        private NotifyCollectionChangedAction action;
        private IList newItems;
        private int newStartingIndex;
        private IList oldItems;
        private int oldStartingIndex;
        private NotifyCollectionChangedEventArgs()
        {

        }
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            this.action = action;
        }


        //public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
        //{
        //    this.action = action;
        //    switch (action)
        //    {
        //        case NotifyCollectionChangedAction.Remove:
        //            this.oldItems = changedItems;
        //            break;
        //        default:
        //            this.newItems = changedItems;
        //            break;
        //    }

        //}
        ///// <summary>
        ///// Add
        ///// </summary>
        ///// <param name="action"></param>
        ///// <param name="newItems"></param>
        ///// <param name="oldItems"></param>
        //public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index)
        //{
        //    this.action = action;
        //    this.newItems = changedItems;
        //    this.newStartingIndex = index;

        //}

        public static NotifyCollectionChangedEventArgs Add(IList changedItems, int index)
        {
            var args = new NotifyCollectionChangedEventArgs()
            {
                action = NotifyCollectionChangedAction.Add,
                newItems = changedItems,
                newStartingIndex = index,
            };
            return args;
        }
        public static NotifyCollectionChangedEventArgs Replace(IList newItems, IList oldItems, int index)
        {
            var args = new NotifyCollectionChangedEventArgs()
            {
                action = NotifyCollectionChangedAction.Replace,
                oldItems = oldItems,
                newItems = newItems,
                newStartingIndex = index,
                oldStartingIndex = index
            };
            return args;
        }
        public static NotifyCollectionChangedEventArgs Move(IList changedItems, int newStartingIndex, int oldStartingIndex)
        {
            var args = new NotifyCollectionChangedEventArgs()
            {
                action = NotifyCollectionChangedAction.Move,
                oldItems = changedItems,
                newItems = changedItems,
                newStartingIndex = newStartingIndex,
                oldStartingIndex = oldStartingIndex,
            };
            return args;
        }
        public static NotifyCollectionChangedEventArgs Remove(IList changedItems, int index)
        {
            var args = new NotifyCollectionChangedEventArgs()
            {
                action = NotifyCollectionChangedAction.Remove,
                oldItems = changedItems,
                oldStartingIndex = index,
            };
            return args;
        }
        ///// <summary>
        ///// Replace
        ///// </summary>
        ///// <param name="action"></param>
        ///// <param name="newItems"></param>
        ///// <param name="oldItems"></param>
        //public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        //{
        //    this.action = action;
        //    this.oldItems = oldItems;
        //    this.newItems = newItems;
        //    this.newStartingIndex = -1;
        //    this.oldStartingIndex = -1;
        //}

        /// <summary>
        /// Move
        /// </summary>
        /// <param name="action"></param>
        /// <param name="newItems"></param>
        /// <param name="oldItems"></param>
        //public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int newStartingIndex, int oldStartingIndex)
        //{
        //    this.action = action;
        //    this.newStartingIndex = newStartingIndex;
        //    this.oldStartingIndex = oldStartingIndex;

        //    switch (action)
        //    {
        //        case NotifyCollectionChangedAction.Move:
        //        case NotifyCollectionChangedAction.Remove:
        //            this.oldItems = changedItems;
        //            break;
        //        default:
        //            this.newItems = changedItems;
        //            break;
        //    }
        //}

        public NotifyCollectionChangedAction Action { get { return action; } }
        public IList NewItems { get { return newItems; } }
        public int NewStartingIndex { get { return newStartingIndex; } }
        public IList OldItems { get { return oldItems; } }
        public int OldStartingIndex { get { return oldStartingIndex; } }
    }
}
