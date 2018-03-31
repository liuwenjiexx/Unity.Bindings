using LWJ.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LWJ.UnityEditor
{

    [CustomEditor(typeof(Unity.Binding))]
    public class BindingEditor : Editor
    {

        SerializedProperty bindingsProperty;
        SerializedProperty startedBindingProperty;

        static GUIContent addButtonContent;


        void OnEnable()
        {


            bindingsProperty = serializedObject.FindProperty("bindings");
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

            DrawEntryArray(bindingsProperty);

            serializedObject.ApplyModifiedProperties();
        }
        static void DrawArrayItem(EditorArrayState ctrlState, SerializedProperty arrayProperty, Unity.Binding.BindingType bindingType)
        {
            //using (new GUILayout.HorizontalScope())
            //{
            //    GUILayout.Space(EditorGUI.indentLevel * 14);

            //using (new EditorGUIScopes.IndentLevel())
            //using (new EditorGUILayout.VerticalScope())
            //{
            Event evt = Event.current;


            //using (new EditorGUILayout.VerticalScope("box"))
            //{

            //using (new GUILayout.HorizontalScope())
            //{
            //    EditorGUILayout.PrefixLabel(label);
            //}


            using (new GUILayout.VerticalScope())
            {

                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    SerializedProperty itemProperty = arrayProperty.GetArrayElementAtIndex(i);
                    if (itemProperty.FindPropertyRelative("bindingType").intValue != (int)bindingType)
                        continue;

                    if (ctrlState.SelectedIndex == i)
                    {
                        GUI.backgroundColor = Color.blue;
                    }

                    using (new GUILayout.VerticalScope("box"))
                    {
                        GUI.backgroundColor = Color.white;

                        EditorGUILayout.PropertyField(itemProperty, GUIContent.none, false);
                    }
                    if (evt.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(evt.mousePosition))
                    {
                        ctrlState.SelectedIndex = i;
                        evt.Use();
                    }

                }
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope("Toolbar"))
                {
                    if (GUILayout.Button("+", EditorStyles.toolbarButton))
                    {
                        arrayProperty.arraySize++;
                        arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1)
                            .FindPropertyRelative("bindingType").intValue = (int)bindingType;
                        ctrlState.SelectedIndex = arrayProperty.arraySize - 1;

                    }
                    //GUI.enabled = ctrlState.SelectedIndex > 0;
                    if (GUILayout.Button(EditorGUIHelper.IconToolbarMinus, EditorStyles.toolbarButton))
                    {
                        int removeIndex = ctrlState.SelectedIndex;
                        if (removeIndex >= 0)
                        {
                            arrayProperty.DeleteArrayElementAtIndex(removeIndex);
                        }
                        if (arrayProperty.arraySize >= ctrlState.SelectedIndex)
                            ctrlState.SelectedIndex--;
                    }
                    GUI.enabled = true;
                }
            }

            //}


            //}
            //}
        }
        public static void DrawEntryArray(SerializedProperty bindingsProperty)
        {
            int removeIndex = -1;
            //Vector2 removeButtonSize = GUIStyle.none.CalcSize(EditorGUIHelper.IconToolbarMinus);

            //for (int i = 0; i < itemsProperty.arraySize; ++i)
            //{
            //    SerializedProperty itemProperty = itemsProperty.GetArrayElementAtIndex(i);

            //    EditorGUILayout.PropertyField(itemProperty);

            //    Rect callbackRect = GUILayoutUtility.GetLastRect();

            //    Rect removeButtonPos = new Rect(callbackRect.xMax - removeButtonSize.x - 8, callbackRect.y + 1, removeButtonSize.x, removeButtonSize.y);
            //    if (GUI.Button(removeButtonPos, EditorGUIHelper.IconToolbarMinus, GUIStyle.none))
            //    {
            //        removeIndex = i;
            //    }

            //}

            EditorArrayState ctrlState;
            int ctrlId = GUIUtility.GetControlID(FocusType.Passive);
            ctrlState = (EditorArrayState)GUIUtility.GetStateObject(typeof(EditorArrayState), ctrlId);

            List<int> list = new List<int>();
            for (int i = 0; i < bindingsProperty.arraySize; i++)
            {
                var itemProperty = bindingsProperty.GetArrayElementAtIndex(i);
                int val = itemProperty.FindPropertyRelative("bindingType").intValue;
                if (!list.Contains(val))
                    list.Add(val);
            }


            for (int i = 0; i < list.Count; i++)
            {
                Unity.Binding.BindingType bindingType = (Unity.Binding.BindingType)list[i];
                if (i > 0)
                    EditorGUILayout.Space();
                using (new GUILayout.HorizontalScope())
                {
                    //GUILayout.Space(EditorGUI.indentLevel * 14);
                    EditorGUILayout.Space();

                    using (new EditorGUIScopes.IndentLevel())
                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.VerticalScope("box"))
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField(new GUIContent(bindingType.ToString()));
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button(EditorGUIHelper.IconToolbarMinus, GUIStyle.none))
                                {
                                    removeIndex = i;
                                }
                            }

                            DrawArrayItem(ctrlState, bindingsProperty, bindingType);

                        }

                    }
                }
            }


            if (removeIndex > -1)
            {
                for (int i = 0; i < bindingsProperty.arraySize; i++)
                {
                    SerializedProperty bindingProperty = bindingsProperty.GetArrayElementAtIndex(i);
                    if (bindingProperty.FindPropertyRelative("bindingType").intValue == list[removeIndex])
                    {
                        bindingsProperty.DeleteArrayElementAtIndex(i);
                        i--;
                    }
                }
            }


            const float addButonWidth = 200f;
            EditorGUILayout.Space();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(addButtonContent, GUILayout.MinWidth(addButonWidth)))
                {
                    ShowAddBindingMenu(bindingsProperty);
                }
                GUILayout.FlexibleSpace();
            }

        }



        private class EditorArrayState
        {
            public int SelectedIndex = -1;
        }




        static void ShowAddBindingMenu(SerializedProperty itemsProperty)
        {
            GenericMenu menu = new GenericMenu();
            Type enumType = typeof(Unity.Binding.BindingType);
            string[] names = Enum.GetNames(enumType);
            int[] values = (int[])Enum.GetValues(enumType);
            for (int i = 0; i < names.Length; ++i)
            {
                bool active = true;

                for (int p = 0; p < itemsProperty.arraySize; ++p)
                {
                    SerializedProperty bindingEntry = itemsProperty.GetArrayElementAtIndex(p);
                    SerializedProperty typeProperty = bindingEntry.FindPropertyRelative("bindingType");
                    if (typeProperty.intValue == values[i])
                    {
                        active = false;
                        break;
                    }
                }

                //if (itemsProperty.type == typeof(Unity.Binding.ChildEntry).Name)
                //{
                //    if (values[i] != (int)Unity.Binding.BindingType.Binding)
                //        active = false;
                //}
                GUIContent content = new GUIContent(names[i]);
                if (active)
                {
                    menu.AddItem(content, false, (data) =>
                    {
                        object[] array = (object[])data;
                        //var itemsProperty = (SerializedProperty)array[0];
                        int selected = (int)array[1];

                        itemsProperty.arraySize++;

                        SerializedProperty bindingEntry = itemsProperty.GetArrayElementAtIndex(itemsProperty.arraySize - 1);

                        SerializedProperty typeProperty = bindingEntry.FindPropertyRelative("bindingType");

                        typeProperty.enumValueIndex = selected;

                        itemsProperty.serializedObject.ApplyModifiedProperties();
                    }, new object[] { itemsProperty, i });
                }
                else
                {
                    menu.AddDisabledItem(content);
                }
            }
            menu.ShowAsContext();
            Event.current.Use();
        }


        public static void DrawArrayStyle2(GUIContent label, SerializedProperty arrayProperty, Func<SerializedProperty, GUIContent> getItemLabel)
        {
            using (new EditorGUILayoutScopes.IndentLevel(0))
            using (new GUILayout.HorizontalScope())
            {

                Event evt = Event.current;

                EditorArrayState ctrlState;
                int ctrlId = GUIUtility.GetControlID(FocusType.Passive);
                ctrlState = (EditorArrayState)GUIUtility.GetStateObject(typeof(EditorArrayState), ctrlId);



                using (new EditorGUILayout.VerticalScope("box"))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label(label ?? GUIContent.none);
                        GUILayout.FlexibleSpace();
                    }
                    int removeIndex = -1;
                    using (new GUILayout.VerticalScope())
                    {

                        for (int i = 0; i < arrayProperty.arraySize; i++)
                        {

                            if (ctrlState.SelectedIndex == i)
                            {
                                GUI.backgroundColor = Color.blue;
                            }
                            using (new GUILayout.HorizontalScope())
                            {
                                //GUILayout.Space(EditorGUI.indentLevel * 14);                                
                                // EditorGUILayout.Space();
                                //using (new EditorGUIScopes.IndentLevel())
                                using (new GUILayout.VerticalScope("box"))
                                {
                                    GUI.backgroundColor = Color.white;
                                    SerializedProperty itemProperty = arrayProperty.GetArrayElementAtIndex(i);
                                    using (new GUILayout.HorizontalScope())
                                    {
                                        GUIContent itemLabel = GUIContent.none;
                                        if (getItemLabel != null)
                                            itemLabel = getItemLabel(itemProperty);
                                        EditorGUILayout.LabelField(itemLabel);
                                        GUILayout.FlexibleSpace();
                                        if (GUILayout.Button(EditorGUIHelper.IconToolbarMinus, GUIStyle.none))
                                        {
                                            removeIndex = i;
                                        }
                                    }
                                    EditorGUILayout.PropertyField(itemProperty, GUIContent.none, true);
                                }
                                if (GUILayoutUtility.GetLastRect().Contains(evt.mousePosition))
                                {
                                    if (evt.type == EventType.MouseDown)
                                    {
                                        ctrlState.SelectedIndex = i;
                                        evt.Use();
                                    }
                                }
                                else
                                {
                                    if (evt.type == EventType.MouseDown)
                                    {
                                        ctrlState.SelectedIndex = -1;
                                    }
                                }
                            }
                        }
                    }


                    if (removeIndex >= 0)
                    {
                        arrayProperty.DeleteArrayElementAtIndex(removeIndex);
                    }
                }

            }
        }

        public static void DrawMultiBindingChildren(SerializedProperty itemsProperty)
        {
            /*  int toBeRemovedEntry = -1;
              Vector2 removeButtonSize = GUIStyle.none.CalcSize(EditorGUIHelper.IconToolbarMinus);

              for (int i = 0; i < itemsProperty.arraySize; ++i)
              {
                  SerializedProperty itemProperty = itemsProperty.GetArrayElementAtIndex(i);

                  using (new GUILayout.HorizontalScope())
                  {
                      EditorGUILayout.LabelField(((BindingBehaviour.BindingType)itemProperty.FindPropertyRelative("bindingType").intValue).ToStringOrEmpty());
                      GUILayout.FlexibleSpace();
                      if (GUILayout.Button(EditorGUIHelper.IconToolbarMinus, GUIStyle.none))
                      {
                          toBeRemovedEntry = i;
                      }
                  }
                  using (new EditorGUILayoutScopes.IndentLevel())
                  {
                      EditorGUILayout.PropertyField(itemProperty, GUIContent.none);
                  }
                  //Rect callbackRect = GUILayoutUtility.GetLastRect();

                  //Rect removeButtonPos = new Rect(callbackRect.xMax - removeButtonSize.x - 8, callbackRect.y + 1, removeButtonSize.x, removeButtonSize.y);
                  //if (GUI.Button(removeButtonPos, EditorGUIHelper.IconToolbarMinus, GUIStyle.none))
                  //{
                  //    toBeRemovedEntry = i;
                  //}

              }

              if (toBeRemovedEntry > -1)
              {
                  RemoveEntry(itemsProperty, toBeRemovedEntry);
              }*/

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();
                DrawArrayStyle2(new GUIContent("Bindings"),
                    itemsProperty,
                    (itemProperty) => new GUIContent(((Unity.Binding.BindingType)itemProperty.FindPropertyRelative("bindingType").intValue).ToStringOrEmpty())
                    );
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();

            const float addButonWidth = 200f;
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("Add"), GUILayout.MinWidth(addButonWidth)))
                {
                    GenericMenu menu = new GenericMenu();
                    Type enumType = typeof(Unity.Binding.BindingType);
                    string[] names = new string[] { Unity.Binding.BindingType.Binding.ToString() };
                    int[] values = new int[] { (int)Unity.Binding.BindingType.Binding };

                    GUIContent content = new GUIContent(names[0]);

                    menu.AddItem(content, false, (data) =>
                    {
                        object[] array = (object[])data;
                        //var itemsProperty = (SerializedProperty)array[0];
                        int value = (int)array[1];

                        itemsProperty.arraySize += 1;

                        SerializedProperty bindingEntry = itemsProperty.GetArrayElementAtIndex(itemsProperty.arraySize - 1);

                        SerializedProperty typeProperty = bindingEntry.FindPropertyRelative("bindingType");

                        typeProperty.intValue = value;

                        itemsProperty.serializedObject.ApplyModifiedProperties();
                    }, new object[] { itemsProperty, values[0] });


                    menu.ShowAsContext();
                    Event.current.Use();
                }
                GUILayout.FlexibleSpace();
            }
        }


        public static void ValueConverterField(SerializedProperty property, params GUILayoutOption[] options)
        {
            ValueConverterField(true, property.displayName, property, options);
        }

        private static void ValueConverterField(bool hasLabel, string label, SerializedProperty property, params GUILayoutOption[] options)
        {

            using (new GUILayout.HorizontalScope())
            {
                if (hasLabel)
                    EditorGUILayout.PrefixLabel(new GUIContent(label));
                property.stringValue = GUILayout.TextField(property.stringValue, GUILayout.ExpandWidth(true));

                string converterName = property.stringValue;

                GUIContent[] contents = new GUIContent[] { EditorGUIHelper.NullContent }.Concat(
                    BindingBase.GetValueConveterNames().Select(o => new GUIContent(o)))
                    .ToArray();
                int oldIndex = contents.IndexOf(o => o.text == converterName);

                int selectedIndex;
                selectedIndex = EditorGUILayout.Popup(oldIndex, contents, GUILayout.MaxWidth(24));
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