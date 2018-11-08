using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary.Structures
{
    public class ClassInfo
    {
        public string NamespaceName { get; }
        public string Name { get; }
        public IEnumerable<MethodInfo> Methods { get; }

        public ClassInfo(string namespaceName, string name, IEnumerable<MethodInfo> methods)
        {
            NamespaceName = namespaceName;
            Name = name;            
            Methods = methods;
        }
    }
}
