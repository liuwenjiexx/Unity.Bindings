using LWJ.Unity;
using UnityEditor;
using UnityEngine;

namespace LWJ.UnityEditor
{

    [CustomPropertyDrawer(typeof(ComponentPopupAttribute))]
    internal class ComponentPopupDrawer : PropertyDrawer
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
