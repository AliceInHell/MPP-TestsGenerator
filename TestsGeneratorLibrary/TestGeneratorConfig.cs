using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary
{
    public class TestGeneratorConfig
    {
        public int MaxProcessingTasksCount { get; }
        public int MaxWritingTasksCount { get; }

        public TestGeneratorConfig(int maxProcessingTasksCount, int maxWritingTasksCount)
        {
            MaxProcessingTasksCount = maxProcessingTasksCount;
            MaxWritingTasksCount = maxWritingTasksCount;
        }
    }
}
