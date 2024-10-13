using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Bindings;

public class TestSourceToTargetNotify : EditorWindow
{

    public TestData data = new TestData();

    [MenuItem("Test/Source To Target Notify")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestSourceToTargetNotify));
        w.Show();
    }


    private void OnEnable()
    {

        rootVisualElement.Add(new IMGUIContainer(() =>
        {
            data.Value = EditorGUILayout.TextField("Source", data.Value);
        }));

        BindingSet<TestData> bindingSet = new BindingSet<TestData>(data);

        var fldProperty = new TextField();
        fldProperty.label = "Target Notify enabled";
        bindingSet.Build(fldProperty).From(o => o.Value).EnableSourceToTargetNotify();
        fldProperty.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Target Notify enabled: {e.newValue}");
        });
        rootVisualElement.Add(fldProperty);


        fldProperty = new TextField();
        fldProperty.label = "Target Notify disabled";
        bindingSet.Build(fldProperty).From(o => o.Value).DisableSourceToTargetNotify();
        fldProperty.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Target Notify disabled: {e.newValue}");
        });
        rootVisualElement.Add(fldProperty);

        bindingSet.Bind();

    }


}
