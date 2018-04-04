using UnityEngine;
using System.Collections;
using LWJ.Data;
using System;
namespace LWJ.Unity
{
    [Converter("Sprite"), Converter("Texture2D")]
    public class Texture2DConverter : IValueConverter
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
                if (targetType == typeof(Texture2D) || targetType == typeof(Texture))
                    return SpriteToTexture(sprite);
            }
            throw new InvalidCastException(value.GetType() + " > " + targetType);
        }


        public object ConvertBack(object value, Type targetType, object parameter)
        {
            return Convert(value, targetType, parameter);
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


    }

}