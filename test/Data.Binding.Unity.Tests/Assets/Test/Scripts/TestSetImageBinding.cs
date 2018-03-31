using LWJ.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSetImageBinding : MonoBehaviour
{

    public Sprite sprite;
    public Texture2D texture2d;

    public Image spriteTarget;
    public Image spriteTarget2;
    public Image spriteTarget3;


    // Use this for initialization
    void Start()
    {


        var binding = spriteTarget.gameObject.AddComponent<Binding>();
        var entry = new Binding.BindingEntry();
        entry.source = sprite;
        entry.path = ".";
        entry.target = spriteTarget;
        entry.targetPath = "sprite";

        binding.AddBinding(Binding.BindingType.Binding, entry);
        binding.Bind();


        binding = spriteTarget2.gameObject.AddComponent<Binding>();
        entry = new Binding.BindingEntry();
        entry.source = texture2d;
        entry.path = ".";
        entry.target = spriteTarget2;
        entry.targetPath = "sprite";
        entry.converter = "Texture2D";
        binding.AddBinding(Binding.BindingType.Binding, entry);
        binding.Bind();



        binding = spriteTarget3.gameObject.AddComponent<Binding>();
        entry = new Binding.BindingEntry();
        entry.source = texture2d;
        entry.path = ".";
        entry.target = spriteTarget3;
        entry.targetPath = "sprite";
        entry.converter = typeof(Texture2DConverter).FullName;        
      
        binding.AddBinding(Binding.BindingType.Binding, entry);
        binding.Bind();

    }

 

    // Update is called once per frame
    void Update()
    {

    }
}
