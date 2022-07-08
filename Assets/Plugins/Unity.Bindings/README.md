# Data binding

数据绑定

## 绑定

### 路径

**绑定**

```c#
textField.BindPath<string>(data, nameof(PropertyName));
textField.BindPath<string>(data, "PropertyName");
textField.BindPath<string>(data, "PropertyName1.PropertyName2");
```

对象分隔符 `.`，`data` 数据实现 `INotifyPropertyChanged` 接口提高性能

### 属性

对任意属性进行绑定

#### 目标属性

绑定 `Label.text` 属性

```c#
label.BindProperty<Label, TestData, string>(targetPropertySelector: o => o.text,
                                            source: data, 
                                            propertySelector: o => o.Value);
```

- **targetPropertySelector**

  目标属性选择器，`lambda` 表达式格式，选择 `Label.text` 属性

- **propertySelector**

  源属性选择器，选择 `TestData.Value` 属性

#### 静态属性

```c#
static string staticProperty;
static string StaticProperty
{
    get => staticProperty;
    set => StaticPropertyChanged.Invoke(null, nameof(StaticProperty), ref staticProperty, value);
}

static event PropertyChangedEventHandler StaticPropertyChanged;

var options = new BindingOptions()
{
    SourcePropertyChanged = (handler, b) =>
    {
        if (b)
            StaticPropertyChanged += handler;
        else
            StaticPropertyChanged -= handler;
    }
};
textField.BindProperty(propertySelector: () => StaticProperty, options);
```

- **propertySelector**

  源属性选择器，选择了 `StaticProperty` 静态属性

- **SourcePropertyChanged**

  定制源属性通知，事件类型为 `PropertyChangedEventHandler`

## 解绑

**绑定**

```c#
binding.Bind();
```

扩展方法 `BindPath`，`BindProperty` 立即调用 `Bind` 方法，不用手动调用

**解绑**

```c#
binding.Unbind();
```

` binding.IsBinding` 判断是否绑定

**样例**

```c#
var binding = textField.BindPath<string>(data, "Value");
binding.Unbind();
```



### VisualElement

**绑定**

```c#
root.BindAll();
```

所有子节点调用 `Bind` 方法

**解绑**

```c#
root.UnbindAll();
```

所有子节点调用 `Unbind` 方法



**绑定方法**

```c#
IDisposable Bind<TValue>(this VisualElement target, object source, string path)
```

**样例**

支持实现了 `IBindable` 接口的目标对象，同时实现  `INotifyValueChanged` 支持绑定 `value` 属性

如：TextField，FloatField，IntegerField，EnumField 等， [支持 IBindable 默认绑定完整列表](https://docs.unity3d.com/Manual/UIE-Binding.html)

```
textField.Bind<string>(data, "Text");
```



**绑定方法**

```c#
IDisposable Bind<TValue>(this VisualElement target, INotifyValueChanged<TValue> targetAccessor, object source, string path)
```

**样例**

`Label` 没有实现 `INotifyValueChanged` 需要实现 `targetAccessor` 绑定目标属性访问器

```c#
label.Bind(new Accessor<string>(() => label.text, (v) => label.text = v;), data, "Text");
```



### 属性访问器 Accessor

**绑定方法**

```c#
IDisposable Bind<TValue>(this VisualElement target, object source, string propertyName, INotifyValueChanged<TValue> accessor)
```

数据源或目标对象未实现 `INotifyPropertyChanged` 时，需要每帧检查值实现值绑定

**样例**

```c#
textField.Bind(data, "Text", new Accessor<string>(() => Text, (val) => Text = val));
```



**绑定方法**

```c#
IDisposable Bind<TValue>(this VisualElement target, INotifyValueChanged<TValue> targetAccessor, object source, string propertyName, INotifyValueChanged<TValue> accessor)
```

目标属性和数据源定制属性访问器

**样例**

```c#
label.Bind(new Accessor<string>(() => label.text, (v) => label.text = v), this, "Text", new Accessor<string>(() => Text, (val) => Text = val));
```





