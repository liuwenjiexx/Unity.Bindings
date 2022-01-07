using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

public class TestBinding : EditorWindow, /*IPropertyAccessor,*/ INotifyPropertyChanged
{

    public string value = "A";

    public event PropertyChangedEventHandler PropertyChanged;

    public string Value
    {
        get => value;
        set
        {
            if (value != this.value)
            {
                this.value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }

    public Value2 value2;

    [Serializable]
    public class Value2
    {
        public string value;
    }
    public bool CanReadProperty(string name)
    {
        return true;
    }

    public bool CanWriteProperty(string name)
    {
        return true;
    }

    public object GetPropertyValue(string propertyName)
    {
        switch (propertyName)
        {
            case "Value":
                return Value;
        }
        throw new MemberAccessException($"Not Property: {propertyName}");
    }

    public void SetPropertyValue(string propertyName, object value)
    {
        switch (propertyName)
        {
            case "Value":
                Value = (string)value;
                break;
            default:
                throw new MemberAccessException($"Not Property: {propertyName}");
        }

    }


    [MenuItem("Test/Binding")]
    public static void ShowWindow()
    {
        EditorWindow w = EditorWindow.GetWindow(typeof(TestBinding));
        w.Show();
    }

    [MenuItem("Test/GC")]
    public static void AutoGC()
    {
        L1();
    }
    static void L1()
    {
        Debug.Log("L1");
        L2();
    }
    static void L2()
    {
        Debug.Log("L2");
        var st = new System.Diagnostics.StackTrace(1, true);
        string stacktrace = st.ToString();
        Debug.Log(stacktrace);
        Debug.developerConsoleVisible = !Debug.developerConsoleVisible;
    }

    private void OnEnable()
    {

        rootVisualElement.Add(new IMGUIContainer(() =>
        {
            GUILayout.Label("GUI Layout Begin");
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Button("A");
                GUILayout.Button("A");
                GUILayout.Button("A");
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Button("A", EditorStyles.miniButtonLeft);
                GUILayout.Button("A", EditorStyles.miniButton);
                GUILayout.Button("A", EditorStyles.miniButtonRight);
            }
            GUILayout.Button("A", EditorStyles.toolbar);
            using (new GUILayout.HorizontalScope())
            {

                GUILayout.Button("A", EditorStyles.toolbarButton);
                GUILayout.Button("A", EditorStyles.toolbarButton);
                GUILayout.Button("A", EditorStyles.toolbarButton);
            }

            Value = EditorGUILayout.TextField("Value", Value);

            GUILayout.Label("GUI Layout End");

        }));

        VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/Test/Binding/TestBinding.uxml");
        VisualElement ui = uiAsset.CloneTree();

        ui.style.flexGrow = new StyleFloat(1f);

        var fldGameObjectName = ui.Q<TextField>("gameobject_name");
        fldGameObjectName.bindingPath = "m_Name";

        rootVisualElement.Add(ui);
        StyleSheet lastStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"Assets/Test/Binding/TestBinding.uss");
        rootVisualElement.styleSheets.Add(lastStyle);

        var fld = new TextField();
        fld.label = "IBindable.bindingPath";
        fld.bindingPath = "value";
        rootVisualElement.Add(fld);

        fld = new TextField();
        fld.label = "binding value";
        fld.Bind<string>(this, "Value");
        rootVisualElement.Add(fld);

        fld = new TextField();
        fld.label = "binding value2.value";
        fld.Bind<string>(this, "value2.value");
        rootVisualElement.Add(fld);

        //fld = new TextField();
        //fld.label = "PropertyBinding setter<Property,T>";
        //fld.Bind(this, "Value", (name) => Value, (name, val) => Value = val);
        //rootVisualElement.Add(fld);

        //fld = new TextField();
        //fld.label = "IBindableProperty accessor";
        //fld.Bind<string>(this, "Value", this);
        //rootVisualElement.Add(fld);

        fld = new TextField();
        fld.label = "Binding setter<T>";
        fld.Bind(this, "Value", new Accessor<string>(() => Value, (val) => Value = val));
        rootVisualElement.Add(fld);

        var lbl = new Label();

        lbl.Bind(new Accessor<string>(() => lbl.text, (v) => lbl.text = v), this, "Value", new Accessor<string>(() => Value, (val) => Value = val));
        rootVisualElement.Add(lbl);
        //menu

        var btnMenu = new Button();
        btnMenu.text = "Menu";
        ContextualMenuManipulator m = new ContextualMenuManipulator((e) =>
        {
            e.menu.AppendAction("Hello", (menuItem) =>
            {
                Debug.Log(menuItem.name + ": Hello World");
            }, DropdownMenuAction.Status.Normal);
            e.menu.AppendAction("World", (menuItem) =>
            {
                Debug.Log(menuItem.name + ": Hello World");
            }, DropdownMenuAction.Status.Normal);
        });

        btnMenu.AddManipulator(m);
        rootVisualElement.Add(btnMenu);


        //menu

        rootVisualElement.Add(new InspectorElement(this));

        rootVisualElement.Bind(new SerializedObject(this));
        OnSelectionChange();


    }

    private void CreateGUI()
    {



    }

    public void OnSelectionChange()
    {
        //GameObject selectedObject = Selection.activeObject as GameObject;
        //if (selectedObject != null)
        //{
        //    // Create serialization object
        //    SerializedObject so = new SerializedObject(selectedObject);
        //    // Bind it to the root of the hierarchy. It will find the right object to bind to...
        //    rootVisualElement.Bind(so);
        //}
        //else
        //{
        //    // Unbind the object from the actual visual element
        //    rootVisualElement.Unbind();

        //    // Clear the TextField after the binding is removed
        //    // (this code is not safe if the Q() returns null)
        //    rootVisualElement.Q<TextField>("gameobject_name").value = "";
        //}
    }



    class Binding : IBinding
    {
        public TextField field;
        private INotifyPropertyChanged source;

        public Binding(INotifyPropertyChanged source)
        {
            this.source = source;
            source.PropertyChanged += Source_PropertyChanged;
        }

        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private string value;
        public string Value
        {
            get => value;
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                }
            }
        }

        public void PreUpdate()
        {
            Debug.Log("PreUpdate");
        }

        public void Release()
        {
            Debug.Log("Release");
        }

        public void Update()
        {
            Debug.Log("Update");

        }
    }


    /*public class MainElement : VisualElement
    {
        private PositionField position;
        void Initialize()
        {
            position = new PositionField();
            position.bindingPath = "Data.Rect";
            Add(position);
            
            this.Bind(mySerializedObject);

            position.RegisterValueChangedCallback(e => {
                transform.position = e.newValue.position;
            });

            public void OnStartDrag(DragContext ctx)
            {
                AddToClassList("--dragging");
                isDragging = true;
            }

            public void OnUpdateDrag(DragContext ctx)
            {
                var pos = position.value;
                pos.position += ctx.mouseDelta;
                position.value = pos;
            }

            public void OnEndDrag(DragContext ctx)
            {
                isDragging = false;
                RemoveFromClassList("--dragging");
                serializedViewData.ApplyModifiedProperties();
            }
        }

    }*/

}
