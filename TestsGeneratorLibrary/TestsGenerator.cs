using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestsGeneratorLibrary.Structures;

namespace TestsGeneratorLibrary
{
    public class TestsGenerator
    {
        private readonly TestGeneratorConfig _testsGeneratorConfig;        

        public TestsGenerator(TestGeneratorConfig testsGeneratorConfig)
        {
            _testsGeneratorConfig = testsGeneratorConfig;
        }

        public async Task Generate(CodeReader reader, CodeWriter writer, List<string> source)
        {

            DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            ExecutionDataflowBlockOptions processingTaskRestriction = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _testsGeneratorConfig.MaxProcessingTasksCount };
            ExecutionDataflowBlockOptions outputTaskRestriction = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _testsGeneratorConfig.MaxWritingTasksCount };

            TransformBlock<string, string> readingBlock =
                new TransformBlock<string, string>(new Func<string, Task<string>>(reader.ReadAsync), processingTaskRestriction);

            TransformBlock<string, GeneratedTestClass> producingBlock =
                new TransformBlock<string, GeneratedTestClass>(new Func<string, GeneratedTestClass>(Produce), processingTaskRestriction);

            ActionBlock<GeneratedTestClass> writingBlock = new ActionBlock<GeneratedTestClass>(
               ((generatedClass) => writer.WriteAsync(generatedClass).Wait()), outputTaskRestriction);

            readingBlock.LinkTo(producingBlock, linkOptions);
            producingBlock.LinkTo(writingBlock, linkOptions);

            foreach (string path in source)
            {
               await readingBlock.SendAsync(path);
            }

            readingBlock.Complete();

            await writingBlock.Completion;
        }

        private GeneratedTestClass Produce(string sourceCode)
        {
            SyntaxTreeInfoBuilder syntaxTreeInfoBuilder = new SyntaxTreeInfoBuilder(sourceCode);
            SyntaxTreeInfo syntaxTreeInfo = syntaxTreeInfoBuilder.GetSyntaxTreeInfo();

            TestClassTemplateGenerator testTemplatesGenerator = new TestClassTemplateGenerator(syntaxTreeInfo);
            List<GeneratedTestClass> testTemplates = testTemplatesGenerator.GetTestTemplates().ToList();
           
            return new GeneratedTestClass(testTemplates.First().TestClassName, testTemplates.First().TestClassData);
        }
    }
}
