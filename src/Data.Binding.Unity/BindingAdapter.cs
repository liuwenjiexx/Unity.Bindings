using UnityEngine;
using System.Collections;
namespace LWJ.Unity
{

    public class BindingAdapter : MonoBehaviour
    {

        public bool IsActive
        {
            get { return gameObject.activeSelf; }
            set { gameObject.SetActive(value); }
        }

        public float LocalPositionX
        {
            get
            {
                return transform.localPosition.x;
            }
            set
            {
                var pos = transform.localPosition;
                pos.x = value;
                transform.localPosition = pos;
            }
        }
        public float LocalScaleX
        {
            get
            {
                return transform.localScale.x;
            }
            set
            {
                if (float.IsNaN(value))
                    return;
                var pos = transform.localScale;
                pos.x = value;
                transform.localScale = pos;
            }
        }

    }
}