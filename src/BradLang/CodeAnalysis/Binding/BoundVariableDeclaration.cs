namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundVariableDeclaration : BoundStatement
    {
        public BoundVariableDeclaration(VariableSymbol variableSymbol, BoundExpression initializer)
        {
            VariableSymbol = variableSymbol;
            Initializer = initializer;
        }

        public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;

        public VariableSymbol VariableSymbol { get; }
        public BoundExpression Initializer { get; }
    }
}
