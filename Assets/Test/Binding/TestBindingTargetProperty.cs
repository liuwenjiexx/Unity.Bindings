using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Bindings;

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

        BindingSet<TestData> bindingSet = new BindingSet<TestData>(data);

        var label = new Label();
        bindingSet.Bind<Label, string>(label, o => o.text,  o => o.Value);
 
        rootVisualElement.Add(label);
        
        bindingSet.Bind();
    }


}
