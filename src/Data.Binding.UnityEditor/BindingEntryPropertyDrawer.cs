using LWJ.Unity;
using System;
using UnityEditor;
using UnityEngine;
using LWJ.Unity;
namespace LWJ.UnityEditor
{

    [CustomPropertyDrawer(typeof(Binding.Entry), true)]
    public class BindingEntryPropertyDrawer : PropertyDrawer
    {
        SerializedProperty sourceProperty;
        SerializedProperty sourceTypeProperty;
        SerializedProperty sourceNameProperty;
        SerializedProperty ancestorLevelProperty;
        SerializedProperty relativeProperty;
        //SerializedProperty nameSourceProperty;
        SerializedProperty pathProperty;
        SerializedProperty targetProperty;
        SerializedProperty targetPathProperty;
        SerializedProperty targetNullValueProperty;
        SerializedProperty fallbackValueProperty;
        SerializedProperty stringFormatProperty;
        SerializedProperty modeProperty;
        SerializedProperty converterProperty;
        SerializedProperty converterParameterProperty;
        SerializedProperty notifyOnSourceUpdatedProperty;
        SerializedProperty notifyOnTargetUpdatedProperty;
        SerializedProperty bindingsProperty;
        SerializedProperty bindingTypeProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (label == GUIContent.none)
                return 0;
            return base.GetPropertyHeight(property, label);
        }






        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // if (sourceTypeProperty == null)
            {
                sourceProperty = property.FindPropertyRelative("source");
                sourceTypeProperty = property.FindPropertyRelative("sourceType");
                sourceNameProperty = property.FindPropertyRelative("sourceName");
                ancestorLevelProperty = property.FindPropertyRelative("ancestorLevel");
                relativeProperty = property.FindPropertyRelative("relativeSource");
                pathProperty = property.FindPropertyRelative("path");
                targetProperty = property.FindPropertyRelative("target");
                targetPathProperty = property.FindPropertyRelative("targetPath");
                targetNullValueProperty = property.FindPropertyRelative("nullValue");
                fallbackValueProperty = property.FindPropertyRelative("fallbackValue");
                stringFormatProperty = property.FindPropertyRelative("stringFormat");
                modeProperty = property.FindPropertyRelative("mode");
                converterProperty = property.FindPropertyRelative("converter");
                converterParameterProperty = property.FindPropertyRelative("converterParameter");
                notifyOnSourceUpdatedProperty = property.FindPropertyRelative("enabledSourceUpdated");
                notifyOnTargetUpdatedProperty = property.FindPropertyRelative("enabledTargetUpdated");
                bindingsProperty = property.FindPropertyRelative("bindings");
                bindingTypeProperty = property.FindPropertyRelative("bindingType");
            }

            Binding.BindingType bindingType;

            bindingType = (Binding.BindingType)bindingTypeProperty.intValue;


            EditorGUI.BeginProperty(position, label, property);

            if (label != GUIContent.none)
                EditorGUI.LabelField(position, label);

            using (new GUILayout.VerticalScope())
            {

                if (bindingType == Binding.BindingType.Binding)
                {
                    DrawSource(property);
                    EditorGUILayout.PropertyField(pathProperty);
                }


                Type targetType = null;
                //GameObject targetGo = ((BindingBehaviour)property.serializedObject.targetObject).gameObject;

                //if (targetProperty.objectReferenceValue != null)
                //{
                //if (targetProperty.objectReferenceValue is GameObject)
                //{
                //    if (targetProperty.objectReferenceValue != targetGo)
                //        targetProperty.objectReferenceValue = null;
                //}
                //else if (targetProperty.objectReferenceValue is Component)
                //{
                //    if (((Component)targetProperty.objectReferenceValue).gameObject != targetGo)
                //        targetProperty.objectReferenceValue = null;
                //}
                //else
                //{
                //    targetProperty.objectReferenceValue = null;
                //}

                //}
                //if (targetProperty.objectReferenceValue == null)
                //    targetProperty.objectReferenceValue = targetGo;

                //targetProperty.objectReferenceValue = EditorGUIHelper.ComponentAndGameObjectPop(targetProperty.displayName, targetProperty.objectReferenceValue);
                EditorGUILayout.PropertyField(targetProperty);
                EditorGUILayout.PropertyField(targetPathProperty);
                EditorGUILayout.PropertyField(targetNullValueProperty);
                EditorGUILayout.PropertyField(fallbackValueProperty);
                EditorGUILayout.PropertyField(stringFormatProperty);

                switch (bindingType)
                {
                    case Binding.BindingType.Binding:
                        {
                            EditorGUILayout.PropertyField(modeProperty);
                            BindingEditor.ValueConverterField(converterProperty);

                            EditorGUILayout.PropertyField(converterParameterProperty);

                            //EditorGUILayout.PropertyField(notifyOnSourceUpdatedProperty);

                            //EditorGUILayout.PropertyField(notifyOnTargetUpdatedProperty);
                        }
                        break;
                    case Binding.BindingType.MultiBinding:
                        {
                            EditorGUILayout.PropertyField(modeProperty);
                            //EditorGUILayout.PropertyField(converterProperty);
                            BindingEditor.MultiValueConverterField(converterProperty);
                            EditorGUILayout.PropertyField(converterParameterProperty);
                            //EditorGUILayout.PropertyField(notifyOnSourceUpdatedProperty);
                            //EditorGUILayout.PropertyField(notifyOnTargetUpdatedProperty);

                            using (new EditorGUILayoutScopes.IndentLevel())
                            {
                                // DrawEntryArray(childProperty);
                                // EditorGUILayout.PropertyField(childProperty, true);
                                //BindingEditor.DrawEntryArray(childProperty);
                                BindingEditor.DrawMultiBindingChildren(bindingsProperty);
                            }

                        }
                        break;
                    case Binding.BindingType.PriorityBinding:
                        {
                        }
                        break;
                }
            }

            EditorGUI.EndProperty();
        }



        public static void DrawSource(SerializedProperty item)
        {
            var sourceProperty = item.FindPropertyRelative("source");
            var sourceTypeProperty = item.FindPropertyRelative("sourceType");
            var sourceNameProperty = item.FindPropertyRelative("sourceName");
            var ancestorLevelProperty = item.FindPropertyRelative("ancestorLevel");
            var relativeProperty = item.FindPropertyRelative("relativeSource");
            GameObject go = ((Binding)item.serializedObject.targetObject).gameObject;

            //sourceProperty.objectReferenceValue = EditorGUIHelper.ComponentAndGameObjectPop(sourceProperty.displayName, go, sourceProperty.objectReferenceValue, true);

            //EditorGUIHelper.ComponentPopup(new GUIContent(sourceProperty.displayName), sourceProperty);
            EditorGUILayout.PropertyField(sourceProperty);
            EditorGUILayout.PropertyField(sourceTypeProperty);
            EditorGUILayout.PropertyField(sourceNameProperty);
            EditorGUILayout.PropertyField(ancestorLevelProperty);

            //sourceTypeProperty.intValue = (int)(object)EditorGUILayout.EnumPopup(sourceTypeProperty.displayName, (BindingBehaviour.SourceType)sourceTypeProperty.intValue);

            /*
            using (new EditorGUILayoutScopes.IndentLevel())
            {
                switch ((BindingBehaviour.SourceType)sourceTypeProperty.intValue)
                {
                    //case BindingBehaviour.SourceType.Relative:
                    //    EditorGUILayout.PropertyField(relativeProperty, (GUIContent.none));
                    //    break;
                    case BindingBehaviour.SourceType.Name:
                        var nameSourceProperty = item.FindPropertyRelative("nameSource");
                        SerializedProperty nameProperty = nameSourceProperty.FindPropertyRelative("name");
                        SerializedProperty typeNameProperty = nameSourceProperty.FindPropertyRelative("typeName");
                        SerializedProperty findModeProperty = nameSourceProperty.FindPropertyRelative("findMode");

                        EditorGUI.BeginProperty(GUILayoutUtility.GetRect(0, Screen.width, 0, Screen.height), new GUIContent(nameSourceProperty.displayName), nameSourceProperty);

                        using (new GUILayout.VerticalScope())
                        {
                            EditorGUILayout.PropertyField(findModeProperty);
                            EditorGUILayout.PropertyField(nameProperty);
                            EditorGUILayout.PropertyField(typeNameProperty);
                        }

                        EditorGUI.EndProperty();

                        break;
                    case BindingBehaviour.SourceType.Source:
                    default:

                        sourceProperty.objectReferenceValue = EditorGUIHelper.ComponentAndGameObjectPop(sourceProperty.displayName, go, sourceProperty.objectReferenceValue, true);
                        break;
                }
            }*/
        }
    }




}
