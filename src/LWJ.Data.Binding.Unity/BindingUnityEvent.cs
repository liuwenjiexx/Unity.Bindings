using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Events;
using System;
using Object = UnityEngine.Object;
using LWJ;


namespace LWJ.Unity
{

    [DefaultMember("Value")]
    public class BindingUnityEvent : MonoBehaviour
    {

        public Component target;
        public string eventName;
        private object eventPropertyTarget;
        private MethodInfo addListenerMethod;

        public MethodInfo AddListenerMethod
        {
            get
            {
                if (addListenerMethod == null)
                {
                    if (target != null && !string.IsNullOrEmpty(eventName))
                    {
                        var eventProperty = target.GetType().GetProperty(eventName);
                        Type eventPropertyType;
                        if (eventProperty != null)
                        {
                            eventPropertyTarget = eventProperty.GetValueUnity(target);
                            eventPropertyType = eventProperty.PropertyType;
                        }
                        else
                        {
                            var eventField = target.GetType().GetField(eventName);
                            if (eventField != null)
                            {
                                eventPropertyTarget = eventField.GetValue(target);
                                eventPropertyType = eventField.FieldType;
                            }
                            else
                                throw new Exception("not found eventName:" + eventName);

                        }

                        addListenerMethod = eventPropertyType.GetMethod("AddListener");
                        if (addListenerMethod == null)
                            throw new Exception("not found addListener method");

                    }
                }
                return addListenerMethod;
            }

        }

        object value;
        public object Value
        {
            set
            {
                if (value != this.value)
                {
                    this.value = value;

                    var addMethod = AddListenerMethod;
                    if (target != null && addMethod != null)
                    {
                        var pInfo = addMethod.GetParameters()[0];
                        if (pInfo.ParameterType == typeof(UnityAction))
                        {
                            Delegate del = value as Delegate;
                            value = Delegate.CreateDelegate(typeof(UnityAction), del.Target, del.Method);
                            //value = Activator.CreateInstance(typeof(UnityAction), new object[] { value });
                        }
                        addMethod.Invoke(eventPropertyTarget, new object[] { value });
                    }
                }
            }
        }


    }

}