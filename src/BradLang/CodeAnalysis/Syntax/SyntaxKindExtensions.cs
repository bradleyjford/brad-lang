using System;
using System.Collections.Generic;
using System.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public static class SyntaxKindExtensions
    {
        public static bool IsKeyword(this SyntaxKind kind)
        {
            return kind.ToString().EndsWith("Keyword");
        }

        public static bool IsIdentifier(this SyntaxKind kind)
        {
            return kind == SyntaxKind.IdentifierToken;
        }

        public static bool IsNumber(this SyntaxKind kind)
        {
            return kind == SyntaxKind.NumberToken;
        }

        public static bool IsString(this SyntaxKind kind)
        {
            return kind == SyntaxKind.StringToken;
        }
    }
}
