using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

public class TestBindingArray : EditorWindow
{

    TextField fldBindingPath;

    public TestData[] array = new TestData[] {
        new TestData(){ Value= "abc" },
        new TestData(){ Value= "123" }
    };

    public TestData this[int index]
    {
        get => array[index];
        set => array[index] = value;
    }

    [MenuItem("Test/Binding Array")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestBindingArray));
        w.Show();
    }

    VisualElement contentRoot;

    private void OnEnable()
    {
        foreach (var pInfo in GetType().GetProperties())
        {
            if (pInfo.Name == "Item")
            {

            }
        }
        contentRoot = new VisualElement();
        contentRoot.Add(new IMGUIContainer(() =>
        {
            array[0].Value = EditorGUILayout.TextField("[0].Value", array[0].Value);
            array[1].Value = EditorGUILayout.TextField("[1].Value", array[1].Value);

            if (GUILayout.Button("Change [0]"))
            {
                array[0] = new TestData() { Value = Random.value.ToString() };
            }

            using (new GUILayout.HorizontalScope())
            {
                if (bindingSet.IsBinding )
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

        bindingSet = new BindingSet<TestBindingArray>(this);

        fldBindingPath = new TextField();
        fldBindingPath.label = "bindingPath array[0].Value";
        fldBindingPath.bindingPath = "array[0].Value";
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"[array[0].Value]: {e.newValue}");
            e.StopPropagation();
        });
        contentRoot.Add(fldBindingPath);

        fldBindingPath = new TextField();
        fldBindingPath.label = "array[1].Value";
        bindingSet.Build(fldBindingPath).From("array[1].Value");
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"[array[0].Value]: {e.newValue}");
            e.StopPropagation();
        });
        contentRoot.Add(fldBindingPath);

        fldBindingPath = new TextField();
        fldBindingPath.label = "bindingPath array[1].Value";
        fldBindingPath.bindingPath = "array[1].Value";
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"[array[1].Value]: {e.newValue}");
            e.StopPropagation();
        });
        contentRoot.Add(fldBindingPath);


        fldBindingPath = new TextField();
        fldBindingPath.label = "bindingPath this[0].Value";
        fldBindingPath.bindingPath = "[0].Value";
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"this[0].Value]: {e.newValue}");
            e.StopPropagation();
        });
        contentRoot.Add(fldBindingPath);

        fldBindingPath = new TextField();
        fldBindingPath.label = "this[0].Value";
        bindingSet.Build(fldBindingPath, this).From("[0].Value");

        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"array.[0].Value]: {e.newValue}");
            e.StopPropagation();
        });

        contentRoot.Add(fldBindingPath);

        bindingSet.CreateBinding(contentRoot);
        Bind();
    }
    private void CreateGUI()
    {
        rootVisualElement.Add(contentRoot);
    }

    BindingSet<TestBindingArray> bindingSet;

    void Bind()
    {
        Debug.Log("Bind");
        bindingSet.Bind();
    }

    void Unbind()
    {
        bindingSet.Unbind();
    }

}
