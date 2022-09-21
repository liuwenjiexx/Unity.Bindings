using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using YMFramework;

namespace Yanmonet.Bindings
{
    public class IndexerAccessor<TTarget, TIndex, TValue> : IAccessor
    {
        private TIndex index;
        private Func<TTarget, TIndex, TValue> getter;
        private Action<TTarget, TIndex, TValue> setter;
        private Type valueType;

        public IndexerAccessor(PropertyInfo property)
        {
            ParameterExpression targetExpr = null;
            ParameterExpression indexExpr = null;
            Type targetType = null, indexType;

            indexExpr = Expression.Parameter(typeof(TIndex));

            targetType = property.DeclaringType;
            indexType = typeof(TIndex);
            valueType = property.PropertyType;
            targetExpr = Expression.Parameter(targetType);

            var indexPropertyExpr = Expression.MakeIndex(targetExpr, property, new Expression[] { indexExpr });

            getter = (Func<TTarget, TIndex, TValue>)Expression.Lambda(typeof(Func<,,>).MakeGenericType(targetType, indexType, valueType),
                indexPropertyExpr, targetExpr, indexExpr)
                .Compile();

            if (property.CanWrite)
            {
                var valueExpr = Expression.Parameter(valueType); ;
                var setterBody = Expression.Assign(indexPropertyExpr, valueExpr);
                setter = (Action<TTarget, TIndex, TValue>)Expression.Lambda(typeof(Action<,,>).MakeGenericType(targetType, indexType, valueType),
                    setterBody, targetExpr, indexExpr, valueExpr)
                    .Compile();
            }
        }
        
        public Type ValueType => valueType;

        public bool CanGetValue(object target)
        {
            return getter != null;
        }

        public bool CanSetValue(object target)
        {
            return setter != null;
        }

        public object GetValue(object target)
        {
            if (getter == null)
                throw new MemberAccessException();
            return getter((TTarget)target, index);
        }

        public object SetValue(object target, object value)
        {
            if (setter == null)
                throw new MemberAccessException();
            setter((TTarget)target, index, (TValue)value);
            return target;
        }
    }
}