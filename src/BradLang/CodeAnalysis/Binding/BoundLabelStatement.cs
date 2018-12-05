namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol label)
        {
            Label = label;
        }
        public override BoundNodeKind Kind => BoundNodeKind.Label;

        public LabelSymbol Label { get; }
    }
}
