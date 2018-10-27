using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Syntax;
using Xunit;

namespace BradLang.Tests
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
        [InlineData("a = 42", 42)]
        [InlineData("a = b = 10", 10)]
        [InlineData("1 == 1 ? \"Correct\" : \"Incorrect\"", "Correct")]
        [InlineData("1 == 2 ? \"Incorrect\" : \"Correct\"", "Correct")]
        public void Evaluator_Evaluate(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);   
            var compilation = new Compilation(syntaxTree);
            
            var variables = new Dictionary<VariableSymbol, object>();

            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }
    } 

}
