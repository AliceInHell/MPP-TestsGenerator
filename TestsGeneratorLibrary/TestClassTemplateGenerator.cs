using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using TestsGeneratorLibrary.Structures;

namespace TestsGeneratorLibrary
{
    /// <summary>
    /// Generate class code using roslyn.
    /// </summary>
    internal class TestClassTemplateGenerator
    {
        /// <summary>
        /// Class syntax tree.
        /// </summary>
        private readonly SyntaxTreeInfo _syntaxTreeInfo;

        /// <summary>
        /// Initializes a new instance of a <see cref="TestClassTemplateGenerator"/> class.
        /// </summary>
        /// <param name="syntaxTreeInfo"></param>
        public TestClassTemplateGenerator(SyntaxTreeInfo syntaxTreeInfo)
        {
            _syntaxTreeInfo = syntaxTreeInfo;
        }

        /// <summary>
        /// Generate test template.
        /// </summary>
        /// <returns>Generated classes.</returns>
        public IEnumerable<GeneratedTestClass> GetTestTemplates()
        {
            List<GeneratedTestClass> testTemplates = new List<GeneratedTestClass>();

            foreach (ClassInfo classInfo in _syntaxTreeInfo.Classes)
            {
                NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(QualifiedName(
                    IdentifierName(classInfo.NamespaceName), IdentifierName("Tests")));

                CompilationUnitSyntax compilationUnit = CompilationUnit()
                    .WithUsings(GetUsingDirectives(classInfo))
                    .WithMembers(SingletonList<MemberDeclarationSyntax>(namespaceDeclaration
                        .WithMembers(SingletonList<MemberDeclarationSyntax>(ClassDeclaration(classInfo.Name + "Tests")
                            .WithAttributeLists(
                                SingletonList(
                                    AttributeList(
                                        SingletonSeparatedList(
                                            Attribute(
                                                IdentifierName("TestClass"))))))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithMembers(GetClassMembers(classInfo))))));

                string fileName = $"{classInfo.Name}Tests.cs";
                string fileData = compilationUnit.NormalizeWhitespace().ToFullString();

                testTemplates.Add(new GeneratedTestClass(fileName, fileData));
            }

            return testTemplates;
        }

        /// <summary>
        /// Parse using directives.
        /// </summary>
        /// <param name="classInfo">Class inforation.</param>
        /// <returns></returns>
        private SyntaxList<UsingDirectiveSyntax> GetUsingDirectives(ClassInfo classInfo)
        {
            List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>
            {
                UsingDirective(IdentifierName("System")),
                UsingDirective(QualifiedName(
                QualifiedName(
                    IdentifierName("System"), 
                    IdentifierName("Collections")), 
                IdentifierName("Generic"))),
                UsingDirective(QualifiedName(
                    IdentifierName("System"), 
                    IdentifierName("Linq"))),                
                UsingDirective(
                    QualifiedName(
                        QualifiedName(
                            QualifiedName(
                            IdentifierName("Microsoft"), 
                            IdentifierName("VisualStudio")), 
                        IdentifierName("TestTools")), 
                    IdentifierName("UnitTesting"))),
                UsingDirective(IdentifierName(classInfo.NamespaceName))
            };

            return List(usingDirectives);
        }

        /// <summary>
        /// Parse class members.
        /// </summary>
        /// <param name="classInfo">Class information.</param>
        /// <returns>Syntax tree.</returns>
        private SyntaxList<MemberDeclarationSyntax> GetClassMembers(ClassInfo classInfo)
        {
            List<MemberDeclarationSyntax> classMembers = new List<MemberDeclarationSyntax>();            

            foreach (MethodInfo methodInfo in classInfo.Methods)
            {
                classMembers.Add(GetTestMethodDeclaration(methodInfo));
            }

            return List(classMembers);
        }

        /// <summary>
        /// Parse method declaration.
        /// </summary>
        /// <param name="attributeName">Atribute.</param>
        /// <param name="methodName">Method name.</param>
        /// <param name="blockMembers">Block members.</param>
        /// <returns>Method declaration.</returns>
        private MethodDeclarationSyntax GetMethodDeclaration(string attributeName, string methodName, SyntaxList<StatementSyntax> blockMembers)
        {
            MethodDeclarationSyntax methodDeclaration = MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)), 
                Identifier(methodName))
                .WithAttributeLists(
                    SingletonList(
                        AttributeList(
                            SingletonSeparatedList(
                                Attribute(
                                    IdentifierName(attributeName))))))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithBody(Block(blockMembers));

            return methodDeclaration;
        }        

        /// <summary>
        /// Parse method declaration.
        /// </summary>
        /// <param name="methodInfo">Method information.</param>
        /// <returns>Method declaration.</returns>
        private MethodDeclarationSyntax GetTestMethodDeclaration(MethodInfo methodInfo)
        {
            List<StatementSyntax> blockMembers = new List<StatementSyntax>();
            List<ArgumentSyntax> parameters = new List<ArgumentSyntax>();

            ArgumentListSyntax args = ArgumentList(SingletonSeparatedList(
                Argument(
                    LiteralExpression(
                        SyntaxKind.StringLiteralExpression, 
                        Literal("autogenerated")))));

            blockMembers.Add(ExpressionStatement(
                InvocationExpression(
                    GetMemberAccessExpression(
                        "Assert", 
                        "Fail"))
                    .WithArgumentList(args)));

            return GetMethodDeclaration("TestMethod", $"{methodInfo.Name}Test", List(blockMembers));
        }

        /// <summary>
        /// Parse method expression.
        /// </summary>
        /// <param name="objectName">Object name.</param>
        /// <param name="memberName">Member name.</param>
        /// <returns>Method access expression.</returns>
        private MemberAccessExpressionSyntax GetMemberAccessExpression(string objectName, string memberName)
        {
            return MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, 
                IdentifierName(objectName), 
                IdentifierName(memberName));
        }             
    }
}
