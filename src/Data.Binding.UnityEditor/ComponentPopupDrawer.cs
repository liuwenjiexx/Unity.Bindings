using LWJ.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LWJ.UnityEditor
{

    [CustomPropertyDrawer(typeof(ComponentPopupAttribute))]
    public class ComponentPopupDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUIHelper.ComponentPopup(position, label, prop);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            float h = base.GetPropertyHeight(property, label);

            return h;
        }

    }

}
