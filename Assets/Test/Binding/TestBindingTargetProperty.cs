using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

public class TestBindingTargetProperty : EditorWindow
{
    public TestData data = new TestData() { Value = "a" };


    [MenuItem("Test/Binding Custom Property")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestBindingTargetProperty));
        w.Show();
    }

    TextField fldAccessor;

    private void OnEnable()
    {

        rootVisualElement.Add(new IMGUIContainer(() =>
        {
            data.Value = EditorGUILayout.TextField("Value", data.Value);
        }));

        var label = new Label();
        label.Bind<Label, TestData, string>(o => o.text, data, o => o.Value);
        rootVisualElement.Add(label);
        rootVisualElement.BindAll();
    }


}
