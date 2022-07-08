using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    public class BindingOptions
    {
        /// <summary>
        /// 绑定模式
        /// </summary>
        public BindingMode? Mode { get; set; }

        public BindingNotifyDelegate TargetNotify { get; set; }

        public bool? TargetNotifyValueChangedEnabled { get; set; }

        /// <summary>
        /// 源到目标是否触发事件，如：<see cref="ChangeEvent{T}"/>
        /// </summary>
        public bool? SourceToTargetNotifyEnabled { get; set; }

        public BindingNotifyDelegate SourceNotify { get; set; }



    }
}