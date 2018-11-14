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

        public Task Generate(ParallelCodeReader reader, CodeWriter writer)
        {
            return new Task(
                () =>
                {
                    DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
                    ExecutionDataflowBlockOptions processingTaskRestriction = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _testsGeneratorConfig.MaxProcessingTasksCount };
                    ExecutionDataflowBlockOptions outputTaskRestriction = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _testsGeneratorConfig.MaxWritingTasksCount };

                    TransformBlock<string, GeneratedTestClass> producerBuffer =
                        new TransformBlock<string, GeneratedTestClass>(new Func<string, GeneratedTestClass>(Produce), processingTaskRestriction);

                    ActionBlock<GeneratedTestClass> resultWritingAction = new ActionBlock<GeneratedTestClass>(
                       ((generatedClass) => writer.Consume(generatedClass)), outputTaskRestriction);

                    producerBuffer.LinkTo(resultWritingAction, linkOptions);

                    Parallel.ForEach(reader.Provide(), async generatedClass =>
                    {
                        await producerBuffer.SendAsync(generatedClass);
                    });

                    //producerBuffer.Complete();
                    //resultWritingAction.Completion.Wait();

                    // for execution sequence check !!!
                    //Thread.Sleep(1000);
                    Console.WriteLine("Hello from Task!");
                }, TaskCreationOptions.AttachedToParent);
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
