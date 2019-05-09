namespace BradLang.CodeAnalysis.Binding
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement bodyStatement)
        {
            Condition = condition;
            Body = bodyStatement;
        }

        public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;

        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }
    }
}
