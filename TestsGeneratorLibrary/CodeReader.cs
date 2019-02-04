using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary
{
    /// <summary>
    /// Async code reader
    /// </summary>
    public class CodeReader
    {
        /// <summary>
        /// Read file async
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <returns><see cref="Task{string}"/></returns>
        public async Task<string> ReadAsync(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return await sr.ReadToEndAsync();
            }      
        }
    }
}
