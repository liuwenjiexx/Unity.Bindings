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
        fldBindingPath.label = "Value";
        fldBindingPath.BindPath<string>(data, nameof(TestData.Value));
        fldBindingPath.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"bindingPath [Value] changed: {e.newValue}");
            e.StopPropagation();
        });
        rootVisualElement.Add(fldBindingPath);

        var fld = new TextField();
        fld.label = "Data2.Value";
        fld.BindPath<string>(data, "Data2.Value");
        fld.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"bindingPath [Data2.Value] changed: {e.newValue}");
        });
        rootVisualElement.Add(fld);

        rootVisualElement.RegisterCallback<ChangeEvent<string>>(e =>
        {
            Debug.Log($"{e.target} change " + e.newValue);
        });

        isBind = true;
    }


    void Bind()
    {

        Debug.Log("Bind");
        rootVisualElement.BindAll();

    }

    void Unbind()
    {
        isBind = false;
        rootVisualElement.UnbindAll();
    }

}
