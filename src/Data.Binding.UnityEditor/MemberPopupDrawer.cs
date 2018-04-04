using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using LWJ.Unity;
using UnityEngine;

namespace LWJ.UnityEditor
{
    [CustomPropertyDrawer(typeof(MemberPopupAttribute))]
    public class MemberPopupDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var attr = (MemberPopupAttribute)attribute;
            EditorGUIHelper.MemberPopup(position, label, attr.TargetType, attr.MemberFlags, prop);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            float h = base.GetPropertyHeight(property, label);

            return h;
        }

    }
}
