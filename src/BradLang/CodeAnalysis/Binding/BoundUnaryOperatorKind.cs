using System;

namespace BradLang.CodeAnalysis.Binding
{
    enum BoundUnaryOperatorKind
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
}
