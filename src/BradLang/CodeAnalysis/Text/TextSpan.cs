namespace BradLang.CodeAnalysis.Text;

public struct TextSpan
{
    public static TextSpan FromBounds(int start, int end)
    {
        return new TextSpan(start, end - start);
    }

    public TextSpan(int start, int length)
    {
        Start = start;
        Length = length;
    }

    public int Start { get; }
    public int Length { get; }

    public int End => Start + Length;

    public override string ToString() => $"{Start}..{End}";
}
