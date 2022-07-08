using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yanmonet.Bindings
{

    /// <summary>
    /// 访问器
    /// </summary>
    public interface IAccessor
    {
        /// <summary>
        /// 能否获取值
        /// </summary>
        bool CanGetValue(object target);

        /// <summary>
        /// 能否设置值
        /// </summary>
        bool CanSetValue(object target);

        /// <summary>
        /// 获取值
        /// </summary>
        object GetValue(object target);

        /// <summary>
        /// 设置值
        /// </summary>
        void SetValue(object target, object value);
    }

    /// <summary>
    /// 访问器
    /// </summary>
    public interface IAccessor<TValue> : IAccessor
    {
        new TValue GetValue(object target);

        void SetValue(object target, TValue value);
    }

}
