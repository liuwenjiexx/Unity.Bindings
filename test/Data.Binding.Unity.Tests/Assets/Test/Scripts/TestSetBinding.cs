using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LWJ.Unity;
using System;
public class TestSetBinding : MonoBehaviour
{

    public Text source;
    public Text target;
    public TestData1 testData;
    // Use this for initialization
    void Start()
    {
      
        TestBinding();
    }

    void TestBinding()
    {
        var binding = target.gameObject.AddComponent<Binding>();
        var entry = new Binding.BindingEntry();
        entry.source = source;
        entry.path = "text";
        entry.target = target;
        entry.targetPath = "text";

        binding.AddBinding(Binding.BindingType.Binding, entry);
        binding.Bind();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

[Serializable]
public class TestData1
{
    public int n;
    [SerializeField]
    public TestData1[] arr1;
    [SerializeField]
    public TestData2[] arr2;
}
[Serializable]
public class TestData2 : TestData1
{

}


