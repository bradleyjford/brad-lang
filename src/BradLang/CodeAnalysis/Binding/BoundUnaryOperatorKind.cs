namespace BradLang.CodeAnalysis.Binding;

internal enum BoundUnaryOperatorKind
{
    Identity,
    Negation,
    LogicalNegation,
    PrefixIncrement,
    PrefixDecrement,
    PostfixIncrement,
    PostfixDecrement,
    OnesCompliment,
}
