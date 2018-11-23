using System;
using System.Collections.Generic;
using System.Linq;
using BradLang.CodeAnalysis.Syntax;
using Xunit;

namespace BradLang.Tests.CodeAnalysis.Syntax
{
    public class LexerTests
    {
        [Fact]
        public void Lexer_Lex_AllTokensAreTested()
        {
            var tokenKinds = Enum.GetValues(typeof(SyntaxKind))
                .Cast<SyntaxKind>()
                .Where(sk => sk.ToString().EndsWith("Keyword") || sk.ToString().EndsWith("Token"));

            var testedTokenKinds = GetSyntaxTokens().Concat(GetSeparatorSyntaxTokens()).Select(t => t.kind);

            var untestedTokenKinds = new SortedSet<SyntaxKind>(tokenKinds);
            
            untestedTokenKinds.ExceptWith(testedTokenKinds);
            untestedTokenKinds.Remove(SyntaxKind.UnknownToken);
            untestedTokenKinds.Remove(SyntaxKind.EndOfFileToken);

            Assert.Empty(untestedTokenKinds);
        }

        [Theory]
        [MemberData(nameof(GetSyntaxTokensData))]
        public void Lexer_Lex_CanParseToken(SyntaxKind kind, string text)
        {
            var tokens = ParseTokens(text).ToArray();

            Assert.Single(tokens);
            Assert.Equal(kind, tokens[0].Kind);
            Assert.Equal(text, tokens[0].Text);
        }

        [Theory]
        [MemberData(nameof(GetSyntaxTokenPairsData))]
        public void Lexer_Lex_CanParseTokenPairs(SyntaxKind kind1, string text1, SyntaxKind kind2, string text2)
        {
            var tokens = ParseTokens(text1 + text2).ToArray();

            Assert.Equal(2, tokens.Length);

            Assert.Equal(kind1, tokens[0].Kind);
            Assert.Equal(text1, tokens[0].Text);
            Assert.Equal(kind2, tokens[1].Kind);
            Assert.Equal(text2, tokens[1].Text);
        }

        [Theory]
        [MemberData(nameof(GetSyntaxTokenPairsWithSeparatorsData))]
        public void Lexer_Lex_CanParseTokenPairsWithSeparator(
            SyntaxKind kind1, string text1, 
            SyntaxKind SeparatorKind, string SeparatorText,
            SyntaxKind kind2, string text2)
        {
            var tokens = ParseTokens(text1 + SeparatorText + text2).ToArray();

            Assert.Equal(3, tokens.Length);

            Assert.Equal(kind1, tokens[0].Kind);
            Assert.Equal(text1, tokens[0].Text);
            Assert.Equal(SeparatorKind, tokens[1].Kind);
            Assert.Equal(SeparatorText, tokens[1].Text);
            Assert.Equal(kind2, tokens[2].Kind);
            Assert.Equal(text2, tokens[2].Text);
        }

        public static IEnumerable<object[]> GetSyntaxTokensData()
        {
            foreach (var t in GetSyntaxTokens().Concat(GetSeparatorSyntaxTokens()))
            {
                yield return new object[] { t.kind, t.text };
            }
        }

        public static IEnumerable<object[]> GetSyntaxTokenPairsData()
        {
            foreach (var t in GetSyntaxTokenPairs())
            {
                yield return new object[] { t.kind1, t.text1, t.kind2, t.text2 };
            }
        }

        public static IEnumerable<object[]> GetSyntaxTokenPairsWithSeparatorsData()
        {
            foreach (var t in GetSyntaxTokenPairsWithSeparators())
            {
                yield return new object[] { t.kind1, t.text1, t.SeparatorKind, t.SeparatorText, t.kind2, t.text2 };
            }
        }

        static IEnumerable<(SyntaxKind kind1, string text1, SyntaxKind kind2, string text2)> GetSyntaxTokenPairs()
        {
            foreach (var t1 in GetSyntaxTokens())
            {
                foreach (var t2 in GetSyntaxTokens())
                {
                    if (!CanPairToken(t1.kind, t2.kind))
                    {
                        continue;
                    }

                    yield return (t1.kind, t1.text, t2.kind, t2.text);
                }
            }
        }

        static IEnumerable<(SyntaxKind kind1, string text1, SyntaxKind SeparatorKind, string SeparatorText, SyntaxKind kind2, string text2)> GetSyntaxTokenPairsWithSeparators()
        {
            foreach (var t1 in GetSyntaxTokens())
            {
                foreach (var t2 in GetSyntaxTokens())
                {
                    if (!CanPairToken(t1.kind, t2.kind))
                    {
                        foreach (var s in GetSeparatorSyntaxTokens())
                        {
                            yield return (t1.kind, t1.text, s.kind, s.text, t2.kind, t2.text);
                        }
                    }
                }
            }
        }
        static bool CanPairToken(SyntaxKind kind1, SyntaxKind kind2) 
        {
            if (kind1 == SyntaxKind.IdentifierToken && kind2 == SyntaxKind.IdentifierToken)
                return false;

            if (kind1 == SyntaxKind.NumberToken && kind2 == SyntaxKind.NumberToken)
                return false;

            if (kind1 == SyntaxKind.IdentifierToken && kind2 == SyntaxKind.NumberToken)
                return false;

            if (SyntaxFacts.IsKeyword(kind1) && SyntaxFacts.IsKeyword(kind2))
                return false;

            if (SyntaxFacts.IsKeyword(kind1) && kind2 == SyntaxKind.NumberToken)
                return false;

            if ((SyntaxFacts.IsKeyword(kind1) && kind2 == SyntaxKind.IdentifierToken) ||
                (kind1 == SyntaxKind.IdentifierToken && SyntaxFacts.IsKeyword(kind2)))
                return false;

            if (kind1 == SyntaxKind.LessThanToken && kind2 == SyntaxKind.EqualsToken)
                return false;

            if (kind1 == SyntaxKind.LessThanToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return false;

            if (kind1 == SyntaxKind.GreaterThanToken && kind2 == SyntaxKind.EqualsToken)
                return false;

            if (kind1 == SyntaxKind.GreaterThanToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return false;

            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsToken)
                return false;

            if (kind1 == SyntaxKind.BangToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return false;

            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsToken)
                return false;

            if (kind1 == SyntaxKind.EqualsToken && kind2 == SyntaxKind.EqualsEqualsToken)
                return false;

            if (kind1 == SyntaxKind.PipeToken && kind2 == SyntaxKind.PipeToken)
                return false;

            if (kind1 == SyntaxKind.PipeToken && kind2 == SyntaxKind.PipePipeToken)
                return false;

            if (kind1 == SyntaxKind.AmpersandToken && kind2 == SyntaxKind.AmpersandToken)
                return false;

            if (kind1 == SyntaxKind.AmpersandToken && kind2 == SyntaxKind.AmpersandAmpersandToken)
                return false;

            if (kind1 == SyntaxKind.PlusToken && kind2 == SyntaxKind.PlusToken)
                return false;

            if (kind1 == SyntaxKind.PlusToken && kind2 == SyntaxKind.PlusPlusToken)
                return false;

            if (kind1 == SyntaxKind.MinusToken && kind2 == SyntaxKind.MinusToken)
                return false;

            if (kind1 == SyntaxKind.MinusToken && kind2 == SyntaxKind.MinusMinusToken)
                return false;

            return true;
        }

        static IEnumerable<(SyntaxKind kind, string text)> GetSyntaxTokens()
        {
            var fixedLengthTokens = Enum.GetValues(typeof(SyntaxKind))
                .Cast<SyntaxKind>()
                .Select(sk => ( kind: sk, text: SyntaxFacts.GetText(sk) ))
                .Where(k => k.text != null);

            var variableLengthTokens = new[] {
                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "12"),

                (SyntaxKind.StringToken, "\"\""),
                (SyntaxKind.StringToken, "\"a\""),
                (SyntaxKind.StringToken, "\"a b\""),
                (SyntaxKind.StringToken, "\"ab \\\"ab\""),

                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "ab"),
                (SyntaxKind.IdentifierToken, "a1"),
                (SyntaxKind.IdentifierToken, "a12")
            };

            return fixedLengthTokens.Concat(variableLengthTokens);
        }

        static IEnumerable<(SyntaxKind kind, string text)> GetSeparatorSyntaxTokens()
        {
            return new[] {
                (SyntaxKind.WhiteSpaceToken, " "),
                (SyntaxKind.WhiteSpaceToken, "  "),
                (SyntaxKind.WhiteSpaceToken, "\t"),
                (SyntaxKind.WhiteSpaceToken, "\t\t"),
                (SyntaxKind.WhiteSpaceToken, "\r"),
                (SyntaxKind.WhiteSpaceToken, "\n"),
                (SyntaxKind.WhiteSpaceToken, "\r\n"),
            };
        }

        IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            return SyntaxTree.ParseTokens(text);
        }
    }
}
