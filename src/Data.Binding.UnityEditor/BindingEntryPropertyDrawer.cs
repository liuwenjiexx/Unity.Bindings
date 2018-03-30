using UnityEngine;
using System.Collections;
using UnityEditor;
using LWJ.Data;
using LWJ;
using System;
using LWJ.Unity;
using System.Linq;

namespace LWJ.UnityEditor
{


    [CustomPropertyDrawer(typeof(BindingBehaviour.ChildEntry), true)]
    [CustomPropertyDrawer(typeof(BindingBehaviour.Entry), true)]
    public class BindingEntryPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //if (label == GUIContent.none)
            //    return 0;
            //return base.GetPropertyHeight(property, label);
            return 0;
        }
        SerializedProperty typeProperty;
        SerializedProperty bindingsProperty;
        private class EditorArrayState
        {
            public int SelectedIndex = -1;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            typeProperty = property.FindPropertyRelative("type");
            SerializedProperty bindingsProperty = property.FindPropertyRelative("bindings");
            position = EditorGUI.IndentedRect(position);
            EditorGUI.BeginProperty(position, label, property);

            //if (label != GUIContent.none)
            //    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            label = new GUIContent(typeProperty.enumDisplayNames[typeProperty.enumValueIndex]);
            for (int i = 0; i < bindingsProperty.arraySize; i++)
            {
                SerializedProperty itemProperty = bindingsProperty.GetArrayElementAtIndex(i);
                var sp1 = itemProperty.FindPropertyRelative("bindingType");
                sp1.intValue = typeProperty.intValue;
            }

            DrawArray(label, bindingsProperty);


            EditorGUI.EndProperty();
        }

        public static void DrawArray(GUIContent label, SerializedProperty arrayProperty)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUI.indentLevel * 14);

                using (new EditorGUIScopes.IndentLevel())
                using (new EditorGUILayout.VerticalScope())
                {
                    Event evt = Event.current;

                    EditorArrayState ctrlState;
                    int ctrlId = GUIUtility.GetControlID(FocusType.Passive);
                    ctrlState = (EditorArrayState)GUIUtility.GetStateObject(typeof(EditorArrayState), ctrlId);

                    using (new EditorGUILayout.VerticalScope("box"))
                    {
                        if (label != GUIContent.none)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                EditorGUILayout.PrefixLabel(label);
                            }

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

                                using (new GUILayout.VerticalScope("box"))
                                {
                                    GUI.backgroundColor = Color.white;
                                    SerializedProperty itemProperty = arrayProperty.GetArrayElementAtIndex(i);

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
                                    ctrlState.SelectedIndex = arrayProperty.arraySize - 1;
                                }
                                //GUI.enabled = ctrlState.SelectedIndex > 0;
                                if (GUILayout.Button(EditorGUIHelper.IconToolbarMinus, EditorStyles.toolbarButton))
                                {
                                    removeIndex = ctrlState.SelectedIndex;
                                    if (arrayProperty.arraySize >= ctrlState.SelectedIndex)
                                        ctrlState.SelectedIndex--;
                                }
                                GUI.enabled = true;
                            }
                        }
                        if (removeIndex >= 0)
                        {
                            arrayProperty.DeleteArrayElementAtIndex(removeIndex);
                        }
                    }


                }
            }
        }

    }






    [CustomPropertyDrawer(typeof(BindingBehaviour.ChildBindingEntry), true)]
    public class BindingBehaviourChildBindingEntryPropertyDrawer : PropertyDrawer
    {
        // SerializedProperty sourceTypeProperty;
        SerializedProperty sourceProperty;
        // SerializedProperty relativeProperty;
        //    SerializedProperty nameSourceProperty;
        SerializedProperty pathProperty;

        SerializedProperty targetNullValueProperty;
        SerializedProperty fallbackValueProperty;
        SerializedProperty stringFormatProperty;
        SerializedProperty modeProperty;
        SerializedProperty converterProperty;
        SerializedProperty converterParameterProperty;
        SerializedProperty notifyOnSourceUpdatedProperty;
        SerializedProperty notifyOnTargetUpdatedProperty;

        SerializedProperty bindingTypeProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (label == GUIContent.none)
                return 0;
            return base.GetPropertyHeight(property, label);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // sourceTypeProperty = property.FindPropertyRelative("sourceType");
            sourceProperty = property.FindPropertyRelative("source");
            //   relativeProperty = property.FindPropertyRelative("relativeSource");
            pathProperty = property.FindPropertyRelative("path");
            targetNullValueProperty = property.FindPropertyRelative("nullValue");
            fallbackValueProperty = property.FindPropertyRelative("fallbackValue");
            stringFormatProperty = property.FindPropertyRelative("stringFormat");
            modeProperty = property.FindPropertyRelative("mode");
            converterProperty = property.FindPropertyRelative("converter");
            converterParameterProperty = property.FindPropertyRelative("converterParameter");
            notifyOnSourceUpdatedProperty = property.FindPropertyRelative("notifyOnSourceUpdated");
            notifyOnTargetUpdatedProperty = property.FindPropertyRelative("notifyOnTargetUpdated");
            bindingTypeProperty = property.FindPropertyRelative("bindingType");


            BindingBehaviour.BindingType bindingType = BindingBehaviour.BindingType.Binding;
            bindingType = (BindingBehaviour.BindingType)bindingTypeProperty.intValue;

            EditorGUI.BeginProperty(position, label, property);

            if (label != GUIContent.none)
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            using (new GUILayout.VerticalScope())
            {

                if (bindingType == BindingBehaviour.BindingType.Binding)
                {
                    //EditorGUILayout.PropertyField(sourceTypeProperty);
                    //            switch ((BindingBehaviour.SourceType)sourceTypeProperty.intValue)
                    //            {
                    //                case BindingBehaviour.SourceType.Relative:
                    //                    EditorGUILayout.PropertyField(relativeProperty, (GUIContent.none));
                    //                    break;
                    //                case BindingBehaviour.SourceType.Name:
                    //                    nameSourceProperty = property.FindPropertyRelative("nameSource");
                    //                    SerializedProperty nameProperty = nameSourceProperty.FindPropertyRelative("name");
                    //                    SerializedProperty componentNameProperty = nameSourceProperty.FindPropertyRelative("componentName");
                    //                    SerializedProperty findModeProperty = nameSourceProperty.FindPropertyRelative("findMode");

                    //                    EditorGUI.BeginProperty(GUILayoutUtility.GetRect(0, Screen.width, 0, Screen.height), new GUIContent(nameSourceProperty.displayName), nameSourceProperty);


                    //                    using (new EditorGUILayoutScopes.IndentLevel())
                    //                    using (new GUILayout.VerticalScope())
                    //                    {
                    //                        EditorGUILayout.PropertyField(findModeProperty);
                    //                        EditorGUILayout.PropertyField(nameProperty);
                    //                        EditorGUILayout.PropertyField(componentNameProperty);
                    //                    }

                    //                    EditorGUI.EndProperty();
                    //                    break;
                    //                case BindingBehaviour.SourceType.Source:
                    //                default:
                    //                    using (new EditorGUILayoutScopes.IndentLevel())
                    //                    {
                    //                        sourceProperty.objectReferenceValue = EditorGUIHelper.ComponentAndGameObjectPop(sourceProperty.displayName, sourceProperty.objectReferenceValue, true);
                    //                    }
                    //                    break;
                    //            }

                    BindingBehaviourBindingEntryPropertyDrawer.DrawSource(property);
                    Type sourceType = null;
                    if (sourceProperty.objectReferenceValue != null)
                        sourceType = sourceProperty.objectReferenceValue.GetType();
                    pathProperty.stringValue = EditorGUIHelper.PropertyNamesField(pathProperty.displayName, sourceType, pathProperty.stringValue, false);
                }


                EditorGUILayout.PropertyField(targetNullValueProperty);

                EditorGUILayout.PropertyField(fallbackValueProperty);

                EditorGUILayout.PropertyField(stringFormatProperty);
                switch (bindingType)
                {
                    case BindingBehaviour.BindingType.Binding:
                        {
                            EditorGUILayout.PropertyField(modeProperty);
                            //EditorGUILayout.PropertyField(converterProperty);
                            BindingEditor.ValueConverterField(converterProperty);
                            EditorGUILayout.PropertyField(converterParameterProperty);
                            //EditorGUILayout.PropertyField(notifyOnSourceUpdatedProperty);
                            //EditorGUILayout.PropertyField(notifyOnTargetUpdatedProperty);
                        }
                        break;
                    case BindingBehaviour.BindingType.MultiBinding:
                        {
                            EditorGUILayout.PropertyField(modeProperty);
                            //EditorGUILayout.PropertyField(converterProperty);
                            BindingEditor.MultiValueConverterField(converterProperty);
                            EditorGUILayout.PropertyField(converterParameterProperty);
                            //EditorGUILayout.PropertyField(notifyOnSourceUpdatedProperty);
                            //EditorGUILayout.PropertyField(notifyOnTargetUpdatedProperty);

                        }
                        break;
                    case BindingBehaviour.BindingType.PriorityBinding:
                        {
                        }
                        break;
                }
            }

            EditorGUI.EndProperty();
        }

    }
}