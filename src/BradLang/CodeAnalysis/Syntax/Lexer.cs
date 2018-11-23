using System;
using System.Collections.Generic;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CodeAnalysis.Syntax
{
    public class Lexer
    {
        readonly SourceText _text;
        readonly DiagnosticBag _diagnostics;

        int _start;
        int _position;

        SyntaxKind _kind;
        object _value;

        public Lexer(SourceText text)
        {
            _text = text;

            _position = 0;
            _diagnostics = new DiagnosticBag();
        }

        public IEnumerable<Diagnostic> Diagnostics => _diagnostics;

        public SyntaxToken Lex()
        {
            _start = _position;
            _kind = SyntaxKind.UnknownToken;
            _value = null;

            var text = _text;

            switch (Current)
            {
                case '\0':
                    _kind = SyntaxKind.EndOfFileToken;
                    break;
                case ';':
                    _position++;
                    _kind = SyntaxKind.SemicolonToken;
                    break;
                case '<':
                    _position++;

                    if (Current == '=')
                    {
                        _position++;
                        _kind = SyntaxKind.LessThanEqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.LessThanToken;
                    }

                    break;
                case '>':
                    _position++;

                    if (Current == '=')
                    {
                        _position++;
                        _kind = SyntaxKind.GreaterThanEqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.GreaterThanToken;
                    }

                    break;
                case '=':
                    _position++;

                    if (Current == '=')
                    {
                        _position++;
                        _kind = SyntaxKind.EqualsEqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.EqualsToken;
                    }

                    break;
                case '!':
                    _position++;

                    if (Current == '=')
                    {
                        _position++;
                        _kind = SyntaxKind.BangEqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.BangToken;
                    }

                    break;
                case '|':
                    _position++;

                    if (Current == '|')
                    {
                        _position++;
                        _kind = SyntaxKind.PipePipeToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.PipeToken;
                    }

                    break;
                case '&':
                    _position++;

                    if (Current == '&')
                    {
                        _position++;
                        _kind = SyntaxKind.AmpersandAmpersandToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.AmpersandToken;
                    }

                    break;
                case '+':
                    _position++;

                    if (Current == '+')
                    {
                        _position++;
                        _kind = SyntaxKind.PlusPlusToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.PlusToken;
                    }

                    break;
                case '-':
                    _position++;

                    if (Current == '-')
                    {
                        _position++;
                        _kind = SyntaxKind.MinusMinusToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.MinusToken;
                    }

                    break;
                case '*':
                    _position++;
                    _kind = SyntaxKind.StarToken;
                    break;
                case '/':
                    _position++;
                    _kind = SyntaxKind.SlashToken;
                    break;
                case ':':
                    _position++;
                    _kind = SyntaxKind.ColonToken;
                    break;
                case '?':
                    _position++;
                    _kind = SyntaxKind.QuestionMarkToken;
                    break;
                case '(':
                    _position++;
                    _kind = SyntaxKind.OpenParenthesisToken;
                    break;
                case ')':
                    _position++;
                    _kind = SyntaxKind.CloseParenthesisToken;
                    break;
                case '{':
                    _position++;
                    _kind = SyntaxKind.OpenBraceToken;
                    break;
                case '}':
                    _position++;
                    _kind = SyntaxKind.CloseBraceToken;
                    break;                    
                case '%':
                    _position++;
                    _kind = SyntaxKind.PercentToken;
                    break;
                default:
                    var currentChar = Current;

                    if (Char.IsWhiteSpace(currentChar))
                    {
                        ReadWhiteSpace();
                    }
                    else if (Char.IsLetter(currentChar))
                    {
                        ReadKeywordOrIdentifier();
                    }
                    else if (Char.IsDigit(currentChar))
                    {
                        ReadNumber();
                    }
                    else if (currentChar == '"')
                    {
                        ReadString();
                    }
                    else
                    {
                        _diagnostics.ReportBadCharacter(_start, currentChar);
                        _position++;
                        _kind = SyntaxKind.UnknownToken;
                    }

                    break;
            }

            var tokenText = SyntaxFacts.GetText(_kind);

            if (tokenText == null)
            {
                tokenText = text.ToString(_start, _position - _start);
            }

            return new SyntaxToken(_kind, _start, tokenText, _value);
        }

        void ReadWhiteSpace()
        {
            do
            {
                _position++;
            } while (Char.IsWhiteSpace(Current));

            _kind = SyntaxKind.WhiteSpaceToken;
        }

        void ReadKeywordOrIdentifier()
        {
            do
            {
                _position++;
            } while (Char.IsLetter(Current) || Char.IsDigit(Current));

            var textValue = _text.ToString(_start, _position - _start);

            _kind = SyntaxFacts.GetKeywordKind(textValue);
        }

        void ReadNumber()
        {
            do
            {
                _position++;
            } while (Char.IsDigit(Current));

            var textValue = _text.ToString(_start, _position - _start);

            if (!Int32.TryParse(textValue, out var value))
            {
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, _position - _start), textValue, typeof(int));
            }

            _kind = SyntaxKind.NumberToken;
            _value = value;
        }

        void ReadString()
        {
            do
            {
                _position++;

                if (Current == '\0')
                {
                    _diagnostics.ReportUnterminatedStringConstant(new TextSpan(_start, _position - _start));

                    _kind = SyntaxKind.EndOfFileToken;
                }
                else if (Current == '"' && Previous != '\\')
                {
                    _position++;
                    
                    _kind = SyntaxKind.StringToken;
                    _value = _text.ToString(_start + 1, _position - _start - 2);
                }
            } while (_kind == SyntaxKind.UnknownToken);
        }

        char Current
        {
            get
            {
                if (_position > _text.Length - 1)
                {
                    return '\0';
                }

                return _text[_position];
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

            return _text[index];
        }
    }
}
