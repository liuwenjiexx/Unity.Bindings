# Data binding

数据绑定



## 绑定

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





## 数据源

为了更好的性能，实现 `INotifyPropertyChanged` 属性值改变通知接口

```c#
public class MyData : INotifyPropertyChanged
{
    public string Text { 
        get => text; 
        set => SetProperty(nameof(Text), ref text, value); 
    }
    
    protected void SetProperty<T>(string propertyName, ref T field, T newValue)
    {
        if (!object.Equals(field, newValue))
        {
            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

