using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TestsGeneratorLibrary.Structures;

namespace TestsGeneratorLibrary
{
    public class ParallelCodeWriter
    {
        private readonly string _outputDirectoryPath;

        public ParallelCodeWriter(string outputDirectoryPath)
        {
            _outputDirectoryPath = outputDirectoryPath;

            if (!Directory.Exists(_outputDirectoryPath))
            {
                Directory.CreateDirectory(_outputDirectoryPath);
            }
        }

        public void Consume(GeneratedTestClass @class)
        {
            string filePath = filePath = $"{_outputDirectoryPath}\\{@class.TestClassName}";

            Exception error = null;
            try
            {
                File.WriteAllText(filePath, @class.TestClassData);
            }
            catch (Exception exception)
            {
                error = exception;
            }
        }
    }
}
