using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using BradLang.CodeAnalysis.Text;

namespace BradLang.Tests.CodeAnalysis
{
    public sealed class AnnotatedText
    {
        public static AnnotatedText Parse(string text)
        {
            text = text.Unindent();

            var textBuilder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();

            var position = 0;

            foreach (var c in text)
            {
                if (c == '[')
                {
                    startStack.Push(position);
                }
                else if (c == ']')
                {
                    if (startStack.Count == 0)
                    {
                        throw new ArgumentException("Too many ']' in text", nameof(text));
                    }

                    var start = startStack.Pop();
                    var span = TextSpan.FromBounds(start, position);

                    spanBuilder.Add(span);
                }
                else
                {
                    position++;

                    textBuilder.Append(c);
                }
            }

            if (startStack.Count > 0)
            {
                throw new ArgumentException("Missing ']' in text", nameof(text));
            }

            return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
        }

        AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }
    }
}
