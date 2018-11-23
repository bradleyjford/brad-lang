namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundReturnStatement : BoundStatement
    {
        public BoundReturnStatement(BoundExpression value)
        {
            Value = value;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ReturnStatement;

        public BoundExpression Value { get; }
    }
}
