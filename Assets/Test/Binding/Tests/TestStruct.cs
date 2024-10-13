using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Bindings;
using UnityEngine;
//namespace Yanmonet.Bindings.Tests
//{

public class TestStruct
{
    public int Int32 { get; set; }

    public int int32;
    [Test]
    public void Struct_Set()
    {
        Struct1 s = new Struct1();
        s.int32 = 1;
        var accessor = Accessor.Member(BindingUtility.GetMember<Struct1, int>(o => o.int32));
        Assert.AreEqual(1, accessor.GetValue(s));
        s = (Struct1)accessor.SetValue(s, 2);
        Assert.AreEqual(2, s.int32);
    }

    [Test]
    public void Path_Sourct_To_Target()
    {
        Class1 source = new Class1();
        Class1 target = new Class1();

        BindingSet<Class1> bindingSet = new BindingSet<Class1>(source);
        bindingSet.Bind(target, "struct1.struct2.int32", "struct1.struct2.int32");
        bindingSet.Bind();

        source.struct1.struct2.int32 = 1;
        bindingSet.Update();
        Assert.AreEqual(1, source.struct1.struct2.int32);
        Assert.AreEqual(1, target.struct1.struct2.int32);
    }

    [Test]
    public void Path_Target_To_Source()
    {
        Class1 source = new Class1();
        Class1 target = new Class1();

        BindingSet<Class1> bindingSet = new BindingSet<Class1>(source);
        bindingSet.Bind(target, "struct1.struct2.int32", "struct1.struct2.int32", BindingMode.TwoWay);
        bindingSet.Bind();

        source.struct1.struct2.int32 = 1;
        bindingSet.Update();
        Assert.AreEqual(1, source.struct1.struct2.int32);
        Assert.AreEqual(1, target.struct1.struct2.int32);

        target.struct1.struct2.int32 = 2;
        bindingSet.UpdateTargetToSource();
        Assert.AreEqual(2, source.struct1.struct2.int32);
        Assert.AreEqual(2, target.struct1.struct2.int32);
    }


}


//}
