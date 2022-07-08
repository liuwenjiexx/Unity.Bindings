using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    public class BindingOptions
    {
        /// <summary>
        /// ��ģʽ
        /// </summary>
        public BindingMode? Mode { get; set; }

        public BindingNotifyDelegate TargetNotify { get; set; }

        public bool? TargetNotifyValueChangedEnabled { get; set; }

        /// <summary>
        /// Դ��Ŀ���Ƿ񴥷��¼����磺<see cref="ChangeEvent{T}"/>
        /// </summary>
        public bool? SourceToTargetNotifyEnabled { get; set; }

        public BindingNotifyDelegate SourceNotify { get; set; }



    }
}