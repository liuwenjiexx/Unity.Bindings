using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    public class BindingOptions
    {
        /// <summary>
        /// 绑定模式
        /// </summary>
        public BindingMode? Mode { get; set; }

        public SetPropertyChangedDelegate TargetPropertyChanged { get; set; }

        public bool? TargetNotifyValueChangedEnabled { get; set; }

        /// <summary>
        /// 源到目标不触发事件，如：<see cref="ChangeEvent{T}"/>
        /// </summary>
        public bool? SourceToTargetWithoutNotify { get; set; }

        public SetPropertyChangedDelegate SourcePropertyChanged { get; set; }



    }
}