using System;
using System.Collections.Generic;

namespace BradLang.CodeAnalysis.Syntax
{
    public class Lexer
    {
        readonly ReadOnlyMemory<char> _text;
        int _position;

        readonly DiagnosticBag _diagnostics;

        public Lexer(string text)
        {
            _text = text.AsMemory();

            _position = 0;
            _diagnostics = new DiagnosticBag();
        }

        public IEnumerable<Diagnostic> Diagnostics => _diagnostics;

        public SyntaxToken Lex()
        {
            var text = _text.Span;

            while (true)
            {
                if (_position >= _text.Length)
                {
                    return new SyntaxToken(SyntaxKind.EndOfFileToken, text.Length - 1, "\0", null);
                }

                var start = _position;

                var currentChar = Current;

                if (Char.IsWhiteSpace(currentChar))
                {
                    do
                    {
                        _position++;
                    } while (Char.IsWhiteSpace(Current));

                    return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, new String(text.Slice(start, _position - start)), null);
                }

                if (Char.IsLetter(currentChar))
                {
                    do
                    {
                        _position++;
                    } while (Char.IsLetter(Current) || Char.IsDigit(Current));

                    var textValue = new String(text.Slice(start, _position - start));

                    var kind = SyntaxFacts.GetKeywordKind(textValue);

                    return new SyntaxToken(kind, start, textValue, null);
                }

                if (Char.IsDigit(currentChar))
                {
                    do
                    {
                        _position++;
                    } while (Char.IsDigit(Current));

                    var textValue = new String(text.Slice(start, _position - start));

                    if (!Int32.TryParse(textValue, out var value))
                    {
                        _diagnostics.ReportInvalidNumber(new TextSpan(start, _position - start), textValue, typeof(Int32));
                    }

                    return new SyntaxToken(SyntaxKind.NumberToken, start, textValue, value);
                }

                if (currentChar == '"')
                {
                    while (true)
                    {
                        _position++;

                        if (Current == '\0' && Previous != '"')
                        {
                            _diagnostics.ReportUnterminatedStringConstant(start, _position - start);

                            return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "", null);
                        }

                        if (Current == '"' && Previous != '\\')
                        {
                            break;
                        }
                    }

                    _position++;

                    var textValue = new String(text.Slice(start, _position - start));

                    return new SyntaxToken(SyntaxKind.StringToken, start, textValue, textValue.Substring(1, _position - start - 2));
                }

                switch (currentChar)
                {
                    case '<':
                        if (PeekNext == '=')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.LessThanEqualsToken, start, "<=", null);
                        }

                        return new SyntaxToken(SyntaxKind.LessThanToken, _position++, "<", null);
                    case '>':
                        if (PeekNext == '=')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.GreaterThanEqualsToken, start, ">=", null);
                        }

                        return new SyntaxToken(SyntaxKind.GreaterThanToken, _position++, ">", null);
                    case '=':
                        if (PeekNext == '=')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                        }

                        return new SyntaxToken(SyntaxKind.EqualsToken, _position++, "=", null);
                    case '!':
                        if (PeekNext == '=')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.BangEqualsToken, start, "!=", null);
                        }

                        return new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null);
                    case '|':
                        if (PeekNext == '|')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.PipePipeToken, start, "||", null);
                        }

                        return new SyntaxToken(SyntaxKind.PipeToken, _position++, "|", null);
                    case '&':
                        if (PeekNext == '&')
                        {
                            _position += 2;
                            return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, start, "&&", null);
                        }

                        return new SyntaxToken(SyntaxKind.AmpersandToken, _position++, "&", null);
                    case ':':
                        return new SyntaxToken(SyntaxKind.ColonToken, _position++, ":", null);
                    case '?':
                        return new SyntaxToken(SyntaxKind.QuestionMarkToken, _position++, "?", null);
                    case '+':
                        return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                    case '-':
                        return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                    case '*':
                        return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                    case '/':
                        return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                    case '(':
                        return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                    case ')':
                        return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                    case '%':
                        return new SyntaxToken(SyntaxKind.PercentToken, _position++, "%", null);
                    default:
                        _diagnostics.ReportBadCharacter(start, currentChar);
                        return new SyntaxToken(SyntaxKind.UnknownToken, _position++, new String(text.Slice(start, 1)), null);
                }
            }
        }

        char Current
        {
            get
            {
                if (_position > _text.Length - 1)
                {
                    return '\0';
                }

                return _text.Span[_position];
            }
        }

        char Previous => Peek(-1);

        char PeekNext => Peek(1);

        char Peek(int offset)
        {
            var index = _position + offset;

            if (index > _text.Length - 1)
            {
                return '\0';
            }

            return _text.Span[index];
        }
    }
}
