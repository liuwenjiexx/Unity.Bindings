using UnityEngine;
using System.Collections;
using LWJ.Data;
using System.ComponentModel;

namespace LWJ.Unity
{


    public class BindingDataContext : MonoBehaviour, IDataContext, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //public event DisposedEventHandler Disposed;

        private object data;

        [SerializeField]
        private Object unityObject;

        public object DataContext
        {
            get
            {

                return data;
            }

            set
            {
                if (data != value)
                {
                    data = value;
                    PropertyChanged.Invoke(this, "DataContext");
                }
            }
        }

        void Start()
        {
            enabled = false;
        }

        void Update()
        {
            enabled = false;
        }

        public Object UnityObject
        {
            get
            {
                return unityObject;
            }

            set
            {
                unityObject = value;
            }
        }

        void OnDestroy()
        {

            DataContext = null;

        }

    }


}