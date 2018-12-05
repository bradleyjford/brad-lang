namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(LabelSymbol label)
        {
            Label = label;
        }
        public override BoundNodeKind Kind => BoundNodeKind.GotoStatement;

        public LabelSymbol Label { get; }
    }
}
