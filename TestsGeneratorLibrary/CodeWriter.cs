using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestsGeneratorLibrary.Structures;

namespace TestsGeneratorLibrary
{
    public class CodeWriter
    {
        private readonly string _outputDirectoryPath;

        public CodeWriter(string outputDirectoryPath)
        {
            _outputDirectoryPath = outputDirectoryPath;

            if (!Directory.Exists(_outputDirectoryPath))
            {
                Directory.CreateDirectory(_outputDirectoryPath);
            }
        }

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
