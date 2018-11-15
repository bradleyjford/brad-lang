namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundVariableDeclaration : BoundStatement
    {
        public BoundVariableDeclaration(VariableSymbol variableSymbol, BoundExpression initializer)
        {
            VariableSymbol = variableSymbol;
            InitializerExpression = initializer;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;

        public VariableSymbol VariableSymbol { get; }
        public BoundExpression InitializerExpression { get; }
    }
}
