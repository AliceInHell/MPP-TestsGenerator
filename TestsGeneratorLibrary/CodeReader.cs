using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary
{
    public class CodeReader
    {
        public async Task<string> ReadAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return await sr.ReadToEndAsync();
            }      
        }
    }
}
