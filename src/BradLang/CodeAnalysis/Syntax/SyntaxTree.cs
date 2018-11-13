using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);

            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.From(text);

            return ParseTokens(sourceText);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
        {
            var lexer = new Lexer(text);

            while (true)
            {
                var token = lexer.Lex();

                if (token.Kind == SyntaxKind.EndOfFileToken)
                {
                    break;
                }

                yield return token;
            }
        }

        private SyntaxTree(SourceText text)
        {
            var parser = new Parser(text);

            Root = parser.ParseCompilationUnit();
            Diagnostics = parser.Diagnostics.ToImmutableArray();
        }

        public CompilationUnitSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}
