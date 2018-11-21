namespace BradLang.CodeAnalysis.Binding
{
    sealed class BoundMethodDeclaration : BoundStatement
    {
        public BoundMethodDeclaration(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
        }
        public override BoundNodeKind Kind => BoundNodeKind.MethodDeclaration;

        public MethodInfo MethodInfo { get; }
    }
}
