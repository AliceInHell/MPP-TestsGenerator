using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsGeneratorLibrary;
using System.Linq;
using System.Threading.Tasks;

namespace TestsGeneratorUnitTests
{
    [TestClass]
    public class TestsGeneratorTestsClass
    {
        private int _readingTasksCount;
        private int _writingTasksCount;

        private TestGeneratorConfig _config;

        private string _outputDirectory;
        private List<string> _paths;
        private string _programmText;

        private CodeReader _reader;
        private CodeWriter _writer;

        private TestsGenerator _generator;
        private Task _task;

        private CompilationUnitSyntax _compilationUnitSyntax;

        [TestInitialize]
        public void SetUp()
        {
            _readingTasksCount = 3;
            _writingTasksCount = 3;
            _config = new TestGeneratorConfig(_readingTasksCount, _writingTasksCount);

            _outputDirectory = "./";

            _paths = new List<string>();
            _paths.Add("./SomeClass.csSource");
            _paths.Add("./AnotherClass.csSource");

            _reader = new CodeReader();
            _writer = new CodeWriter(_outputDirectory);

            _generator = new TestsGenerator(_config);
            _task = _generator.Generate(_reader, _writer, _paths);

            _task.Start();
            _task.Wait();

            _programmText = File.ReadAllText("./SomeClassTests.cs");
            SyntaxTree syntaxTree;
            syntaxTree = CSharpSyntaxTree.ParseText(_programmText);

            _compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot();
        }

        [TestMethod]
        public void UnitUsingDirectiveTest()
        {
            IEnumerable<UsingDirectiveSyntax> NUnitUsingDirective =
                from usingDirective in _compilationUnitSyntax.DescendantNodes().OfType<UsingDirectiveSyntax>()
                where usingDirective.Name.ToString() == "Microsoft.VisualStudio.TestTools.UnitTesting"
                select usingDirective;

            Assert.IsNotNull(NUnitUsingDirective.FirstOrDefault());
        }

        [TestMethod]
        public void SomeClassNamespaceTest()
        {
            IEnumerable<NamespaceDeclarationSyntax> Namespace =
                from namespaceDeclaration in _compilationUnitSyntax.DescendantNodes().OfType<NamespaceDeclarationSyntax>()
                where namespaceDeclaration.Name.ToString() == "TestsGeneratorTestsClass.Tests"
                select namespaceDeclaration;

            Assert.IsNotNull(Namespace.FirstOrDefault());
        }

        [TestMethod]
        public void SomeClassNameTest()
        {
            IEnumerable<ClassDeclarationSyntax> className =
                from classDeclaration in _compilationUnitSyntax.DescendantNodes().OfType<ClassDeclarationSyntax>()
                where classDeclaration.Identifier.ValueText == "SomeClassTests"
                select classDeclaration;

            Assert.IsNotNull(className.FirstOrDefault());
        }

        [TestMethod]
        public void TSomeClassMethodsCountTest()
        {
            IEnumerable<MethodDeclarationSyntax> methods =
                from methodDeclaration in _compilationUnitSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
                select methodDeclaration;

            Assert.IsTrue(methods.Count() == 3);
        }

        
        [TestMethod]
        public void SecondMethodAssertFailTest()
        {
            IEnumerable<MethodDeclarationSyntax> method =
                from methodDeclaration in _compilationUnitSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
                where methodDeclaration.Identifier.ValueText == "SecondMethodTest"
                select methodDeclaration;

            IEnumerable<MemberAccessExpressionSyntax> asserts =
                from assertDeclaration in method.FirstOrDefault().Body.DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                where assertDeclaration.Expression.ToString() == "Assert"
                select assertDeclaration;

            Assert.IsNotNull(asserts.FirstOrDefault());
        }
    }
}
