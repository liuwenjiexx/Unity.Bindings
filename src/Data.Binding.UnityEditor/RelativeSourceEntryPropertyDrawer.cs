using LWJ.Data;
using LWJ.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LWJ.UnityEditor
{


    [CustomPropertyDrawer(typeof(ArrayPropertyAttribute),false)]
    public class ArrayPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (label == GUIContent.none)
                return 0;
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //EditorGUI.BeginProperty(position, label, property);

            //if (label != GUIContent.none)
            //    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            SerializedProperty s = property.FindPropertyRelative("type");
            using (new EditorGUILayoutScopes.IndentLevel())
            {
                GUILayout.Label("Array "+property.propertyPath+","+(s==null));
            }

            //EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(BindingBehaviour.RelativeSourceEntry), true)]
    public class BindingBehaviourRelativeSourceEntryPropertyDrawer : PropertyDrawer
    {

        SerializedProperty typeNameProperty;
        SerializedProperty modeProperty;
        SerializedProperty ancestorLevelProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (label == GUIContent.none)
                return 0;
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //if (typeNameProperty == null)
            //{
            typeNameProperty = property.FindPropertyRelative("typeName");
            modeProperty = property.FindPropertyRelative("mode");
            ancestorLevelProperty = property.FindPropertyRelative("ancestorLevel");
            //}

            EditorGUI.BeginProperty(position, label, property);

            if (label != GUIContent.none)
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

   
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.PropertyField(modeProperty);
                EditorGUILayout.PropertyField(typeNameProperty);
                EditorGUILayout.PropertyField(ancestorLevelProperty);
            }

            EditorGUI.EndProperty();
        }
    }

}
