using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yanmonet.Bindings
{
    class ButtonEnabledSelf : MemberAccessor<Button, bool>
    {
        public ButtonEnabledSelf()
            : base(BindingUtility.GetMember<Button, bool>(o => o.enabledSelf), o => o.enabledSelf, (o, v) => o.SetEnabled(v)) { }
    }

}