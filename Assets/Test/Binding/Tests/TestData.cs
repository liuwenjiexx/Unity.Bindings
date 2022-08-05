using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace Yanmonet.Bindings.Tests
//{
class Class1
{
    public Struct1 struct1;

    public int Int32 { get; set; }

    public int int32;

}
struct Struct1
{
    public Struct2 struct2;
    public int int32;
}
struct Struct2
{
    public int int32;
}
//}
