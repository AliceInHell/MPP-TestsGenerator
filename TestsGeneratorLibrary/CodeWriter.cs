using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestsGeneratorLibrary.Structures;

namespace TestsGeneratorLibrary
{
    /// <summary>
    /// Async code wtirer.
    /// </summary>
    public class CodeWriter
    {
        /// <summary>
        /// Output directiory.
        /// </summary>
        private readonly string _outputDirectoryPath;

        /// <summary>
        /// Initializes a new instance of a <see cref="CodeWriter"/> class.
        /// </summary>
        /// <param name="outputDirectoryPath"></param>
        public CodeWriter(string outputDirectoryPath)
        {
            _outputDirectoryPath = outputDirectoryPath;

            if (!Directory.Exists(_outputDirectoryPath))
            {
                Directory.CreateDirectory(_outputDirectoryPath);
            }
        }

        /// <summary>
        /// Write to file async.
        /// </summary>
        /// <param name="generatedCode">Code</param>
        /// <returns><see cref="Task"/></returns>
        public async Task WriteAsync(GeneratedTestClass generatedCode)
        {
            string filePath = $"{_outputDirectoryPath}\\{generatedCode.TestClassName}";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                await sw.WriteAsync(generatedCode.TestClassData);
            }            
        }
    }
}
