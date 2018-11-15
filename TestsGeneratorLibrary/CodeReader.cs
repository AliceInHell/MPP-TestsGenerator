using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary
{
    public class CodeReader
    {
        public string ProvideCode(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
