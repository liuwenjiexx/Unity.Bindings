# Binding

数据绑定

| 特性                   | 支持 |
| ---------------------- | ---- |
| 路径                   | ✔    |
| 数组                   | ✔    |
| 索引                   | ✔    |
| 静态源属性             | ✔    |
| 静态目标属性           |      |
| 双向                   | ✔    |
| INotifyPropertyChanged | ✔    |
| INotifyValueChanged    | ✔    |
| UIElements             | ✔    |

## 快速使用

1. 创建绑定集

```c#
var bindingSet = new BindingSet<TestData>(data);
```

`TestData` 为 [样例数据](#样例数据)

1. 设置 `bindingPath`

```c#
textField.bindingPath = "Value";
```

3. 调用 `CreateBinding` 根据 `bindingPath` 创建绑定 `binding` 对象

```c#
bindingSet.CreateBinding(rootVisualElement);
```

4. 调用 `Bind` 方法执行绑定

```c#
bindingSet.Bind();
```

## 绑定对象

### INotifyValueChanged

默认绑定到 `INotifyValueChanged.value` 属性，默认使用 `SetValueWithoutNotify` 设置值，不会触发 `RegisterValueChangedCallback`，通过开启 [EnableSourceToTargetNotify](#源到目标通知) 使用 `value` 设置值，值传递默认为 `TwoWay` 模式

```c#
BindingBase BindPath<TValue>(this INotifyValueChanged<TValue> target, object source, string path)
```

- **target**

  目标对象

- **source**

  源对象

- **path**

  源对象的属性，多个属性用 `.` 分隔

**样例**

```c#
var bindingSet = new BindingSet<TestData>(data);

bindingSet.Bind(textField, nameof(TestData.Value));
bindingSet.Bind(textField, "Value");
bindingSet.Bind(textField, "Data2.Value");

//绑定
bindingSet.Bind();
```

**VisualElement**

可以绑定实现了 `IBindable` 接口的 `VisualElement` 对象，如：`TextField`，`FloatField`，`IntegerField`，`EnumField` 等， [支持 IBindable 默认绑定完整列表](https://docs.unity3d.com/Manual/UIE-Binding.html)

### IBindable.bindingPath

```c#
var bindingSet = new BindingSet<TestData>(data);

textField.bindingPath = "Value";

// bindingPath生成 binding 对象
bindingSet.CreateBinding(rootVisualElement);

//绑定
bindingSet.Bind();
```

源到目标值传递修改，`UXML bindingPath` 默认会通知 `RegisterValueChangedCallback`，该 `Bind` 默认使用 `SetValueWithoutNotify` 不会进行通知，以区分用户输入值



`CreateBinding` 根据 `bindingPath` 生成 `binding` 对象，条件如果  `binding` 为空且 `bindingPath` 不为空，则创建新的 `Binding`

如果要兼容 UXML 绑定 需要在 `Bind` 之前调用 `root.Bind(SerializedObject)`，让UXML先生成 `binding` 对象

## 绑定属性

### 选择器

```c#
Expression<Func<TTarget, TValue>> targetPropertySelector
Expression<Func<TSource, TValue>> propertySelector
```

**样例**

```c#
bindingSet.Bind(label, o => o.text, "Value");
bindingSet.Bind(textField, o => o.Value);
```


### 访问器

```c#
IAccessor targetAccessor
IAccessor accessor
```

**样例**

`backgroundImage` 属性

```c#
bindingSet.Bind(icon, new Accessor<VisualElement, Texture2D>(
    (o) => o.style.backgroundImage.value.texture,
    (o, v) =>
    {
        o.style.backgroundImage = v;
    }), nameof(Icon));
```

`enabledSelf` 属性

```c#
bindingSet.Bind(button, 
   new Accessor<Button, bool>(
       (t) => btn.enabledSelf, 
       (t, v) => btn.SetEnabled(v)), 
   nameof(Enabled));
```

### 索引

支持数组 `T[]`，列表 `IList`，枚举器 `IEnumerable`

```c#
public TestData[] array = new TestData[] {
    new TestData(){ Value = "abc" },
    new TestData(){ Value = "123" }
};

textField.bindingPath = "array[0].Value";
bindingSet.Bind(textField, "array[0].Value");
```

重新指定 `Source`

```c#
bindingSet.Build(textField, array).From("[0].Value");
```

**This 索引器**

`this[int index]`，索引器类型支持：int， string

```c#
public TestData this[int index]
{
    get => array[index];
    set => array[index] = value;
}

textField.bindingPath = "[0].Value";
bindingSet.Build(textField, this).From("[0].Value");
```



## 解绑

**BindingBase**

```c#
//绑定
binding.Bind();

//解绑
binding.Unbind();
```

` binding.IsBinding` 判断是否绑定

**BindingSet** 

```c#
//绑定
bindingSet.Bind();

//解绑
bindingSet.Unbind();
```

## 绑定生成器 

生成复杂的绑定，`Bind` 方法获取 `BindingBuilder` 生成器，最后调用 `Build` 生成绑定

```c#
bindingSet.Build(Target) //绑定 Source 的静态属性
bindingSet.Build(Target, Source)
```

**样例**

```c#
bindingSet.Build(textField).From(o => o.Value);
```

**模式**

默认源到目标，`INotifyValueChanged` 默认为 `TwoWay` 模式

- **OneWay**

  源到目标

  ```c#
  bindingSet.Build(label).From(o => o.Value).OneWay();
  ```

- **OneWayToSource**

  目标到源

  ```c#
  bindingSet.Build(textField).From(o => o.Value).OneWayToSource();
  ```

- **TwoWay**

  双向模式，源到目标和目标到源

```c#
bindingSet.Build(textField).From(o => o.Value).TwoWay();
```

- **To**

  目标属性

  ```c#
  To<TValue>(Expression<Func<TTarget, TValue>> targetPropertySelector)
  ```

  自动设置 `TargetPropertyName`

  ```c#
  To(IAccessor targetAccessor)
  ```

  需要手动指定 `TargetPropertyName`

- **TargetPropertyName**

  目标属性名称，需要 `Target` 支持 `INotifyPropertyChanged`，监听属性值变化事件

- **From**

  源属性

  ```c#
  From(string path)
  ```

  使用属性路径绑定

  ```c#
  From<TValue>(Expression<Func<TSource, TValue>> propertySelector)
  From<TValue>(Expression<Func<TValue>> propertySelector)
  ```

  自动设置 `TargetPropertyName`

  ```c#
  From(IAccessor accessor)
  ```

  需要手动指定 `TargetPropertyName`

- **PropertyName**

  源属性名称，需要 `Source` 支持 `INotifyPropertyChanged`，监听属性值变化事件

- **EnableSourceToTargetNotify**

  是否开启源到目标通知，`DisableSourceToTargetNotify` 为关闭

```c#
bindingSet.Build(textField).From(o => o.Value).EnableSourceToTargetNotify();
```

- **TargetNotify**

  定制目标属性变化通知

- **SourceNotify**

  定制源属性属性变化通知，事件类型为 `PropertyChangedEventHandler`

  ```c#
  SourceNotify((handler, b) =>
               {
                    if (b)
                        StaticPropertyChanged += handler;
                    else
                        StaticPropertyChanged -= handler;
                })
  ```

  

**绑定静态属性样例**

```c#
//静态属性
static string staticProperty;
static string StaticProperty
{
    get => staticProperty;
    set => StaticPropertyChanged.Invoke(null, nameof(StaticProperty), ref staticProperty, value);
}

//静态属性通知
static event PropertyChangedEventHandler StaticPropertyChanged;

//获取 BindingBuilder 绑定生成器, source为null
bindingSet.Build(textField)
    //设置绑定源属性
    .From(() => StaticProperty)
    //定制源属性通知
    .SourceNotify((handler, b) =>
       {
           if (b)
               StaticPropertyChanged += handler;
           else
               StaticPropertyChanged -= handler;
       });

//绑定
bindingSet.Bind();
```



## 定制读写器

**访问器接口**

```c#
public interface IAccessor
{
    bool CanGetValue(object target);
    bool CanSetValue(object target);
    object GetValue(object target);
    void SetValue(object target, object value);
}
```

**样例**

Button.enabledSelf

```c#
bindingSet.Bind(btn, 
   new Accessor<Button, bool>((t) => btn.enabledSelf, 
                              (t, v) => btn.SetEnabled(v)), 
   nameof(IsAvailable));
```

验证值

```c#
bindingSet.Build(textField).From((o) => MyDirectory, (o, value) =>
	{
        if (!value.StartsWith("Assets/", StringComparison.InvariantCultureIgnoreCase))
        {
            textField.SetValueWithoutNotify(MyDirectory);
            return;
        }
        MyDirectory = value;
    });
```

**Accessor 提供的读写器**

- This

```c#
IAccessor This()
```

- This 索引器

```c#
IAccessor Indexer(Type type, object index)
```

- 属性或字段

```c#
IAccessor Member(MemberInfo propertyOrField)
```

`lambda` 属性选择器

```c#
IMemberAccessor<TValue> Member<TTarget, TValue>(Expression<Func<TTarget, TValue>> propertySelector)
IMemberAccessor<TValue> Member<TValue>(Expression<Func<TValue>> propertySelector)
```

- 数组 `T[]`

```c#
IAccessor Array(int index)
```

- 列表 `IList`

```c#
IAccessor List(int index)
```

- 枚举器 `IEnumerable`

```c#
IAccessor Enumerable(int index)
```

枚举器只支持获取，不能写入

## 样例数据

```c#
[Serializable]
public class TestData : INotifyPropertyChanged
{
    private string value;
    public string Value
    {
        get => value;
        set => PropertyChanged.Invoke(this, nameof(Value), ref this.value, value);
    }

    private TestData2 data2;
    public TestData2 Data2
    {
        get => data2;
        set => PropertyChanged.Invoke(this, nameof(Data2), ref data2, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;
}


[Serializable]
public class TestData2 : INotifyPropertyChanged
{
    [SerializeField]
    private string value;
    public string Value
    {
        get => value;
        set => PropertyChanged.Invoke(this, nameof(Value), ref this.value, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
```

