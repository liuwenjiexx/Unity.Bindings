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
        var entry = new Binding.Entry();
        entry.bindingType = Binding.BindingType.MultiBinding;
        entry.target = target;
        entry.targetPath = "text";
        entry.converter = "StringFormat";
        entry.converterParameter = "{0} {1}!";

        entry.AddBinding(new Binding.Entry()
        {
            source = source,
            path = "text",
        }).AddBinding(new Binding.Entry()
        {
            source = source2,
            path = "text",
        });

        binding.AddBinding(entry);
        binding.Bind();

 
    }
    // Update is called once per frame
    void Update()
    {

    }
}
