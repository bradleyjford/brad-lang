namespace BradLang.CodeAnalysis.Binding;

sealed class BoundLabel
{
    public BoundLabel(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => Name;
}
