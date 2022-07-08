using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

public class TestUXMLBinding : EditorWindow
{
    public string value = "A";

    public TestData2 value2 = new TestData2();
      
    TextField fldBindingPath;

    [MenuItem("Test/Uxml Binding")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestUXMLBinding));
        w.Show();
    }


    private void OnEnable()
    {

        rootVisualElement.Add(new IMGUIContainer(() =>
        {
            value = EditorGUILayout.TextField("value", value);
            value2.Value = EditorGUILayout.TextField("value2", value2.Value);
        
            using (new GUILayout.HorizontalScope())
            {

                if (isBind)
                {
                    if (GUILayout.Button("Unbind"))
                    {
                        Unbind();
                    }
                }
                else
                {
                    if (GUILayout.Button("Bind"))
                    {
                        Bind();
                    }
                }
            }
        }));


        fldBindingPath = new TextField();
        fldBindingPath.label = "bindingPath";
        fldBindingPath.bindingPath = nameof(value);
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"[value] bindingPath changed: {e.newValue}");
            e.StopPropagation();
        });
        rootVisualElement.Add(fldBindingPath);

        var fld = new TextField();
        fld.label = "value2.value";
        fld.bindingPath = "value2.value";
        fld.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"[value2.value] bindingPath changed: {e.newValue}");
        });
        rootVisualElement.Add(fld);


        rootVisualElement.RegisterCallback<ChangeEvent<string>>(e =>
        {
            Debug.Log($"{e.target} change " + e.newValue);
        });

        Bind();


    }
    bool isBind;
    void Bind()
    {
        isBind = true;

        Debug.Log($"bindingPath [{fldBindingPath.bindingPath}], binding null: {fldBindingPath.binding == null}");
        Debug.Log("Bind");
        rootVisualElement.Bind(new SerializedObject(this));
        Debug.Log($"bindingPath [{fldBindingPath.bindingPath}], binding null: {fldBindingPath.binding == null}");
    }

    void Unbind()
    {
        isBind = false;
        Debug.Log($"bindingPath [{fldBindingPath.bindingPath}], binding null: {fldBindingPath.binding == null}");
        Debug.Log("Unbind");
        rootVisualElement.Unbind();
        Debug.Log($"bindingPath [{fldBindingPath.bindingPath}], binding null: {fldBindingPath.binding == null}");
    }

}
