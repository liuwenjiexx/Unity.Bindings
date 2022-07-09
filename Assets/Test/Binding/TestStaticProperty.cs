using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

public class TestStaticProperty : EditorWindow
{

    public static string staticProperty = "1";

    public static string StaticProperty
    {
        get => staticProperty;
        set => StaticPropertyChanged.Invoke(null, nameof(StaticProperty), ref staticProperty, value);
    }
    public static event PropertyChangedEventHandler StaticPropertyChanged;


    [MenuItem("Test/Static Property")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestStaticProperty));
        w.Show();
    }
    private void OnEnable()
    {

        rootVisualElement.Add(new IMGUIContainer(() =>
        {
            StaticProperty = EditorGUILayout.TextField("StaticValue", StaticProperty);

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

        var fldStatic = new TextField();
        fldStatic.label = "Static Property";
        fldStatic.Bind()
            .From(() => StaticProperty)
            .SourceNotify((handler, b) =>
            {
                if (b)
                    StaticPropertyChanged += handler;
                else
                    StaticPropertyChanged -= handler;
            }).Build();

        fldStatic.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Static Property: {StaticProperty}: {e.newValue}");
        });
        rootVisualElement.Add(fldStatic);

        Bind();

    }

    bool isBind;
    void Bind()
    {
        isBind = true;
        rootVisualElement.BindAll();
    }

    void Unbind()
    {
        isBind = false;
        rootVisualElement.UnbindAll();
    }
}
