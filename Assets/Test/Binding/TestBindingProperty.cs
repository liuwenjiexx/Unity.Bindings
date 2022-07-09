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
        fldProperty.Bind(data, o => o.Value);
        fldProperty.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Value: {e.newValue}");
        });
        rootVisualElement.Add(fldProperty);

        var fldProperty2 = new TextField();
        fldProperty2.label = "Build Value";
        fldProperty2.Bind(data).From(o => o.Value).Build();
        fldProperty2.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Build Value: {e.newValue}");
        });
        rootVisualElement.Add(fldProperty2);

        var fldCustom = new TextField();
        fldCustom.label = "Source Accessor";
        fldCustom.Bind(data, new Accessor<TestData, string>((o) => o.Value, (o, val) => o.Value = val), nameof(TestData.Value));
        fldCustom.RegisterValueChangedCallback(e =>
        {
            Debug.Log($"Source Accessor: {e.newValue}");
        });
        rootVisualElement.Add(fldCustom);
         

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


        Bind();



    }

    public delegate void GetPropertyChangedEventHandlerDelegate(out PropertyChangedEventHandler handler);

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
