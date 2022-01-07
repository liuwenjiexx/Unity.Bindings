using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.Bindings
{

    /// <summary>
    /// 绑定模式
    /// </summary>
    public enum BindingMode
    {

        /// <summary>
        /// default mode, source to target
        /// </summary>
        OneWay,
        /// <summary>
        /// target to source
        /// </summary>
        OneWayToSource,
        /// <summary>
        /// source to target, target to source
        /// </summary>
        TwoWay,
    }

}
