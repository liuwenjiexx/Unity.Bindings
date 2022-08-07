using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Yanmonet.Bindings
{
    public class BindingPropertyChangedEventArgs : EventArgs
    {

        private BindingBase binding;
        private string propertyPath;

        private BindingPropertyChangedEventArgs()
        {

        }


        public BindingBase Binding { get => binding; }


        public string PropertyPath { get => propertyPath; }

        static Queue<BindingPropertyChangedEventArgs> pool = new Queue<BindingPropertyChangedEventArgs>();

        public static BindingPropertyChangedEventArgs Get(BindingBase binding, string propertyPath)
        {
            BindingPropertyChangedEventArgs e;
            if (pool.Count > 0)
            {
                e = pool.Dequeue();
            }
            else
            {
                e = new BindingPropertyChangedEventArgs();
            }
            e.binding = binding;
            e.propertyPath = propertyPath;
            return e;
        }

        public static void Release(BindingPropertyChangedEventArgs e)
        {
            pool.Enqueue(e);
        }
    }
}