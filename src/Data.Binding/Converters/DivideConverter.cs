using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.Data
{

    [Named("Divide")]
    public class DivideConverter : IMultiValueConverter
    {


        public object Convert(object[] values, Type targetType, object parameter)
        {
            if (values.Length < 1)
                throw new Exception("div values length < 2");
            //if (targetType == null)
            //    targetType = typeof(float); 
            TypeCode typeCode = Type.GetTypeCode(targetType);
            switch (typeCode)
            {
                case TypeCode.Single:
                    {
                        float[] vals = new float[values.Length];
                        float result = 0;
                        for (int i = 0; i < values.Length; i++)
                        {
                            float val;
                            val = (float)System.Convert.ChangeType(values[i], typeof(float));
                            if (i == 0)
                            {
                                result = val;
                            }
                            else
                            {
                                result /= val;
                            }
                        }
                        return result;
                    }
                    break;
                case TypeCode.Double:
                    {

                        double[] vals = new double[values.Length];
                        double result = 0d;
                        for (int i = 0; i < values.Length; i++)
                        {
                            double val;
                            val = (double)System.Convert.ChangeType(values[i], typeof(double));
                            if (i == 0)
                            {
                                result = val;
                            }
                            else
                            {
                                result /= val;
                            }
                        }
                        return result;
                    }
                    break;
                case TypeCode.Int32:
                    {

                        int[] vals = new int[values.Length];
                        int result = 0;
                        for (int i = 0; i < values.Length; i++)
                        {
                            int val;
                            val = (int)System.Convert.ChangeType(values[i], typeof(int));
                            if (i == 0)
                            {
                                result = val;
                            }
                            else
                            {
                                result /= val;
                            }
                        }
                        return result;
                    }
                    break;
            }
            throw new Exception("not implement type:" + targetType);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
