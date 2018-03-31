using LWJ.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSetMultiBinding : MonoBehaviour
{
    public Text source;
    public Text source2;
    public Text target;

    // Use this for initialization
    void Start()
    {
        TestBinding1();
    }
    void TestBinding1()
    {
        var binding = target.gameObject.AddComponent<Binding>();
        var entry = new Binding.BindingEntry();

        entry.target = target;
        entry.targetPath = "text";
        entry.converter = "StringFormat";
        entry.converterParameter = "{0} {1}!";
        entry.AddBinding(new Binding.BindingEntry()
        {
            source = source,
            path = "text",
        }).AddBinding(new Binding.BindingEntry()
        {
            source = source2,
            path = "text",
        });

        binding.AddBinding(Binding.BindingType.MultiBinding, entry);
        binding.Bind();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
