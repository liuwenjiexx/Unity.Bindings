using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Bindings;

public class TestStaticProperty : EditorWindow
{
    BindingSet bindingSet;

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

        bindingSet = new BindingSet();


        var fldStatic = new TextField();
        fldStatic.label = "Static Property";
        bindingSet.Build(fldStatic)
            .From(() => StaticProperty)
            .SourceNotify((handler, b) =>
            {
                if (b)
                    StaticPropertyChanged += handler;
                else
                    StaticPropertyChanged -= handler;
            });

        fldStatic.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Static Property: {StaticProperty}: {e.newValue}");
        });
        rootVisualElement.Add(fldStatic);


        Bind();

    }

    void Bind()
    {
        bindingSet.Bind();
    }

    void Unbind()
    {
        bindingSet.Unbind();
    }
}
