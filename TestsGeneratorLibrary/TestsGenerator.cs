using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestsGeneratorLibrary.Structures;

namespace TestsGeneratorLibrary
{
    public class TestsGenerator
    {
        private readonly TestGeneratorConfig _testsGeneratorConfig;
        private BufferBlock<GeneratedTestClass> _additionalProducerBuffer = new BufferBlock<GeneratedTestClass>();

        public TestsGenerator(TestGeneratorConfig testsGeneratorConfig)
        {
            _testsGeneratorConfig = testsGeneratorConfig;
        }

        public void Generate<TResultPayload>(ParallelCodeReader reader, ParallelCodeWriter writer)
        {
            DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            ExecutionDataflowBlockOptions processingTaskRestriction = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _testsGeneratorConfig.MaxProcessingTasksCount };
            ExecutionDataflowBlockOptions outputTaskRestriction = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _testsGeneratorConfig.MaxWritingTasksCount };

            TransformBlock<string, GeneratedTestClass> producerBuffer = 
                new TransformBlock<string, GeneratedTestClass>(new Func<string, GeneratedTestClass>(Produce), processingTaskRestriction);

            ActionBlock<GeneratedTestClass> generatedTestsBuffer = new ActionBlock<GeneratedTestClass>(
               ((generatedClass) => writer.Consume(generatedClass)), outputTaskRestriction);

            producerBuffer.LinkTo(generatedTestsBuffer, linkOptions);
            _additionalProducerBuffer.LinkTo(generatedTestsBuffer, linkOptions);

            Parallel.ForEach(reader.Provide(), async generatedClass => {
                await producerBuffer.SendAsync(generatedClass);
            });

            producerBuffer.Complete();
        }

        private GeneratedTestClass Produce(string sourceCode)
        {
            SyntaxTreeInfoBuilder syntaxTreeInfoBuilder = new SyntaxTreeInfoBuilder(sourceCode);
            SyntaxTreeInfo syntaxTreeInfo = syntaxTreeInfoBuilder.GetSyntaxTreeInfo();

            TestClassTemplateGenerator testTemplatesGenerator = new TestClassTemplateGenerator(syntaxTreeInfo);
            List<GeneratedTestClass> testTemplates = testTemplatesGenerator.GetTestTemplates().ToList();

            if (testTemplates.Count > 1)
            {
                for (int i = 1; i < testTemplates.Count; ++i)
                {
                    _additionalProducerBuffer.Post(new GeneratedTestClass(
                        testTemplates.ElementAt(i).TestClassName, 
                        testTemplates.ElementAt(i).TestClassData));
                }
            }

            return new GeneratedTestClass(testTemplates.First().TestClassName, testTemplates.First().TestClassData);
        }
    }
}
