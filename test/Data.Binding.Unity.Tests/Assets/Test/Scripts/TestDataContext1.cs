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

public class TextureConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter)
    {
        if (value == null)
            return null;

        if (targetType.IsAssignableFrom(value.GetType()))
            return value;


        if (value is Texture2D)
        {
            Texture2D texture = (Texture2D)value;
            if (targetType == typeof(Sprite))
                return TextureToSprite(texture);

        }
        else if (value is Sprite)
        {
            Sprite sprite = (Sprite)value;
            if (targetType == typeof(Texture2D))
                return SpriteToTexture(sprite);
        }
        return null;
    }



    Sprite TextureToSprite(Texture2D texture)
    {
        if (texture == null)
            return null;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
    Texture2D SpriteToTexture(Sprite sprite)
    {
        if (sprite == null)
            return null;
        return sprite.texture;
    }

    public object ConvertBack(object value, Type targetType, object parameter)
    {
        return Convert(value, targetType, parameter);
    }
}