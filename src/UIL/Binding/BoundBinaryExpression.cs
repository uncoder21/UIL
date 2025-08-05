using UIL.Symbols;
using UIL.Syntax;

namespace UIL.Binding;

public sealed class BoundBinaryExpression : BoundExpression
{
    public BoundExpression Left { get; }
    public SyntaxKind OperatorKind { get; }
    public BoundExpression Right { get; }

    public BoundBinaryExpression(BoundExpression left, SyntaxKind operatorKind, BoundExpression right, TypeSymbol type)
        : base(type, null, Nullability.NotNull)
    {
        Left = left;
        OperatorKind = operatorKind;
        Right = right;
    }

    public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
}
