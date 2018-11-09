using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestsGeneratorLibrary;

namespace ConsoleTestsGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            int readingTasksCount = Convert.ToInt32(args[0]);
            int writingTasksCount = Convert.ToInt32(args[1]);

            TestGeneratorConfig config = new TestGeneratorConfig(readingTasksCount, writingTasksCount);

            string outputDirectory = args[2];
            List<string> paths = new List<string>();

            for (int i = 3; i < args.Length; i++)
            {
                paths.Add(args[i]);
            }

            ParallelCodeReader reader = new ParallelCodeReader(paths, config.MaxProcessingTasksCount);
            CodeWriter writer = new CodeWriter(outputDirectory);

            TestsGenerator generator = new TestsGenerator(config);
            generator.Generate(reader, writer);

            Console.ReadLine();
        }
    }
}
