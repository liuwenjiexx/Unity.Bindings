using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

public class TestBindingPath : EditorWindow
{

    TextField fldBindingPath;
    BindingSet<TestData> bindingSet;

    TestData data = new TestData()
    {
        Value = "a",
        Data2 = new TestData2()
        {
            Value = "b"
        }
    };

    [MenuItem("Test/Binding Path")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestBindingPath));
        w.Show();
    }


    private void OnEnable()
    {
        rootVisualElement.Add(new IMGUIContainer(() =>
        {
            data.Value = EditorGUILayout.TextField("Value", data.Value);
            data.Data2.Value = EditorGUILayout.TextField("Data2.Value", data.Data2.Value);
            if (GUILayout.Button("Set Data2"))
            {
                data.Data2 = new TestData2()
                {
                    Value = Random.value.ToString()
                };
            }

            using (new GUILayout.HorizontalScope())
            {
                if (bindingSet.IsBinding)
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

        bindingSet = new BindingSet<TestData>(data);

        fldBindingPath = new TextField();
        fldBindingPath.label = "Value";
        bindingSet.Bind(fldBindingPath, nameof(TestData.Value));
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Path [Value] changed: {e.newValue}");
            e.StopPropagation();
        });
        rootVisualElement.Add(fldBindingPath);


        fldBindingPath = new TextField();
        fldBindingPath.label = "bindingPath Value";
        fldBindingPath.bindingPath = nameof(TestData.Value);
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"bindingPath [Value] changed: {e.newValue}");
            e.StopPropagation();
        });
        rootVisualElement.Add(fldBindingPath);

        var fld = new TextField();
        fld.label = "Data2.Value";
        bindingSet.Bind(fld, "Data2.Value");
        fld.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"bindingPath [Data2.Value] changed: {e.newValue}");
        });
        rootVisualElement.Add(fld);

        fld = new TextField();
        fld.label = "bindingPath Data2.Value";
        fld.bindingPath = "Data2.Value";
        fld.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"bindingPath [Data2.Value] changed: {e.newValue}");
        });
        rootVisualElement.Add(fld);

        VisualElement h;
        h = new VisualElement();
        h.style.flexDirection = FlexDirection.Row;
        h.Add(new Label() { text = "Target Selector" });

        var fldTargetSelector = new Label();
        bindingSet.Bind(fldTargetSelector, o => o.text, nameof(TestData.Value));
        fldTargetSelector.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Target Selector: {e.newValue}");
            e.StopPropagation();
        });
        h.Add(fldTargetSelector);
        rootVisualElement.Add(h);


        rootVisualElement.RegisterCallback<ChangeEvent<string>>(e =>
        {
            Debug.Log($"{e.target} change " + e.newValue);
        });

        bindingSet.CreateBinding(rootVisualElement);

        Bind();
    }

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
