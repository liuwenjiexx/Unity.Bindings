using UnityEngine;
using System.Collections;
using System;
using LWJ;
using LWJ.Data;
using UnityEngine.UI; 
using LWJ.Unity;
using System.ComponentModel;

public class TestDataContext1 : MonoBehaviour
{
    public InputField inputField;
    TestData data;
    // Use this for initialization
    void Start()
    {
        var dataContext = gameObject.AddComponent<BindingDataContext>();
        data = new TestData();
 
        if (data.Texture2DImage == null)
            Debug.LogError("Texture2DImage null");
        if (data.SpriteImage == null)
            Debug.LogError("SpriteImage null");
        dataContext.DataContext = data;

        if (inputField)
            inputField.onValueChanged.AddListener(OnTextChange);
        UnityEngine.UI.Text text=null;
        var val1 = text.text;
        UnityEngine.UI.Slider slider=null;
        var val2 = slider.value;
        //slider.onValueChanged;
    }
     

    public void OnTextChange(string value)
    {
        if (string.IsNullOrEmpty(value))
            data.Text = null;
        else
            data.Text = value;
    }


    private class TestData : INotifyPropertyChanged
    {
        private string text;

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                if (text != value)
                {
                    text = value;
                    PropertyChanged.Invoke(this, "Text");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Texture2D Texture2DImage = Resources.Load<Texture2D>("icon");
        public Sprite SpriteImage = Resources.Load<Sprite>("icon");

    }

 

}
 