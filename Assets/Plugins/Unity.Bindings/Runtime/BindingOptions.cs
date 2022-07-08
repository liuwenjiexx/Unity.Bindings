using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    public class BindingOptions
    {
        /// <summary>
        /// ��ģʽ
        /// </summary>
        public BindingMode? Mode { get; set; }

        public SetPropertyChangedDelegate TargetPropertyChanged { get; set; }

        public bool? TargetNotifyValueChangedEnabled { get; set; }

        /// <summary>
        /// Դ��Ŀ�겻�����¼����磺<see cref="ChangeEvent{T}"/>
        /// </summary>
        public bool? SourceToTargetWithoutNotify { get; set; }

        public SetPropertyChangedDelegate SourcePropertyChanged { get; set; }



    }
}