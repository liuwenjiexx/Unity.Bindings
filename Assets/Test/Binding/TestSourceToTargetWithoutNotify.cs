using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

public class TestSourceToTargetWithoutNotify : EditorWindow
{

    public TestData data = new TestData();

    [MenuItem("Test/Source To Target Without Notify")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestSourceToTargetWithoutNotify));
        w.Show();
    }


    private void OnEnable()
    {

        rootVisualElement.Add(new IMGUIContainer(() =>
        {
            data.Value = EditorGUILayout.TextField("Source", data.Value);
        }));


        BindingOptions options = new BindingOptions()
        {
            SourceToTargetNotifyEnabled = false,
        };
        var fldProperty = new TextField();
        fldProperty.label = "Target";
        fldProperty.BindProperty(data, o => o.Value, options);
        fldProperty.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Value: {e.newValue}");
        });
        rootVisualElement.Add(fldProperty);


    }


}
