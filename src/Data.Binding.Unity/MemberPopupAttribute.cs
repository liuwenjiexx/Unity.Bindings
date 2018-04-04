using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LWJ.Unity
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MemberPopupAttribute : PropertyAttribute
    {
        public MemberPopupAttribute(string targetMember, MemberPopupFlags memberFlags)
        {
            this.TargetMember = targetMember;
            this.MemberFlags = memberFlags;
        }
        public MemberPopupAttribute(Type targetType, MemberPopupFlags memberFlags)
        {
            this.TargetType = targetType;
            this.MemberFlags = memberFlags;
        }

        public Type TargetType { get; set; }

        public string TargetMember { get; set; }

        public MemberPopupFlags MemberFlags { get; set; }
    }

    [Flags]
    public enum MemberPopupFlags
    {
        Field = 1,
        Property = 2,
        Method = 4,
    }

}
