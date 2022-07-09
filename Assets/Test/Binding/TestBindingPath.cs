using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

public class TestBindingPath : EditorWindow
{

    TextField fldBindingPath;
    bool isBind;

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
        fldBindingPath.label = "Path";
        fldBindingPath.Bind(data, nameof(TestData.Value));
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Path [Value] changed: {e.newValue}");
            e.StopPropagation();
        });
        rootVisualElement.Add(fldBindingPath);


        fldBindingPath = new TextField();
        fldBindingPath.label = "bindingPath";
        fldBindingPath.bindingPath = nameof(TestData.Value);
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"bindingPath [Value] changed: {e.newValue}");
            e.StopPropagation();
        });
        rootVisualElement.Add(fldBindingPath);

        var fld = new TextField();
        fld.label = "Data2.Value";
        fld.Bind<string>(data, "Data2.Value");
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
        fldTargetSelector.Bind<Label, string>(o => o.text, data, nameof(TestData.Value));
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

        Bind();
    }


    void Bind()
    { 
        Debug.Log("Bind");
        isBind = true;
        rootVisualElement.BindAll(data);

    }

    void Unbind()
    {
        isBind = false;
        rootVisualElement.UnbindAll();
    }

}
