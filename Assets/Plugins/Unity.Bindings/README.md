# Binding

数据绑定

| 特性                   | 支持 |
| ---------------------- | ---- |
| 路径                   | ✔    |
| 静态源属性             | ✔    |
| 静态目标属性           |      |
| 双向                   | ✔    |
| INotifyPropertyChanged | ✔    |
| INotifyValueChanged    | ✔    |
| UIElements             | ✔    |



## 绑定

### INotifyValueChanged

绑定到 `INotifyValueChanged.value` 属性，默认使用 `SetValueWithoutNotify` 设置值，不会触发 `RegisterValueChangedCallback`，通过开启 [EnableSourceToTargetNotify](#源到目标通知) 使用 `value` 设置值，默认为 `TwoWay` 模式

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
textField.Bind(data, nameof(TestData.Value));
textField.Bind(data, "Value");
textField.Bind(data, "Data2.Value");

//绑定
rootVisualElement.BindAll();
```

`TestData` 为 [样例数据](#样例数据)

**VisualElement**

可以绑定实现了 `IBindable` 接口的 `VisualElement` 对象，同时实现  `INotifyValueChanged` 支持绑定 `value` 属性

如：TextField，FloatField，IntegerField，EnumField 等， [支持 IBindable 默认绑定完整列表](https://docs.unity3d.com/Manual/UIE-Binding.html)

### IBindable.bindingPath

```c#
textField.bindingPath = nameof(TestData.Value);

//需要 source 源对象参数
rootVisualElement.BindAll(data);
```



### 目标属性

```c#
BindingBase Bind<TTarget, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, object source, string path)
```

- **targetPropertySelector**

  目标属性选择器

**样例**

```c#
label.Bind<Label, string>(o => o.text, data, "Value");
```



### 源属性

```c#
BindingBase BindProperty<TSource, TValue>(this object target, TSource source, Expression<Func<TSource, TValue>> propertySelector)
```

- **propertySelector**

  源属性选择器

**样例**

```c#
textField.Bind(data, o => o.Value);
```



**同时指定目标和源属性**

```c#
BindingBase BindProperty<TTarget, TSource, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, TSource source, Expression<Func<TSource, TValue>> propertySelector)
```

**样例**

```c#
label.BindProperty<Label, TestData, string>(o => o.text, data, o => o.Value);
```



### 解绑

```c#
//绑定
binding.Bind();

//解绑
binding.Unbind();
```

` binding.IsBinding` 判断是否绑定

**样例**

```c#
var binding = textField.Bind<string>(data, "Value");
binding.Unbind();

//绑定
binding.Bind();
```



### VisualElement

**绑定**

```c#
root.BindAll();
root.BindAll(source);
```

所有子节点调用 `binding.Bind` 方法

如果  `binding` 为空且 `bindingPath` 不为空，则创建新的 `Binding`

如果要兼容 UXML 绑定 需要在 `BindAll` 之前调用 `root.Bind(SerializedObject)`，将生成 `binding` 对象

**解绑**

```c#
root.UnbindAll();
```

所有子节点调用 `binding.Unbind` 方法

## 绑定生成器 

```c#
static BindingBuilder<TTarget, TSource> Bind<TTarget, TSource>(this TTarget target, TSource source)

static BindingBuilder<TTarget, object> Bind<TTarget>(this TTarget target)
```

`BindingBuilder` 生成复杂的绑定

**样例**

```c#
textField.Bind(data).From(o => o.Value).Build();
```

### 模式

默认源到目标，`INotifyValueChanged` 默认为 `TwoWay` 模式

- OneWay

  源到目标

  ```c#
  label.Bind(data).From(o => o.Value).OneWay().Build();
  ```

- OneWayToSource

  目标到源

  ```c#
  textField.Bind(data).From(o => o.Value).OneWayToSource().Build();
  ```

- TwoWay

  双向模式，源到目标和目标到源

```c#
textField.Bind(data).From(o => o.Value).TwoWay().Build();
```



### 源到目标通知

`EnableSourceToTargetNotify` 开启，`DisableSourceToTargetNotify` 关闭

```c#
textField.Bind(data).From(o => o.Value).EnableSourceToTargetNotify().Build();
```



### 定制通知

- **TargetNotify**

  定制目标通知

- **SourceNotify**

  定制源属性通知，事件类型为 `PropertyChangedEventHandler`

**绑定静态属性**

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

//获取 BindingBuilder 绑定生成器
textField.Bind()
    //设置绑定源属性
    .From(() => StaticProperty)
    //定制源属性通知
    .SourceNotify((handler, b) =>
                  {
                      if (b)
                          StaticPropertyChanged += handler;
                      else
                          StaticPropertyChanged -= handler;
                  })
    //生成 binding 对象
    .Build();


//执行绑定
root.BindAll();
```



## 定制访问器

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

**Accessor 提供的访问器**

- 属性或字段

```c#
IAccessor Member(MemberInfo propertyOrField)
IMemberAccessor<TValue> Member<TValue>(Expression<Func<TValue>> propertySelector)
```

支持 `lambda` 属性选择器

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

