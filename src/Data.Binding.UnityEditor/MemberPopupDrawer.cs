using LWJ.Unity;
using System;
using UnityEditor;
using UnityEngine;

namespace LWJ.UnityEditor
{
    [CustomPropertyDrawer(typeof(MemberPopupAttribute))]
    internal class MemberPopupDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var attr = (MemberPopupAttribute)attribute;
            Type type = null;
            if (attr.TargetType != null)
            {
                type = attr.TargetType;

            }
            else if (!string.IsNullOrEmpty(attr.TargetMember))
            {

                var spMember = prop.serializedObject.FindProperty(prop.propertyPath.Substring(0, prop.propertyPath.LastIndexOf('.')) + "." + attr.TargetMember);
                if (spMember != null)
                { 
                    if (spMember.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (spMember.objectReferenceValue)
                            type = spMember.objectReferenceValue.GetType();
                    }
                }
                else
                {
                    Debug.LogError("Not Member: " + attr.TargetMember + ", type: " + attr.TargetType);
                }

            }
            EditorGUIHelper.MemberPopup(position, label, type, attr.MemberFlags, prop);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h = base.GetPropertyHeight(property, label);
            return h;
        }

    }
}
