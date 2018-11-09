﻿using System;
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

        public void Generate(ParallelCodeReader reader, CodeWriter writer)
        {
            DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            ExecutionDataflowBlockOptions processingTaskRestriction = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _testsGeneratorConfig.MaxProcessingTasksCount };
            ExecutionDataflowBlockOptions outputTaskRestriction = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _testsGeneratorConfig.MaxWritingTasksCount };

            TransformBlock<string, GeneratedTestClass> producerBuffer = 
                new TransformBlock<string, GeneratedTestClass>(new Func<string, GeneratedTestClass>(Produce), processingTaskRestriction);

            ActionBlock<GeneratedTestClass> resultWritingAction = new ActionBlock<GeneratedTestClass>(
               ((generatedClass) => writer.Consume(generatedClass)), outputTaskRestriction);

            producerBuffer.LinkTo(resultWritingAction, linkOptions);
            _additionalProducerBuffer.LinkTo(resultWritingAction, linkOptions);

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

            return new GeneratedTestClass(testTemplates.First().TestClassName, testTemplates.First().TestClassData);
        }
    }
}
