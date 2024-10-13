using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Unity.Bindings;

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