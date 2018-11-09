using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary.Structures
{
    public class GeneratedTestClass
    {
        public string TestClassName { get; }
        public string TestClassData { get; }

        public GeneratedTestClass(string testClassName, string testClassData)
        {
            TestClassName = testClassName;
            TestClassData = testClassData;
        }
    }
}
