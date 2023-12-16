using System.Collections.Immutable;

namespace BradLang.CodeAnalysis.Text;

public sealed class SourceText
{
    public static SourceText From(string text)
    {
        return new SourceText(text);
    }

    readonly string _text;

    SourceText(string text)
    {
        _text = text;

        Lines = ParseLines(this, text);
    }

    static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
    {
        var result = ImmutableArray.CreateBuilder<TextLine>();

        var position = 0;
        var lineStart = 0;

        while (position < text.Length)
        {
            var lineBreakWidth = GetLineBreakWidth(text, position);

            if (lineBreakWidth == 0)
            {
                position++;
            }
            else
            {
                var line = CreateLine(sourceText, position, lineStart, lineBreakWidth);

                result.Add(line);

                position += lineBreakWidth;
                lineStart = position;
            }
        }

        if (position >= lineStart)
        {
            var line = CreateLine(sourceText, position, lineStart, 0);

            result.Add(line);
        }

        return result.ToImmutable();
    }

    static TextLine CreateLine(SourceText sourceText, int position, int lineStart, int lineBreakWidth)
    {
        var lineLength = position - lineStart;

        return new TextLine(sourceText, lineStart, lineLength, lineLength + lineBreakWidth);
    }

    static int GetLineBreakWidth(string text, int position)
    {
        var c = text[position];
        var l = position + 1 >= text.Length ? '\0' : text[position + 1];

        if (c == '\r' && l == '\n')
        {
            return 2;
        }

        if (c == '\r' || c == '\n')
        {
            return 1;
        }

        return 0;
    }

    public ImmutableArray<TextLine> Lines { get; }

    public char this[int index] => _text[index];

    public int Length => _text.Length;

    public int GetLineIndex(int position)
    {
        var lower = 0;
        var upper = Lines.Length - 1;

        while (lower <= upper)
        {
            var index = lower + (upper - lower) / 2;
            var start = Lines[index].Start;

            if (position == start)
            {
                return index;
            }

            if (start > position)
            {
                upper = index - 1;
            }
            else
            {
                lower = index + 1;
            }
        }

        return lower - 1;
    }

    public override string ToString() => _text;

    public string ToString(int start, int length) => _text.Substring(start, length);

    public string ToString(TextSpan span) => ToString(span.Start, span.Length);
}
