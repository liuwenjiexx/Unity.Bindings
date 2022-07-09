using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

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

        var fldProperty = new TextField();
        fldProperty.label = "Target Notify enabled";
        fldProperty.Bind(data).From(o => o.Value).EnableSourceToTargetNotify().Build();
        fldProperty.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Target Notify enabled: {e.newValue}");
        });
        rootVisualElement.Add(fldProperty);


        fldProperty = new TextField();
        fldProperty.label = "Target Notify disabled";
        fldProperty.Bind(data).From(o => o.Value).DisableSourceToTargetNotify().Build();
        fldProperty.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Target Notify disabled: {e.newValue}");
        });
        rootVisualElement.Add(fldProperty);

        rootVisualElement.BindAll();

    }


}
