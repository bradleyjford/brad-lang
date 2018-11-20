using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis;
using BradLang.CodeAnalysis.Syntax;
using Xunit;

namespace BradLang.Tests.CodeAnalysis
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+45", 45)]
        [InlineData("-1", -1)]
        [InlineData("1 + 2", 3)]
        [InlineData("17 - 9", 8)]
        [InlineData("2 * 7", 14)]
        [InlineData("21 / 7", 3)]
        [InlineData("5 % 3", 2)]
        [InlineData("1 + 2 * 3", 7)]
        [InlineData("(1 + 2) * 3", 9)]
        [InlineData("(540)", 540)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("10 == 10", true)]
        [InlineData("50 != 12", true)]
        [InlineData("10 > 9", true)]
        [InlineData("10 > 100", false)]
        [InlineData("10 >= 10", true)]
        [InlineData("10 >= 100", false)]
        [InlineData("9 < 10", true)]
        [InlineData("100 < 10", false)]
        [InlineData("10 <= 10", true)]
        [InlineData("100 <= 10", false)]
        [InlineData("true && true", true)]
        [InlineData("true || false", true)]
        [InlineData("true && false", false)]
        [InlineData("\"Hello\"", "Hello")]
        [InlineData("\"Hello, \" + \"World!\"", "Hello, World!")]
        [InlineData("\"Hello\" == \"Hello\"", true)]
        [InlineData("\"Hello\" == \"World\"", false)]
        [InlineData("\"Hello\" != \"Hello\"", false)]
        [InlineData("\"Hello\" != \"World\"", true)]
        [InlineData("{ var a = 42 a }", 42)]
        [InlineData("{ var a = 0 var b = 0 a = b = 10 a }", 10)]
        [InlineData("1 == 1 ? \"Correct\" : \"Incorrect\"", "Correct")]
        [InlineData("1 == 2 ? \"Incorrect\" : \"Correct\"", "Correct")]
        [InlineData("{ var a = 10 if (a == 10) { a = 50 } a}", 50)]
        [InlineData("{ var a = 10 if (a == 50) { a = 50 } a}", 10)]
        [InlineData("{ var a = 10 if (a == 10) { a = 50 } else { a = 0 } a}", 50)]
        [InlineData("{ var a = 10 if (a == 50) { a = 50 } else { a = 0 } a}", 0)]
        public void Evaluator_Evaluate(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);   
            var compilation = new Compilation(syntaxTree);
            
            var variables = new Dictionary<VariableSymbol, object>();

            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
        {
            var text = @"
                {
                    var x = 10
                    var y = 100
                    {
                        var x = 10
                    }
                    var [x] = 5
                }
            ";

            var diagnostics = @"
                Variable ""x"" is already declared.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_BlockStatement_NoInfiniteLoop()
        {
            var text = @"
                {
                [)][]
            ";

            var diagnostics = @"
                Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>.
                Unexpected token <EndOfFileToken>, expected <CloseBraceToken>.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            
            var compilation = new Compilation(syntaxTree);

            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = diagnosticText.UnindentLines();

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
            {
                throw new Exception("ERROR: Must mark as many spans as there are expected diagnostics");
            }

            Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;

                Assert.Equal(expectedMessage, actualMessage);

                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;

                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    } 
}
