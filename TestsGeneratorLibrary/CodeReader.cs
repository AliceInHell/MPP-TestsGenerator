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
            StreamReader sr = new StreamReader(path); 
            string result = await sr.ReadToEndAsync();

            sr.Close();
            return result;         
        }
    }
}
