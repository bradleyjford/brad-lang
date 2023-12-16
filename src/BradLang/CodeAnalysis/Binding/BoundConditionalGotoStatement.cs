using BradLang.CodeAnalysis.Symbols;

namespace BradLang.CodeAnalysis.Binding;

sealed class BoundConditionalGotoStatement : BoundStatement
{
    public BoundConditionalGotoStatement(BoundLabel label, BoundExpression condition, bool jumpIfTrue = true)
    {
        Label = label;
        Condition = condition;
        JumpIfTrue = jumpIfTrue;
    }

    public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;

    public BoundLabel Label { get; }
    public BoundExpression Condition { get; }
    public bool JumpIfTrue { get; }
}
