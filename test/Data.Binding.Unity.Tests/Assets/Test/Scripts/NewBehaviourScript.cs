using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using LWJ.Data;
using System.Reflection;
using System;
using Object = UnityEngine.Object;
namespace LWJ.Unity
{

    [DefaultMember("Value")]
    public class NewBehaviourScript : MonoBehaviour, System.ComponentModel.INotifyPropertyChanged
    {
        public UnityEngine.Component ccc;
        [ComponentPopup]
        public UnityEngine.Object target;
        [MemberPopup(typeof(UnityEngine.Component), MemberPopupFlags.Field| MemberPopupFlags.Property)]
        public string sss;
        public string eventMember;
        public string valueMember;
        public event PropertyChangedEventHandler PropertyChanged;
        MethodInfo eventAddListenerMethod;
        FieldInfo fInfo;
        PropertyInfo pInfo;

        // Use this for initialization
        void Start()
        {
            Init();
            GameObject g;
        }

        private void StringValue(string value)
        {
            PropertyChanged.Invoke(this, "Value");
        }

        private void FloatValue(float value)
        {
            PropertyChanged.Invoke(this, "Value");
        }
        private void IntValue(int value)
        {
            PropertyChanged.Invoke(this, "Value");
        }


        public object Value
        {
            get
            {
                if (target != null)
                {
                    if (pInfo != null)
                    {
                        pInfo.GetGetMethod().Invoke(target, null);
                    }
                    else if (fInfo != null)
                    {
                        fInfo.GetValue(target);
                    }
                }
                return null;
            }
            set
            {
                if (target != null)
                {
                    if (pInfo != null)
                    {
                        pInfo.GetSetMethod().Invoke(target, new object[] { value });
                    }
                    else if (fInfo != null)
                    {
                        fInfo.SetValue(target, value);
                    }
                }

            }
        }

        private void Init()
        {
            if (target)
            {
                Type targetType = target.GetType();
                object eventPropertyTarget;
                MethodInfo addListenerMethod;
                GetAddListenerMethod(targetType, eventMember, out eventPropertyTarget, out addListenerMethod);

                if (addListenerMethod != null)
                {
                    object value = null;
                    var parameters = addListenerMethod.GetParameters();
                    Type valueType = parameters[0].ParameterType;
                    if (valueType == typeof(string))
                    {
                        value = Delegate.CreateDelegate(valueType, this, "StringValue");
                    }
                    else if (valueType == typeof(int))
                    {
                        value = Delegate.CreateDelegate(valueType, this, "IntValue");
                    }
                    else if (valueType == typeof(float))
                    {
                        value = Delegate.CreateDelegate(valueType, this, "FloatValue");
                    }

                    addListenerMethod.Invoke(eventPropertyTarget, new object[] { value });

                    fInfo = targetType.GetField(valueMember);
                    if (fInfo == null)
                    {
                        pInfo = targetType.GetProperty(valueMember);
                    }
                    if (fInfo == null && pInfo == null)
                    {
                        Debug.LogErrorFormat("Type {0} Not Value Member: {1}", targetType, valueMember);
                    }

                }
                else
                {
                    Debug.LogErrorFormat("Type {0} Not EventMember: {1}", targetType, eventMember);
                }
            }
        }



        public void GetAddListenerMethod(Type type, string eventName, out object eventObj, out MethodInfo addListenerMethod)
        {
            eventObj = null;
            addListenerMethod = null;

            if (addListenerMethod == null)
            {


                if (target != null && !string.IsNullOrEmpty(eventName))
                {

                    var eventProperty = type.GetProperty(eventName);
                    Type eventPropertyType;
                    if (eventProperty != null)
                    {
                        eventObj = eventProperty.GetValue(target, null);
                        eventPropertyType = eventProperty.PropertyType;
                    }
                    else
                    {
                        var eventField = type.GetField(eventName);
                        if (eventField != null)
                        {
                            eventObj = eventField.GetValue(target);
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


        }

    }
}