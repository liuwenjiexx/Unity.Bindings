using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LWJ.Data
{
    [DefaultMember("DataContext")]
    public interface IDataContext 
    {
        object DataContext { get; }
    }


}
