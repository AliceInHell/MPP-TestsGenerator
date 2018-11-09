using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsGeneratorLibrary;

namespace TestsGeneratorUnitTests
{
    [TestClass]
    public class TestsGeneratorTestsClass
    {
        private int readingTasksCount;
        private int writingTasksCount;

        private TestGeneratorConfig config;

        private string outputDirectory;
        private List<string> paths;

        private ParallelCodeReader reader;
        private CodeWriter writer;

        private TestsGenerator generator;

        [TestInitialize]
        public void SetUp()
        {
            readingTasksCount = 3;
            writingTasksCount = 3;
            config = new TestGeneratorConfig(readingTasksCount, writingTasksCount);

            outputDirectory = @"./";

            paths = new List<string>();
            paths.Add(@"./someClass.cs");

            reader = new ParallelCodeReader(paths, readingTasksCount);
            writer = new CodeWriter(outputDirectory);

            generator = new TestsGenerator(config);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
