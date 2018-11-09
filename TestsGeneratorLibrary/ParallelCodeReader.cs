using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary
{
    public class ParallelCodeReader
    {
        private readonly IEnumerable<string> _filePaths;
        private readonly ParallelOptions _maxReadingTasksCount;

        public ParallelCodeReader(IEnumerable<string> filePaths, int maxReadingTasksCount)
        {
            _filePaths = filePaths;

            _maxReadingTasksCount = new ParallelOptions { MaxDegreeOfParallelism = maxReadingTasksCount };
        }

        public int Count
        {
            get
            {
                return _filePaths.Count();
            }
        }

        public IEnumerable<string> Provide()
        {
            List<string> codeBuffer = new List<string>();
            List<Exception> exceptions = new List<Exception>();

            Parallel.ForEach(
                _filePaths, 
                _maxReadingTasksCount, 
                filePath =>
                {
                    try
                    {
                        codeBuffer.Add(File.ReadAllText(filePath));
                    }
                    catch (Exception exception)
                    {
                        exceptions.Add(exception);
                    }
                });

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            return codeBuffer;
        }
    }
}
