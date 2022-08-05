using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yanmonet.Bindings;
//namespace Yanmonet.Bindings.Tests
//{
public class TestPropertyBinder
{

    [Test]
    public void Field_Int32111()
    {
        Struct2 struct2 = new Struct2();
        object obj = struct2;
        Struct2 s2 = (Struct2)obj;
        s2.int32 = 1;
        Struct2 s3 = (Struct2)obj;

        Debug.Log(s2.int32 + ", " + s3.int32 + ", " + struct2.int32);
    }
     
    [Test]
    public void Field_Int32()
    {
        Class1 source = new Class1();
        source.int32 = 1;
        PropertyBinder binder = PropertyBinder.Create("int32");
        binder.Target = source;

        object value;
        Assert.IsTrue(binder.TryGetTargetValue(out value));
        Assert.AreEqual(1, value);

        Assert.IsTrue(binder.CanSetValue);
        Assert.IsTrue(binder.TrySetTargetValue(2));
        Assert.AreEqual(2, source.int32);
    }

    [Test]
    public void Property_Int32()
    {
        Class1 source = new Class1();
        source.Int32 = 1;
        PropertyBinder binder = PropertyBinder.Create("Int32");
        binder.Target = source;

        object value;
        Assert.IsTrue(binder.TryGetTargetValue(out value));
        Assert.AreEqual(1, value);


        Assert.IsTrue(binder.CanSetValue);
        Assert.IsTrue(binder.TrySetTargetValue(2));
        Assert.AreEqual(2, source.Int32);
    }

    [Test]
    public void Struct_Path()
    {
        Class1 source = new Class1();
        source.struct1.struct2.int32 = 1;

        PropertyBinder binder = PropertyBinder.Create("struct1.struct2.int32");
        binder.Target = source;

        object value;

        Assert.IsTrue(binder.TryGetTargetValue(out value));
        Assert.AreEqual(1, value);

        Assert.IsTrue(binder.CanSetValue);
        Assert.IsTrue(binder.TrySetTargetValue(2));
        Assert.AreEqual(2, source.struct1.struct2.int32);

    }

}
//}