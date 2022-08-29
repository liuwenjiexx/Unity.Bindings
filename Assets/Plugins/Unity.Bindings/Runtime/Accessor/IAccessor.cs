using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Yanmonet
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
        /// <returns>return struct</returns>
        object SetValue(object target, object value);
         
    }

    /// <summary>
    /// 访问器
    /// </summary>
    public interface IAccessor<TValue> : IAccessor
    {
        new TValue GetValue(object target);

        object SetValue(object target, TValue value);

    }



}
