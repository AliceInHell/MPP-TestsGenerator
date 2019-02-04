using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLibrary
{
    /// <summary>
    /// <see cref="TestsGenerator"/> class configuration.
    /// </summary>
    public class TestGeneratorConfig
    {
        /// <summary>
        /// Max processing tasks count.
        /// </summary>
        public int MaxProcessingTasksCount { get; }

        /// <summary>
        /// Max writting tasks count.
        /// </summary>
        public int MaxWritingTasksCount { get; }

        /// <summary>
        /// Initializes a new instance of a <see cref="TestGeneratorConfig"/> class.
        /// </summary>
        /// <param name="maxProcessingTasksCount"></param>
        /// <param name="maxWritingTasksCount"></param>
        public TestGeneratorConfig(int maxProcessingTasksCount, int maxWritingTasksCount)
        {
            MaxProcessingTasksCount = maxProcessingTasksCount;
            MaxWritingTasksCount = maxWritingTasksCount;
        }
    }
}
