using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Yanmonet.Bindings;

public class TestBindingProperty : EditorWindow
{

    public TestData data = new TestData()
    {
        Value = "a",
        Data2 = new TestData2()
        {
            Value = "b"
        }
    };

    public static string staticProperty = "1";

    public static string StaticProperty
    {
        get => staticProperty;
        set => StaticPropertyChanged.Invoke(null, nameof(StaticProperty), ref staticProperty, value);
    }
    public static event PropertyChangedEventHandler StaticPropertyChanged;


    [MenuItem("Test/Binding Property")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestBindingProperty));
        w.Show();
    }


    private void OnEnable()
    {

        rootVisualElement.Add(new IMGUIContainer(() =>
        {
            data.Value = EditorGUILayout.TextField("Source Value", data.Value);
        //    data.Data2.Value = EditorGUILayout.TextField("Data2.Value", data.Data2.Value);
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



        var fldProperty = new TextField();
        fldProperty.label = "Value";
        fldProperty.BindProperty(data, o => o.Value);
        fldProperty.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Value: {e.newValue}");
        });
        rootVisualElement.Add(fldProperty);

        var fldCustom = new TextField();
        fldCustom.label = "Custom MemberAccessor";
        fldCustom.BindProperty(data, nameof(TestData.Value), new Accessor<TestData, string>((o) => o.Value, (o, val) => o.Value = val));
        fldCustom.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Custom MemberAccessor: {e.newValue}");
        });
        rootVisualElement.Add(fldCustom);

        var fldCustom2 = new TextField();
        fldCustom2.label = "MemberAccessor.From";
        fldCustom2.BindProperty(data, nameof(TestData.Value), Accessor.Member<TestData, string>(o => o.Value));
        fldCustom2.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"MemberAccessor.From: {e.newValue}");
        });
        rootVisualElement.Add(fldCustom2);

        //GetPropertyChangedEventHandlerDelegate aaa;
        //aaa = (out PropertyChangedEventHandler handler) =>
        //{

        //    handler = out StaticPropertyChanged;
        //};
        // PropertyChangedEventHandler handler;
        //aaa(out handler);
        //handler += (s, e) =>
        //{
        //    Debug.Log("-----------"+e.PropertyName);
        //};
        //var aaab= typeof(TestBindingProperty).GetEvent("StaticPropertyChanged");
        //foreach (var m in typeof(TestBindingProperty).GetEvent("StaticPropertyChanged").GetType().GetMethods())
        //{
        //    Debug.Log("xxx:" + m.Name);
        //}

        //Expression<Func<PropertyChangedEventHandler>> aaa = () => StaticPropertyChanged;

        //var mmm = aaa.FindMember();

        //var changedEventInfo = mmm.DeclaringType.GetEvent(mmm.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        //  changedEventInfo.

        var options = new BindingOptions()
        {
            SourceNotify = (handler, b) =>
            {
                if (b)
                    StaticPropertyChanged += handler;
                else
                    StaticPropertyChanged -= handler;
            }
        };

        var fldStatic = new TextField();
        fldStatic.label = "Static Property";
        fldStatic.BindProperty(() => StaticProperty, options);

        fldStatic.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Static Property: {StaticProperty}: {e.newValue}");
        });
        rootVisualElement.Add(fldStatic);

        Bind();



    }

    public delegate void GetPropertyChangedEventHandlerDelegate(out PropertyChangedEventHandler handler);

    bool isBind;
    void Bind()
    {
        isBind = true;

        rootVisualElement.Bind(new SerializedObject(this));
        rootVisualElement.BindAll();
    }

    void Unbind()
    {
        isBind = false;

        rootVisualElement.Unbind();
        rootVisualElement.UnbindAll();
    }

}
