using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;
using LWJ;
using Object = UnityEngine.Object;
using LWJ.Unity;
using LWJ.Data;

namespace LWJ.UnityEditor
{

    [CustomEditor(typeof(BindingBehaviour))]
    public class BindingEditor : Editor
    {

        SerializedProperty itemsProperty;
        SerializedProperty startedBindingProperty;

        static GUIContent addButtonContent;


        void OnEnable()
        {


            itemsProperty = serializedObject.FindProperty("items");
            startedBindingProperty = serializedObject.FindProperty("startedBinding");
            addButtonContent = new GUIContent("Add Binding");

            //Debug.Log(new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus")) == null);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            //EditorGUILayout.Space();

            //  Debug.Log(itemsProperty.type + "," + itemsProperty.propertyType);

            EditorGUILayout.PropertyField(startedBindingProperty);
            DrawEntryArray(itemsProperty);

            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawEntryArray(SerializedProperty itemsProperty)
        {
            int toBeRemovedEntry = -1;
            Vector2 removeButtonSize = GUIStyle.none.CalcSize(EditorGUIHelper.IconToolbarMinus);

            for (int i = 0; i < itemsProperty.arraySize; ++i)
            {
                SerializedProperty itemProperty = itemsProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.PropertyField(itemProperty);

                Rect callbackRect = GUILayoutUtility.GetLastRect();

                Rect removeButtonPos = new Rect(callbackRect.xMax - removeButtonSize.x - 8, callbackRect.y + 1, removeButtonSize.x, removeButtonSize.y);
                if (GUI.Button(removeButtonPos, EditorGUIHelper.IconToolbarMinus, GUIStyle.none))
                {
                    toBeRemovedEntry = i;
                }

            }

            if (toBeRemovedEntry > -1)
            {
                RemoveEntry(itemsProperty, toBeRemovedEntry);
            }



            Rect pos = GUILayoutUtility.GetRect(addButtonContent, GUI.skin.button);
            const float addButonWidth = 200f;
            pos.x = pos.x + (pos.width - addButonWidth) / 2;
            pos.width = addButonWidth;
            if (GUI.Button(pos, addButtonContent))
            {
                ShowAddBindingMenu(itemsProperty);
            }
        }



        private class EditorArrayState
        {
            public int SelectedIndex = -1;
        }



        private static void RemoveEntry(SerializedProperty itemsProperty, int toBeRemovedEntry)
        {
            itemsProperty.DeleteArrayElementAtIndex(toBeRemovedEntry);
        }

        static void ShowAddBindingMenu(SerializedProperty itemsProperty)
        {
            GenericMenu menu = new GenericMenu();
            Type enumType = typeof(BindingBehaviour.BindingType);
            string[] names = Enum.GetNames(enumType);
            int[] values = (int[])Enum.GetValues(enumType);
            for (int i = 0; i < names.Length; ++i)
            {
                bool active = true;

                for (int p = 0; p < itemsProperty.arraySize; ++p)
                {
                    SerializedProperty bindingEntry = itemsProperty.GetArrayElementAtIndex(p);
                    SerializedProperty typeProperty = bindingEntry.FindPropertyRelative("type");
                    if (typeProperty.intValue == values[i])
                    {
                        active = false;
                        break;
                    }
                }

                if (itemsProperty.type == typeof(BindingBehaviour.ChildEntry).Name)
                {
                    if (values[i] != (int)BindingBehaviour.BindingType.Binding)
                        active = false;
                }
                GUIContent content = new GUIContent(names[i]);
                if (active)
                    menu.AddItem(content, false, OnAddNewSelected, new object[] { itemsProperty, i });
                //else
                    //menu.AddDisabledItem(content);
            }
            menu.ShowAsContext();
            Event.current.Use();
        }
        private static void OnAddNewSelected(object data)
        {
            object[] array = (object[])data;
            var itemsProperty = (SerializedProperty)array[0];
            int selected = (int)array[1];

            itemsProperty.arraySize += 1;

            SerializedProperty bindingEntry = itemsProperty.GetArrayElementAtIndex(itemsProperty.arraySize - 1);

            var sp1 = bindingEntry.FindPropertyRelative("bindings");
            sp1.ClearArray();
            sp1.arraySize = 1;

            SerializedProperty typeProperty = bindingEntry.FindPropertyRelative("type");

            typeProperty.enumValueIndex = selected;

            itemsProperty.serializedObject.ApplyModifiedProperties();
        }






        public static void ValueConverterField(SerializedProperty property, params GUILayoutOption[] options)
        {
            ValueConverterField(true, property.displayName, property, options);
        }

        private static void ValueConverterField(bool hasLabel, string label, SerializedProperty property, params GUILayoutOption[] options)
        {

            using (new GUILayout.HorizontalScope())
            {
                string converterName = property.stringValue;

                GUIContent[] contents = new GUIContent[] { EditorGUIHelper.NullContent }.Concat(
                    BindingBase.GetValueConveterNames().Select(o => new GUIContent(o)))
                    .ToArray();
                int oldIndex = contents.IndexOf(o => o.text == converterName);

                int selectedIndex;
                if (hasLabel)
                    selectedIndex = EditorGUILayout.Popup(new GUIContent(label), oldIndex, contents, options);
                else
                    selectedIndex = EditorGUILayout.Popup(GUIContent.none, oldIndex, contents, options);
                if (selectedIndex != oldIndex)
                {
                    if (selectedIndex == 0)
                        converterName = null;
                    else
                        converterName = contents[selectedIndex].text;
                    property.stringValue = converterName;
                }

            }
        }

        public static void MultiValueConverterField(SerializedProperty property, params GUILayoutOption[] options)
        {
            MultiValueConverterField(true, property.displayName, property, options);
        }

        private static void MultiValueConverterField(bool hasLabel, string label, SerializedProperty property, params GUILayoutOption[] options)
        {

            using (new GUILayout.HorizontalScope())
            {
                string converterName = property.stringValue;

                GUIContent[] contents = new GUIContent[] { EditorGUIHelper.NullContent }.Concat(
                    BindingBase.GetMultiValueConveterNames().Select(o => new GUIContent(o)))
                    .ToArray();
                int oldIndex = contents.IndexOf(o => o.text == converterName);

                int selectedIndex;
                if (hasLabel)
                    selectedIndex = EditorGUILayout.Popup(new GUIContent(label), oldIndex, contents, options);
                else
                    selectedIndex = EditorGUILayout.Popup(GUIContent.none, oldIndex, contents, options);
                if (selectedIndex != oldIndex)
                {
                    if (selectedIndex == 0)
                        converterName = null;
                    else
                        converterName = contents[selectedIndex].text;
                    property.stringValue = converterName;
                }

            }
        }



    }


}