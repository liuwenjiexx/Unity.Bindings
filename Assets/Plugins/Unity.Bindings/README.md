# Binding

数据绑定

| 特性                   | 支持 |
| ---------------------- | ---- |
| 路径                   | ✔    |
| 静态源属性             | ✔    |
| 静态目标属性           |      |
| 双向 TwoWay            | ✔    |
| INotifyPropertyChanged | ✔    |
| INotifyValueChanged    | ✔    |
| UIElements             | ✔    |



## 绑定

### 绑定路径

```c#
BindingBase BindPath<TValue>(this object target, object source, string path)
```

- **target**

  目标对象，默认绑定 `INotifyValueChanged.value` 属性

- **source**

  源对象

- **path**

  源对象的属性，多个属性用 `.` 分隔

**样例**

```c#
textField.BindPath<string>(data, nameof(TestData.Value));
textField.BindPath<string>(data, "Value");
textField.BindPath<string>(data, "Data2.Value");
```

`TestData` 为 [样例数据](#样例数据)

**VisualElement**

可以绑定实现了 `IBindable` 接口的 `VisualElement` 对象，同时实现  `INotifyValueChanged` 支持绑定 `value` 属性

如：TextField，FloatField，IntegerField，EnumField 等， [支持 IBindable 默认绑定完整列表](https://docs.unity3d.com/Manual/UIE-Binding.html)

### 目标属性

```c#
BindingBase BindPath<TTarget, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, object source, string path)
```

- **targetPropertySelector**

  目标属性 `lambda` 选择器

**样例**

```c#
label.BindPath<Label, string>(o => o.text, data, "Value");
```



### 源属性

```c#
BindingBase BindProperty<TSource, TValue>(this object target, TSource source, Expression<Func<TSource, TValue>> propertySelector)
```

- **propertySelector**

  源属性 `lambda` 选择器

**样例**

```c#
textField.BindProperty(data, o => o.Value);
```



同时指定目标和源属性

```c#
BindingBase BindProperty<TTarget, TSource, TValue>(this object target, Expression<Func<TTarget, TValue>> targetPropertySelector, TSource source, Expression<Func<TSource, TValue>> propertySelector)
```

**样例**

```c#
label.BindProperty<Label, TestData, string>(o => o.text, data, o => o.Value);
```



## 解绑

```c#
binding.Bind();
binding.Unbind();
```

` binding.IsBinding` 判断是否绑定，使用扩展方法 `BindPath`，`BindProperty` 会立即调用 `Bind` 方法

**样例**

```c#
var binding = textField.BindPath<string>(data, "Value");
binding.Unbind();

//绑定
binding.Bind();
```



### VisualElement

**绑定**

```c#
root.BindAll();
```

所有子节点调用 `binding.Bind` 方法

**解绑**

```c#
root.UnbindAll();
```

所有子节点调用 `binding.Unbind` 方法

## 定制通知

- **TargetNotify**

  定制目标通知

- **SourceNotify**

  定制源属性通知，事件类型为 `PropertyChangedEventHandler`

### 绑定静态属性

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

var options = new BindingOptions()
{
    //定制属性通知
    SourceNotify = (handler, add) =>
    {
        if (add)
            StaticPropertyChanged += handler;
        else
            StaticPropertyChanged -= handler;
    }
};

textField.BindProperty(() => StaticProperty, options);
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
IAccessor<TValue> Member<TValue>(Expression<Func<TValue>> propertySelector)
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

