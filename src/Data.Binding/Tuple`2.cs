/**************************************************************
 *  Filename:    Tuple`2.cs
 *  Copyright:  © 2017 WenJie Liu. All rights reserved.
 *  Description: LWJ ClassFile
 *  @author:     WenJie Liu
 *  @version     2017/2/27
 **************************************************************/
using System.Collections;
using System.Collections.Generic;

namespace LWJ
{
    public class Tuple<T1, T2> : IEqualityComparer, IEqualityComparer<Tuple<T1, T2>>
    {
        private T1 item1;
        private T2 item2;


        public Tuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public T1 Item1
        {
            get { return item1; }
        }

        public T2 Item2
        {
            get { return item2; }
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;

            var a = obj as Tuple<T1, T2>;

            if (a == null)
                return false;

            return object.Equals(item1, a.item1) && object.Equals(item2, a.item2);
        }

        public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            if (object.ReferenceEquals(a, b))
                return true;

            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;


            return a.Equals(b);
        }
        public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
        {
            return !(a == b);
        }
        public override string ToString()
        {
            return string.Format("({0},{1})", item1, item2);
        }
        internal static int CombineHashCodes2(int h1, int h2)
        {
            return h1 * 31 + h2;
        }



        public override int GetHashCode()
        {
            int hashCode ;

            hashCode = item1 == null ? 0 : item1.GetHashCode();

            hashCode = CombineHashCodes2((item2 == null ? 0 : item2.GetHashCode()), hashCode); 

            return hashCode;
        }

        public new bool Equals(object x, object y)
        {
            if (object.ReferenceEquals(x, y))
                return true;
            var x1 = x as Tuple<T1, T2>;
            if (x1 == null)
                return false;
            return x1.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            var o = obj as Tuple<T1, T2>;
            if (o == null)
                return 0;
            return o.GetHashCode();
        }

        public bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y)
        {
            return x == y;
        }

        public int GetHashCode(Tuple<T1, T2> obj)
        {
            if (obj == null)
                return 0;
            return obj.GetHashCode();
        }
    }


}